using System.Collections.Generic;
using System.Linq;
using streamnote.Models;
using streamnote.Models.Descriptors;

namespace streamnote.Mapper
{
    /// <summary>
    /// Mapper for items.
    /// </summary>
    public class ItemMapper
    {
        private readonly TopicMapper TopicMapper;

        public ItemMapper(TopicMapper topicMapper)
        {
            TopicMapper = topicMapper;
        }

        /// <summary>
        /// Map a list of items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ItemDescriptor> MapDescriptors(List<Item> items, string userId)
        {
            var itemDescriptors = new List<ItemDescriptor>();

            foreach (var item in items)
            {
                itemDescriptors.Add(MapDescriptor(item, userId));
            }

            return itemDescriptors;
        }

        /// <summary>
        /// Map a single item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ItemDescriptor MapDescriptor(Item item, string userId)
        {
            return new ItemDescriptor
            {
                Id = item.Id,
                Created = item.Created,
                Modified = item.Modified,
                Title = item.Title,
                Content = item.Content,
                Image = item.Image,
                ImageContentType = item.ImageContentType,
                IsPublic = item.IsPublic,
                UserName = item.User.UserName,
                UserImageContentType = item.User.ImageContentType,
                UserImage = item.User.Image ,
                CreatedByLoggedInUser = item.User.Id == userId,
                CommentCount = item.CommentCount,
                LikeCount = item.LikeCount,
                LoggedInUserLikesThis = item.Likes.Any(l => l.User.Id == userId),
                Topics = TopicMapper.MapDescriptors(item.Topics, userId)
            };
        }
    }
}
