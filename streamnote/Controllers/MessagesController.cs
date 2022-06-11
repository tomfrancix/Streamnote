using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Web.Mapper;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Controllers
{
    /// <summary>
    /// Controller for message actions.
    /// </summary>
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly UserMapper UserMapper;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, UserMapper userMapper)
        {
            Context = context;
            UserManager = userManager;
            UserMapper = userMapper;
        }

        /// <summary>
        /// Get all the messages between the two users.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string username)
        {
            var loggedInUser = await UserManager.GetUserAsync(User);
            var messages = new List<Message>();

            if (username != null)
            {
                ViewBag.MessageToUserName = username;
                messages = await Context.Messages
                    .Include(m => m.SentBy)
                    .Include(m => m.SentTo)
                    .Where(m => (m.SentBy.UserName == username && m.SentTo.UserName == loggedInUser.UserName) ||
                                (m.SentTo.UserName == username && m.SentBy.UserName == loggedInUser.UserName))
                    .ToListAsync();
            }

            var users = UserMapper.MapDescriptors(Context.Users.Where(u => u.Id != loggedInUser.Id).ToList());
            var model = new MessagesDescriptor()
            {
                Messages = (username != null) ? messages : null,
                Users = users
            };

            return View(model);
        }

        /// <summary>
        /// Create a message view.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a message action.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> Create(string username, string text)
        {
            var model = new Message()
            {
                Text = text,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                MessageSeen = false,
                SentBy = await UserManager.GetUserAsync(User),
                SentTo = Context.Users.FirstOrDefault(u => u.UserName == username)
            };

            if (ModelState.IsValid)
            {

                Context.Add(model);
                await Context.SaveChangesAsync();
                return PartialView("_Message", model);
            }

            throw new Exception("Model state was not valid!");
        }

        /// <summary>
        /// Edit a message view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await Context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        /// <summary>
        /// Edit a message action.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Created,Modified,Text,Content,Image,ImageContentType,IsPublic")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(message);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            return View(message);
        }

        /// <summary>
        /// Delete a message action.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await Context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        /// <summary>
        /// Confirm a message was deleted.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await Context.Messages.FindAsync(id);
            Context.Messages.Remove(message);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Check if a message exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool MessageExists(int id)
        {
            return Context.Messages.Any(e => e.Id == id);
        }

        /// <summary>
        /// Check if a user has unread messages.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> UserHasUnreadMessages()
        {
            var user = await UserManager.GetUserAsync(User);

            if (user != null)
            {
                var unreadMessages = Context.Messages
                    .Where(u => u.SentTo.Id == user.Id)
                    .Where(m => !m.MessageSeen).ToList().Count;

                if (unreadMessages > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Confirm a message has been seen by receiver.
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task ConfirmMessageHasBeenSeen(int messageId)
        {
            var message = await Context.Messages
                .Include(m => m.SentTo)
                .Include(m => m.SentBy)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            message.MessageSeen = true;

            Context.Update(message);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public async Task SoftDelete(int id)
        {
            var obj = await Context.Messages.FindAsync(id);

            Context.Messages.Remove(obj);
            await Context.SaveChangesAsync();
        }
    }
}
