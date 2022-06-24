using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        /// Delete a project and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteProject(int id)
        {
            var project = Context.Projects
                .Include(p => p.Tasks).ThenInclude(t => t.Steps)
                .Include(p => p.Tasks).ThenInclude(t => t.Comments)
                .Include(p => p.Users)
                .FirstOrDefault(p => p.Id == id);

            if (project != null)
            {
                if (project.Tasks.Count > 0)
                {
                    DeleteTasks(project.Tasks);
                    project.Tasks = null;
                }

                /*if (project.Users.Count > 0)
                {
                    project.Users.RemoveAll(u => true);
                }*/

                Context.Projects.Remove(project);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of tasks.
        /// </summary>
        /// <param name="tasks">The tasks to remove.</param>
        /// <returns></returns>
        public bool DeleteTasks(List<TaskItem> tasks)
        {
            if (tasks.Count > 0)
            {
                Context.Tasks.RemoveRange(tasks);
                Context.SaveChangesAsync();
                return true;
            }

            return false;
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
        public async Task<bool> DeleteItem(int id)
        {
            try
            {
                var item = Context.Items
                    .Include(i => i.Comments)
                    .Include(i => i.Likes)
                    .Include(i => i.Images)
                    .Include(i => i.Blocks)
                    .FirstOrDefault(t => t.Id == id);

                if (item != null)
                {
                    if (item.Comments.Count > 0)
                    {
                        item = await DeleteComments(item);
                    }

                    if (item.Likes.Count > 0)
                    {
                        item = await DeleteLikes(item);
                    }

                    if (item.Images.Count > 0)
                    {
                        item = await DeleteImages(item);
                    }

                    Context.Items.Remove(item);
                    await Context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of images.
        /// </summary>
        /// <param name="images">The images to remove.</param>
        /// <returns></returns>
        public async Task<Item> DeleteImages(Item item)
        {
            var images = item.Images;

            if (images.Count > 0)
            {
                Context.Images.RemoveRange(images);
                await Context.SaveChangesAsync();
            }

            return item;
        }

        /// <summary>
        /// Delete a collection of comments.
        /// </summary>
        /// <param name="comments">The comments to remove.</param>
        /// <returns></returns>
        public async Task<Item> DeleteComments(Item item)
        {
            var comments = item.Comments;

            if (comments.Count > 0)
            {
                Context.Comments.RemoveRange(comments);
                await Context.SaveChangesAsync();
            }

            return item;
        }

        /// <summary>
        /// Delete a comment and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteComment(int id)
        {
            var comment = Context.Comments
                .FirstOrDefault(t => t.Id == id);

            if (comment != null)
            {
                Context.Comments.Remove(comment);
                await Context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a collection of likes.
        /// </summary>
        /// <param name="likes">The likes to remove.</param>
        /// <returns></returns>
        public async Task<Item> DeleteLikes(Item item)
        {
            var likes = item.Likes;

            if (likes.Count > 0)
            {
                Context.Likes.RemoveRange(likes);
                await Context.SaveChangesAsync();
            }

            return item;
        }

        /// <summary>
        /// Delete a like and cascade delete inner entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteLike(int id)
        {
            var like = Context.Likes
                .FirstOrDefault(t => t.Id == id);

            if (like != null)
            {
                Context.Likes.Remove(like);
                await Context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
