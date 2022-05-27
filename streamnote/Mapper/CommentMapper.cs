using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// The mapper for comments.
    /// </summary>
    public class CommentMapper
    {
        /// <summary>
        /// Map a list of comments.
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CommentDescriptor> MapDescriptors(List<Comment> comments, string userId)
        {
            var commentDescriptors = new List<CommentDescriptor>();

            foreach (var comment in comments)
            {
                commentDescriptors.Add(MapDescriptor(comment, userId));
            }

            return commentDescriptors;
        }
        
        /// <summary>
        /// Map a single comment.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CommentDescriptor MapDescriptor(Comment comment, string userId)
        {
            return new CommentDescriptor
            {
                Id = comment.Id,
                Created = comment.Created,
                Modified = comment.Modified,
                Content = comment.Content,
                Image = comment.Image,
                ImageContentType = comment.ImageContentType,
                UserName = comment.User.UserName,
                UserImageContentType = comment.User.ImageContentType,
                UserImage = comment.User.Image,
                CreatedByLoggedInUser = comment.User.Id == userId
            };
        }
    }
}
