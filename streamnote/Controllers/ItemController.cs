using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Interfaces.Services;
using Streamnote.Web.Mapper;
using Streamnote.Relational.Models;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="itemMapper"></param>
        /// <param name="commentMapper"></param>
        /// <param name="imageProcessingHelper"></param>
        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ItemMapper itemMapper, CommentMapper commentMapper, ImageProcessingHelper imageProcessingHelper, IItemRepository itemRepository, ITopicRepository topicRepository, IS3Service s3Service)
        {
            Context = context;
            UserManager = userManager;
            ItemMapper = itemMapper;
            CommentMapper = commentMapper;
            ImageProcessingHelper = imageProcessingHelper;
            ItemRepository = itemRepository;
            TopicRepository = topicRepository;
            S3Service = s3Service;
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
            return View();
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
        public async Task<IActionResult> CreateOrUpdate(int? id, string title, string content, string isPublic, IFormFile image, string selectedTopics)
        {                                      
            if (id is > 0)
            {
                var existing = ItemRepository.Read((int)id);

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
                        await ItemRepository.UpdateItemAsync(existing);

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                var now = DateTime.UtcNow;
                var item = new Item
                {
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

                if (ModelState.IsValid)
                {
                    await ItemRepository.CreateItem(item);

                    return RedirectToAction(nameof(Index));
                }
            }

            return Json(true);
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

            var item = await Context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
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

        public async Task<Item> AppendTopics(Item item, string selectedTopics)
        {
            if (selectedTopics != null)
            {
                var listOfTopics = selectedTopics.Split(",");

                foreach (var topic in listOfTopics)
                {
                    var existing = TopicRepository.QueryExistingTopic(topic);

                    if (existing is { Name: { } })
                    {
                        await TopicRepository.IncrementItemCount(existing);

                    }
                    else
                    {
                        var newTopic = new Topic
                        {
                            Name = topic.ToLower(),
                            ItemCount = 1
                        };

                        existing = await TopicRepository.CreateTopic(newTopic);
                    }

                    item.Topics.Add(existing);
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
    }
}
