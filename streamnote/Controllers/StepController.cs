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

                return PartialView("_Step", StepMapper.MapDescriptor(newStep, user.Id)); ;
            }
            return View(step);
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

        /*// GET: Step/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stepItem = await Context.Steps.FindAsync(id);
            if (stepItem == null)
            {
                return NotFound();
            }
            return View(stepItem);
        }

        // POST: Step/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Step<IActionResult> Edit(int id, [Bind("Id,Created,Modified,Title,Description,Status,IsPublic,OwnedByUsername")] StepItem stepItem)
        {
            if (id != stepItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(stepItem);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StepItemExists(stepItem.Id))
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
            return View(stepItem);
        }

        // GET: Step/Delete/5
        public async Step<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stepItem = await Context.Steps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stepItem == null)
            {
                return NotFound();
            }

            return View(stepItem);
        }

        // POST: Step/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Step<IActionResult> DeleteConfirmed(int id)
        {
            var stepItem = await Context.Steps.FindAsync(id);
            Context.Steps.Remove(stepItem);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
         */
        private bool StepExists(int id)
        {
            return Context.Steps.Any(e => e.Id == id);
        }
    }
}
