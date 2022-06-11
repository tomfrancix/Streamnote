using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces.Services;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Service
{
    /// <summary>
    /// The data culling service.
    /// </summary>
    public class DataCullingService : IDataCullingService
    {
        private readonly ApplicationDbContext Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public DataCullingService(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Delete a task and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTask(int id)
        {
            var task = Context.Tasks
                .Include(t => t.Steps)
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == id);

            if (task != null)
            {
                if (task.Steps.Count > 0)
                {
                    DeleteSteps(task.Steps);
                }

                if (task.Comments.Count > 0)
                {
                    DeleteTaskComments(task.Comments);
                }

                Context.Tasks.Remove(task);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of comments.
        /// </summary>
        /// <param name="comments">The comments to remove.</param>
        /// <returns></returns>
        public bool DeleteTaskComments(List<TaskComment> comments)
        {
            if (comments.Count > 0)
            {
                Context.TaskComments.RemoveRange(comments);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a taskComment and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTaskComment(int id)
        {
            var taskComment = Context.TaskComments
                .FirstOrDefault(t => t.Id == id);

            if (taskComment != null)
            {
                Context.TaskComments.Remove(taskComment);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of steps.
        /// </summary>
        /// <param name="steps">The steps to remove.</param>
        /// <returns></returns>
        public bool DeleteSteps(List<Step> steps)
        {
            if (steps.Count > 0)
            {
                Context.Steps.RemoveRange(steps);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a step and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteStep(int id)
        {
            var step = Context.Steps
                .FirstOrDefault(t => t.Id == id);

            if (step != null)
            {
                Context.Steps.Remove(step);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete an item and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteItem(int id)
        {
            var item = Context.Items
                .Include(i => i.Comments)
                .Include(i => i.Likes)
                .FirstOrDefault(t => t.Id == id);

            if (item != null)
            {
                if (item.Comments.Count > 0)
                {
                    DeleteComments(item.Comments);
                }

                if (item.Likes.Count > 0)
                {
                    DeleteLikes(item.Likes);
                }

                Context.Items.Remove(item);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of comments.
        /// </summary>
        /// <param name="comments">The comments to remove.</param>
        /// <returns></returns>
        public bool DeleteComments(List<Comment> comments)
        {
            if (comments.Count > 0)
            {
                Context.Comments.RemoveRange(comments);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a comment and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteComment(int id)
        {
            var comment = Context.Comments
                .FirstOrDefault(t => t.Id == id);

            if (comment != null)
            {
                Context.Comments.Remove(comment);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of likes.
        /// </summary>
        /// <param name="likes">The likes to remove.</param>
        /// <returns></returns>
        public bool DeleteLikes(List<Like> likes)
        {
            if (likes.Count > 0)
            {
                Context.Likes.RemoveRange(likes);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a like and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteLike(int id)
        {
            var like = Context.Likes
                .FirstOrDefault(t => t.Id == id);

            if (like != null)
            {
                Context.Likes.Remove(like);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
