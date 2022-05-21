using System;
using System.Collections.Generic;
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
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ItemMapper ItemMapper;
        private readonly CommentMapper CommentMapper;

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ItemMapper itemMapper, CommentMapper commentMapper)
        {
            Context = context;
            UserManager = userManager;
            ItemMapper = itemMapper;
            CommentMapper = commentMapper;
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
                .Where(u => u.User != null)
                .OrderByDescending(i => i.Id)
                .ToList();

            var items = ItemMapper.MapDescriptors(model, user.Id);

            return View(items);
        }

        /// <summary>
        /// Get the single item.
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
                .FirstOrDefaultAsync(m => m.Id == id);

            var itemDescriptor = ItemMapper.MapDescriptor(item, user.Id);

            var comments = Context.Comments
                .Where(c => c.Item.Id == item.Id)
                .ToList();

            itemDescriptor.Comments = CommentMapper.MapDescriptors(comments, user.Id);

            return View(itemDescriptor);
        }

        // GET: Item/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Item/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
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
                        item.Image = br.ReadBytes((int) fs.Length);
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

        // GET: Item/Edit/5
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

        // POST: Item/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,IsPublic,Image")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            item.Modified = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(item);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            return View(item);
        }

        // GET: Item/Delete/5
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

        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await Context.Items.FindAsync(id);
            Context.Items.Remove(item);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return Context.Items.Any(e => e.Id == id);
        }
    }
}
