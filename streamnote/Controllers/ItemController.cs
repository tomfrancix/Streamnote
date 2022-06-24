using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational;
using Streamnote.Relational.Data;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Interfaces.Services;
using Streamnote.Web.Mapper;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;
using WebGrease.Css.Extensions;

namespace Streamnote.Web.Controllers
{
    /// <summary>
    /// Controller for item actions.
    /// </summary>
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ItemMapper ItemMapper;
        private readonly CommentMapper CommentMapper;
        private readonly ImageProcessingHelper ImageProcessingHelper;
        private readonly IItemRepository ItemRepository;
        private readonly ITopicRepository TopicRepository;
        private readonly IS3Service S3Service;
        private readonly BlockMapper BlockMapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="itemMapper"></param>
        /// <param name="commentMapper"></param>
        /// <param name="imageProcessingHelper"></param>
        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ItemMapper itemMapper, CommentMapper commentMapper, ImageProcessingHelper imageProcessingHelper, IItemRepository itemRepository, ITopicRepository topicRepository, IS3Service s3Service, BlockMapper blockMapper)
        {
            Context = context;
            UserManager = userManager;
            ItemMapper = itemMapper;
            CommentMapper = commentMapper;
            ImageProcessingHelper = imageProcessingHelper;
            ItemRepository = itemRepository;
            TopicRepository = topicRepository;
            S3Service = s3Service;
            BlockMapper = blockMapper;
        }

        /// <summary>
        /// Get the items created by the user.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {              
            var user = await UserManager.GetUserAsync(User);

            var model = ItemRepository.QueryUsersItems(user)
                .OrderByDescending(i => i.Id)
                .ToList();

            var items = ItemMapper.MapDescriptors(model, user.Id);

            return View(items);
        }

        /// <summary>
        /// Get the details for an item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            var user = await UserManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var item = await ItemRepository.QueryAllItems().FirstOrDefaultAsync(m => m.Id == id);

            var itemDescriptor = ItemMapper.MapDescriptor(item, user.Id);

            var comments = Context.Comments
                .Include(c => c.User)
                .Where(c => c.Item.Id == item.Id)
                .ToList();

            itemDescriptor.Comments = CommentMapper.MapDescriptors(comments, user.Id).OrderByDescending(c => c.Id).ToList();

            itemDescriptor.IsDetails = true;

            return View(itemDescriptor);
        }

        /// <summary>
        /// Create an item view.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View(new ItemDescriptor());
        }

        /// <summary>
        /// Create an item action.
        /// </summary>                 
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="isPublic"></param>
        /// <param name="image"></param>
        /// <param name="topics"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> CreateOrUpdate(int? id, string title, string content, string isPublic, IFormFile image,
            string[] selectedTopics)
        {
            var item = new Item();
            if (id is > 0)
            {
                var existing = await Context.Items.Include(i => i.Topics).FirstOrDefaultAsync(i => i.Id == id);

                if (existing != null)
                {
                    existing.Images = existing.Images;
                    existing.Modified = DateTime.UtcNow;
                    existing.Content = content;
                    existing.Title = title;
                    existing.IsPublic = isPublic == "true";

                    existing = await AppendTopics(existing, selectedTopics);

                    existing = await AppendImage(existing, image);

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            await ItemRepository.UpdateItemAsync(existing);
                        }
                        catch (Exception ex)
                        {
                            //
                        }
                    }
                }
            }
            else
            {
                var now = DateTime.UtcNow;
                item = new Item
                {
                    Id = 0,
                    Created = now,
                    Modified = now,
                    Title = title,
                    Content = content,
                    Images = new List<ItemImage>(),
                    IsPublic = isPublic == "true",
                    CommentCount = 0,
                    ShareCount = 0,
                    LikeCount = 0,
                    User = await UserManager.GetUserAsync(User),
                    Comments = null,
                    Likes = null,
                    Topics = new List<Topic>()
                };

                item = await AppendTopics(item, selectedTopics);

                item = await AppendImage(item, image);

                try
                {
                    return await ItemRepository.CreateItem(item);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return (int)id;
        }

        /// <summary>
        /// Upload a thumnail for a post.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<IActionResult> UploadThumbnail(int postId, IFormFile image)
        {
            if (postId > 0 && image != null)
            {
                var item = await Context.Items.FirstOrDefaultAsync(i => i.Id == postId);

                if (item != null)
                {
                    item = await AppendImage(item, image); 
                    Context.Items.Update(item);
                    await Context.SaveChangesAsync(); 
                    return RedirectToAction("Edit", new { item.Id });
                }

            }

            throw new Exception("This image or post wasn't good enough.");
        }

        /// <summary>
        /// Edit an item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await Context.Items
                .Include(i => i.Blocks)
                .Include(i => i.Images)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var user = await UserManager.GetUserAsync(User);

            return View(ItemMapper.MapDescriptor(item, user.Id));
        }

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = ItemRepository.Read((int)id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        /// <summary>
        /// Confirm that an item was deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await Context.Items.FindAsync(id);
            Context.Items.Remove(item);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<Item> AppendTopics(Item item, string[] selectedTopics)
        {
            if (selectedTopics != null)
            {
                var chosenTopics = selectedTopics.ToList();

                var existingTopics = new List<Topic>();

                foreach (var chosenTopic in chosenTopics.Select(t => t.ToLower()))
                {
                    var existingTopic = await Context.Topics.FirstOrDefaultAsync(t => t.Name.ToLower() == chosenTopic);

                    if (existingTopic is null)
                    {
                        var newTopic = new Topic
                        {
                            Name = chosenTopic.ToLower(),
                            ItemCount = 1
                        };

                        Context.Add(newTopic);
                        existingTopics.Add(newTopic);
                    }

                    if (existingTopic is not null)
                    {
                        // Topic exists
                        if (!item.Topics.Contains(existingTopic))
                        {
                            item.Topics.Add(existingTopic);
                        }

                        existingTopics.Add(existingTopic);
                    }
                }

                var itemTopics = item.Topics.ToList();

                foreach (var topic in itemTopics)
                {
                    if (!existingTopics.Contains(topic))
                    {
                        item.Topics.Remove(topic);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Append an image to the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="file">The form file.</param>
        /// <returns></returns>
        public async Task<Item> AppendImage(Item item, IFormFile file)
        {
            if (file != null)
            {
                if (item.Images == null)
                {
                    item.Images = new List<ItemImage>();
                }

                byte[] bytes;

                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    bytes = ms.ToArray();
                }
                
                var imageName = "IMG_" + file.FileName;

                if (bytes.Length < 49152)
                {
                    var image = new ItemImage()
                    {
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow,
                        ImageContentType = file.ContentType,
                        Name = imageName,
                        Bytes = bytes
                    };

                    item.Images.Add(image);
                }
                else
                {
                    await S3Service.UploadImage(imageName, file.ContentType, bytes, new Dictionary<string, string>());

                    item.Images.Add(new ItemImage
                    {         
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow,
                        Name = imageName,
                        FullS3Location = "https://sn-content.s3.eu-west-1.amazonaws.com/Images/" + imageName,
                        Bytes = new byte[] {},
                        ImageContentType = file.ContentType
                    });
                }
            }

            return item;
        }

        /// <summary>
        /// Add a new block to a post.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PostBlockDescriptor> AddTextBlock(int postId)
        {
            var post = await Context.Items.FindAsync(postId);

            if (post != null)
            {
                post.Blocks ??= new List<PostBlock>();

                var newBlock = new PostBlock
                {         
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    Type = BlockType.Text,
                    Text = "",
                    Item = post
                };

                post.Blocks.Add(newBlock);

                await Context.SaveChangesAsync();

                return BlockMapper.MapDescriptor(newBlock);
            }

            throw new InvalidDataException("This post does not exist: " + postId);
        }

        /// <summary>
        /// Add a new block to a post.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> GetTextBlockHtml(PostBlockDescriptor block)
        { 
            return Task.FromResult<ActionResult>(PartialView("_NewBlock", block));
        }

        /// <summary>
        /// Update a block on a post.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UpdateTextBlock(int blockId, string html)
        {
            var block = await Context.Blocks.FindAsync(blockId);
            block.Modified = DateTime.UtcNow;
            block.Text = html;
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Update a block on a post.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteTextBlock(int blockId)
        {
            var block = await Context.Blocks.FindAsync(blockId);
            Context.Blocks.Remove(block);
            await Context.SaveChangesAsync();
        }
    }
}
