using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserMapper UserMapper;

        public ProjectsController(ApplicationDbContext context, ProjectMapper projectMapper, UserManager<ApplicationUser> userManager, TaskMapper taskMapper, IProjectRepository projectRepository, ITaskRepository taskRepository, UserMapper userMapper)
        {
            Context = context;
            ProjectMapper = projectMapper;
            UserManager = userManager;
            TaskMapper = taskMapper;
            ProjectRepository = projectRepository;
            TaskRepository = taskRepository;
            UserMapper = userMapper;
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
                

                if (id != null)
                {
                    var allTasks = TaskMapper.MapDescriptors(TaskRepository.QueryAllTasks(user, (int)id).ToList(), user.Id);

                    foreach (var task in allTasks)
                    {
                        task.OwnedByLoggedInUser = task.OwnedByUsername == user.UserName;
                    }

                    organizer.Tasks = allTasks
                        .Where(t => t.Status == TodoStatus.Unstarted || (t.Status == TodoStatus.Started && t.OwnedByUsername != user.UserName))
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
            if (project is { Title: { Length: > 0 } })
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
            }

            return RedirectToAction(nameof(View));
        }

        /// <summary>
        /// Get participants for a project.
        /// </summary>
        /// <param name="projectId"></param> 
        /// <returns></returns>
        public async Task<IActionResult> GetParticipants(int projectId)
        {
            if (projectId > 0)
            {
                var user = await UserManager.GetUserAsync(User);

                var project = ProjectRepository
                    .QueryAllProjects(user)
                    .First(p => p.Id == projectId);

                var userIdsInProject = new List<string>();

                if (project.Users is { Count: > 0 })
                {
                    userIdsInProject.AddRange(project.Users.Select(u => u.Id));
                }

                var participants = Context.Users.Where(u => userIdsInProject.Contains(u.Id)).ToList();

                return PartialView("_ParticipantsModalBody", UserMapper.MapDescriptors(participants));
            }

            throw new InvalidDataException("Parameters were not correct.");
        }

        /// <summary>
        /// Get potential participant for a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPotentialParticipants(int projectId, string query)
        {
            if (projectId > 0 && query.Length > 3)
            {
                var user = await UserManager.GetUserAsync(User);

                var project = ProjectRepository
                    .QueryAllProjects(user)
                    .First(p => p.Id == projectId);

                var userIdsInProject = new List<string>
                {
                    project.CreatedBy.Id
                };

                if (project.Users is { Count: > 0 })
                {
                    userIdsInProject.AddRange(project.Users.Select(u => u.Id));
                }

                var potentialParticipants = Context.Users.Where(u => 
                       (u.UserName.StartsWith(query) || u.FirstName.StartsWith(query)) 
                    && !userIdsInProject.Contains(u.Id)).ToList();

                return PartialView("_ParticipantSelectOptions", UserMapper.MapDescriptors(potentialParticipants));
            }

            throw new InvalidDataException("Parameters were not correct.");
        }

        /// <summary>
        /// Add participant to a project by username.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddOrRemoveParticipant(string username, int projectId)
        {
            if (username is { Length: > 0 } && projectId > 0)
            {
                var user = await UserManager.GetUserAsync(User);
                
                var project = ProjectRepository
                    .QueryAllProjects(user)
                    .First(p => p.Id == projectId);

                if (project.Users is { Count: > 0 } && project.Users.Any(u => u.UserName == username))
                {
                    var userToRemove = Context.Users.First(u => u.UserName == username);

                    project.Users.Remove(userToRemove);
                }
                else
                {
                    var participant = await UserManager.FindByNameAsync(username);

                    project.Users.Add(participant);
                }

                await Context.SaveChangesAsync();

                return PartialView("_ParticipantSelectOptions", UserMapper.MapDescriptors(project.Users));
            }

            throw new InvalidDataException("Parameters were not correct.");
        }
    }
}
