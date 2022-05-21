using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using streamnote.Models;
using streamnote.Models.Descriptors;

namespace streamnote.Mapper
{
    public class CommentMapper
    {
        public List<CommentDescriptor> MapDescriptors(List<Comment> comments, string userId)
        {
            var commentDescriptors = new List<CommentDescriptor>();

            foreach (var comment in comments)
            {
                commentDescriptors.Add(MapDescriptor(comment, userId));
            }

            return commentDescriptors;
        }

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
