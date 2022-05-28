using System.Collections.Generic;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// Mapper for taskComments.
    /// </summary>
    public class TaskCommentMapper
    {
        private readonly UserMapper UserMapper;
        private readonly DateTimeHelper DateTimeHelper;

        public TaskCommentMapper(DateTimeHelper dateTimeHelper, UserMapper userMapper)
        {
            DateTimeHelper = dateTimeHelper;
            UserMapper = userMapper;
        }

        /// <summary>
        /// Map a list of taskComments.
        /// </summary>
        /// <param name="taskComments"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<TaskCommentDescriptor> MapDescriptors(List<TaskComment> taskComments, string userId)
        {
            var taskCommentDescriptors = new List<TaskCommentDescriptor>();

            foreach (var taskComment in taskComments)
            {
                taskCommentDescriptors.Add(MapDescriptor(taskComment, userId));
            }

            return taskCommentDescriptors;
        }

        /// <summary>
        /// Map a single taskComment.
        /// </summary>
        /// <param name="taskComment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TaskCommentDescriptor MapDescriptor(TaskComment taskComment, string userId)
        {
            var time = (taskComment.Created > taskComment.Modified) ? taskComment.Created : taskComment.Modified;

            var timeString = DateTimeHelper.GetFriendlyDateTime(time);

            return new TaskCommentDescriptor
            {
                Id = taskComment.Id,
                Created = taskComment.Created,
                Modified = taskComment.Modified,
                Content = taskComment.Content,
                UserName = taskComment.User.UserName,
                UserImageContentType = taskComment.User.ImageContentType,
                UserImage = taskComment.User.Image,
                CreatedByLoggedInUser = taskComment.User.Id == userId
            };
        }
    }
}
