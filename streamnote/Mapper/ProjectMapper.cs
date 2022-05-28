using System.Collections.Generic;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// Mapper for projects.
    /// </summary>
    public class ProjectMapper
    {
        private readonly UserMapper UserMapper;
        private readonly DateTimeHelper DateTimeHelper;
        private readonly TaskMapper TaskMapper;

        public ProjectMapper(DateTimeHelper dateTimeHelper, UserMapper userMapper, TaskMapper taskMapper)
        {
            DateTimeHelper = dateTimeHelper;
            UserMapper = userMapper;
            TaskMapper = taskMapper;
        }

        /// <summary>
        /// Map a list of projects.
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ProjectDescriptor> MapDescriptors(List<Project> projects, string userId)
        {
            var projectDescriptors = new List<ProjectDescriptor>();

            foreach (var project in projects)
            {
                projectDescriptors.Add(MapDescriptor(project, userId));
            }

            return projectDescriptors;
        }

        /// <summary>
        /// Map a single project.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ProjectDescriptor MapDescriptor(Project project, string userId)
        {
            var time = (project.Created > project.Modified) ? project.Created : project.Modified;

            var timeString = DateTimeHelper.GetFriendlyDateTime(time);

            return new ProjectDescriptor
            {
                Id = project.Id,
                Created = project.Created,
                Modified = project.Modified,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                IsPublic = project.IsPublic,
                CreatedBy = project.CreatedBy,
                OwnedByUsername = project.OwnedByUsername,
                Tasks = TaskMapper.MapDescriptors(project.Tasks, userId),
                Users = UserMapper.MapDescriptors(project.Users)
            };
        }
    }
}
