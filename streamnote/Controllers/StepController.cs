using System;
using System.Collections.Generic;
using System.Linq;       
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational;
using Streamnote.Relational.Data;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;
using Streamnote.Web.Mapper;

namespace Streamnote.Web.Controllers
{
    public class StepController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly StepMapper StepMapper;
        private readonly UserManager<ApplicationUser> UserManager;

        public StepController(ApplicationDbContext context, StepMapper stepMapper, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            StepMapper = stepMapper;
            UserManager = userManager;
        } 

        // GET: Step/Create
        public async Task<IActionResult> New(int taskId)
        {
            var user = await UserManager.GetUserAsync(User);

            var step = new StepDescriptor
            {         
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Content = null,
                IsCompleted = false,
                TaskId = taskId
            };

            return PartialView("_CreateStep", step);
        }

        // POST: Step/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string content, bool isPublic, int taskId)
        {
            if (content != null && content.Length > 0)
            {
                var user = await UserManager.GetUserAsync(User);
                var now = DateTime.UtcNow;
                var task = Context.Tasks.Find(taskId);
                var step = new Step
                {
                    Created = now,
                    Modified = now,
                    Content = content,
                    IsCompleted = false,
                    Task = task
                };

                if (ModelState.IsValid)
                {
                    Context.Add(step);
                    await Context.SaveChangesAsync();
                    var newStep = Context.Steps.FirstOrDefault(s => s.Created == now);

                    return PartialView("_Step", StepMapper.MapDescriptor(newStep, user.Id));
                }
            }

            throw new Exception();
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int stepId)
        {
            var user = await UserManager.GetUserAsync(User);
            var step = Context.Steps.FirstOrDefault(t => t.Id == stepId);

            step.IsCompleted = !step.IsCompleted;

            try
            {
                Context.Update(step);
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StepExists(stepId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var newStep = Context.Steps                      
                .FirstOrDefault(i => i.Id == stepId);

            return PartialView("_Step", StepMapper.MapDescriptor(newStep, user.Id)); ;

        }

        private bool StepExists(int id)
        {
            return Context.Steps.Any(e => e.Id == id);
        }
    }
}
