using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational;
using Streamnote.Relational.Data;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;
using Streamnote.Web.Mapper;

namespace Streamnote.Web.Controllers
{
    public class TaskCommentController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly TaskCommentMapper TaskCommentMapper;
        private readonly UserManager<ApplicationUser> UserManager;

        public TaskCommentController(ApplicationDbContext context, TaskCommentMapper commentMapper, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            TaskCommentMapper = commentMapper;
            UserManager = userManager;
        }

        // GET: Comment/Create
        public async Task<IActionResult> New(int taskId)
        {
            var user = await UserManager.GetUserAsync(User);

            var comment = new TaskCommentDescriptor
            {         
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Content = null,
                TaskId = taskId,
                CreatedByLoggedInUser = false,
                UserName = user.UserName,
                UserImageContentType = user.ImageContentType,
                UserImage = user.Image

            };

            return PartialView("_CreateTaskComment", comment);
        }

        // POST: Comment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string content, bool isPublic, int taskId)
        {
            var user = await UserManager.GetUserAsync(User);
            var now = DateTime.UtcNow;
            var task = Context.Tasks.Find(taskId);
            var comment = new TaskComment
            {          
                Created = now,
                Modified = now,
                Content = content,
                User = user,
                Task = task
            };

            if (ModelState.IsValid)
            {
                var newComment = Context.Add(comment);
                await Context.SaveChangesAsync();

                return PartialView("_TaskComment", TaskCommentMapper.MapDescriptor(newComment.Entity, user.Id)); ;
            }
            throw new Exception();
        }
    }
}
