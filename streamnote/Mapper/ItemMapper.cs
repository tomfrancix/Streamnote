using System.Collections.Generic;
using System.Linq;
using streamnote.Data;
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
        private readonly DateTimeHelper DateTimeHelper;

        public ItemMapper(TopicMapper topicMapper, DateTimeHelper dateTimeHelper)
        {
            TopicMapper = topicMapper;
            DateTimeHelper = dateTimeHelper;
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
            var time = (item.Created > item.Modified) ? item.Created : item.Modified;

            var timeString = DateTimeHelper.GetFriendlyDateTime(time);

            return new ItemDescriptor
            {
                Id = item.Id,
                FriendlyDateTime = timeString,
                Title = item.Title,
                Content = item.Content,
                Image = item.Image,
                ImageContentType = item.ImageContentType,
                IsPublic = item.IsPublic,
                FullName = item.User.FirstName + " " + item.User.LastName,
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
