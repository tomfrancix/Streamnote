using System;
using System.Collections.Generic;
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
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly ProjectMapper ProjectMapper;
        private readonly TaskMapper TaskMapper;
        private readonly UserManager<ApplicationUser> UserManager;

        public ProjectsController(ApplicationDbContext context, ProjectMapper projectMapper, UserManager<ApplicationUser> userManager, TaskMapper taskMapper)
        {
            Context = context;
            ProjectMapper = projectMapper;
            UserManager = userManager;
            TaskMapper = taskMapper;
        }

        // GET: Projects
        public async Task<IActionResult> View(int? id)
        {
            var user = await UserManager.GetUserAsync(User);

            var tasks = Context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Steps) 
                .Include(t => t.Comments).ThenInclude(c => c.User)
                .Include(t => t.CreatedBy)
                .Where(t => t.CreatedBy.Id == user.Id);

            if (id != null && id > 0)
            {
                tasks = tasks.Where(i => i.Project.Id == id);
            }

            var projects = await Context.Projects
                .Include(t => t.Tasks)
                .ToListAsync();

            var allTasks = TaskMapper.MapDescriptors(tasks.ToList(), user.Id);

            var organizer = new OrganizerDescriptor()
            {
                Projects = ProjectMapper.MapDescriptors(projects, user.Id),
                Tasks = allTasks.Where(t => t.Status == TodoStatus.Unstarted && t.OwnedByUsername != user.UserName).ToList(),
                YourTasks = allTasks.Where(t => (t.Status == TodoStatus.Started || t.Status == TodoStatus.Finished) && t.OwnedByUsername == user.UserName).ToList(),
                CompletedTasks = allTasks.Where(t => t.Status == TodoStatus.Delivered).ToList(),
                IsViewingProject = (id != null && id > 0),
                ProjectId = id ?? 0
            };

            return View(organizer);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await Context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
                               
        [HttpPost]
        public IActionResult New()
        {
            var project = new ProjectDescriptor
            {                    
                Title = null,
                Description = null,
                Status = TodoStatus.Unstarted,
                IsPublic = false,
                CreatedBy = null,
                OwnedByUsername = null,
                Tasks = null,
                Users = null,
                StatusOptions = Enum.GetValues<TodoStatus>().ToList()
            };

            return PartialView("_CreateProject", project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Created,Modified,Title,Description,Status,IsPublic,OwnedByUsername")] Project project)
        {
            var user = await UserManager.GetUserAsync(User);
            project.CreatedBy = user;
            project.Created = DateTime.UtcNow;
            project.Modified = DateTime.UtcNow;
            project.Status = TodoStatus.Unstarted;

            if (ModelState.IsValid)
            {
                Context.Add(project);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(View));
            }
            return RedirectToAction(nameof(View));
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await Context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Created,Modified,Title,Description,Status,IsPublic,OwnedByUsername")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(project);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await Context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await Context.Projects.FindAsync(id);
            Context.Projects.Remove(project);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return Context.Projects.Any(e => e.Id == id);
        }
    }
}
