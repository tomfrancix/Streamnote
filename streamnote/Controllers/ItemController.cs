﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using streamnote.Data;
using streamnote.Mapper;
using streamnote.Models;

namespace streamnote.Controllers
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="itemMapper"></param>
        /// <param name="commentMapper"></param>
        /// <param name="imageProcessingHelper"></param>
        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ItemMapper itemMapper, CommentMapper commentMapper, ImageProcessingHelper imageProcessingHelper)
        {
            Context = context;
            UserManager = userManager;
            ItemMapper = itemMapper;
            CommentMapper = commentMapper;
            ImageProcessingHelper = imageProcessingHelper;
        }

        /// <summary>
        /// Get the items created by the user.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {              
            var user = await UserManager.GetUserAsync(User);

            var model = Context.Items
                .Where(i => i.User.Id == user.Id)
                .Include(b => b.User)
                .Include(b => b.Likes).ThenInclude(u => u.User)
                .Where(u => u.User != null)
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

            var item = await Context.Items
                .Include(u => u.User)
                .Include(u => u.Likes).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            var itemDescriptor = ItemMapper.MapDescriptor(item, user.Id);

            var comments = Context.Comments
                .Where(c => c.Item.Id == item.Id)
                .ToList();

            itemDescriptor.Comments = CommentMapper.MapDescriptors(comments, user.Id);

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
        /// <param name="item"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,IsPublic,Image")] Item item, IFormFile image)
        {
            item.Created = DateTime.UtcNow;
            item.Modified = DateTime.UtcNow;
            item.User = await UserManager.GetUserAsync(User);

            if (image != null)
            {
                item.ImageContentType = image.ContentType;
                using (var fs = image.OpenReadStream())
                {
                    using (var br = new BinaryReader(fs))
                    {
                        item.Image = ImageProcessingHelper.ResizeImageFile(br.ReadBytes((int) fs.Length), 1024);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                Context.Add(item);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
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
        /// Edit an item action.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item, IFormFile image)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            var existing = Context.Items.FirstOrDefault(i => i.Id == id);

            if (existing != null)
            {
                existing.Image = existing.Image;
                existing.ImageContentType = existing.ImageContentType;
                existing.Modified = DateTime.UtcNow;

                if (image != null)
                {
                    existing.ImageContentType = image.ContentType;
                    using (var fs = image.OpenReadStream())
                    {
                        using (var br = new BinaryReader(fs))
                        {
                            existing.Image = br.ReadBytes((int)fs.Length);
                        }
                    }
                }

                if (ModelState.IsValid)
                {

                    try
                    {
                        Context.Update(existing);
                        await Context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ItemExists(existing.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
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

            var item = await Context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
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

        /// <summary>
        /// Check if an item exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ItemExists(int id)
        {
            return Context.Items.Any(e => e.Id == id);
        }
    }
}
