using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces.Repositories;
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
        private readonly IProjectRepository ProjectRepository;
        private readonly ITaskRepository TaskRepository;

        public ProjectsController(ApplicationDbContext context, ProjectMapper projectMapper, UserManager<ApplicationUser> userManager, TaskMapper taskMapper, IProjectRepository projectRepository, ITaskRepository taskRepository)
        {
            Context = context;
            ProjectMapper = projectMapper;
            UserManager = userManager;
            TaskMapper = taskMapper;
            ProjectRepository = projectRepository;
            TaskRepository = taskRepository;
        }

        // GET: Projects
        public async Task<IActionResult> View(int? id)
        {
            var user = await UserManager.GetUserAsync(User);

            var projects = ProjectRepository.QueryAllProjects(user).ToList();
            
            var organizer = new OrganizerDescriptor()
            {
                Projects = ProjectMapper.MapDescriptors(projects, user.Id),
                IsViewingProject = id is > 0,
                ProjectId = id ?? 0
            };

            if (organizer.IsViewingProject)
            {     
                foreach (var project in organizer.Projects)
                {
                    if (project.Id == id)
                    {
                        project.IsCurrentProject = true;
                    }
                }

                if (id != null)
                {
                    var allTasks = TaskMapper.MapDescriptors(TaskRepository.QueryAllTasks(user, (int)id).ToList(), user.Id);

                    organizer.Tasks = allTasks
                        .Where(t => t.Status == TodoStatus.Unstarted && t.OwnedByUsername != user.UserName)
                        .OrderBy(t => t.Rank).ToList();
                    organizer.YourTasks = allTasks
                        .Where(t => (t.Status == TodoStatus.Started) && t.OwnedByUsername == user.UserName)
                        .OrderBy(t => t.Rank).ToList();
                    organizer.CompletedTasks = allTasks
                        .Where(t => t.Status == TodoStatus.Finished || t.Status == TodoStatus.Delivered)
                        .OrderBy(t => t.Rank).ToList();
                }
            }

            return View(organizer);
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
    }
}
