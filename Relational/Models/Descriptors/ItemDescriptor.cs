using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace Streamnote.Relational.Models.Descriptors
{
    public class ItemDescriptor
    {
        public virtual int Id { get; set; }
        public virtual string FriendlyDateTime { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual bool CreatedByLoggedInUser { get; set; }
        public virtual bool LoggedInUserLikesThis { get; set; }
        public virtual int CommentCount { get; set; }
        public virtual int ShareCount { get; set; }
        public virtual int LikeCount { get; set; }
        public virtual List<TopicDescriptor> Topics { get; set; }
        public bool IsDetails { get; set; }


        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserImageContentType { get; set; }
        public byte[] UserImage { get; set; }
        public string ImageLocation { get; set; }

        public List<CommentDescriptor> Comments { get; set; }
        public List<ItemImage> Images { get; set; }
        public List<PostBlockDescriptor> Blocks { get; set; }
    }
}
