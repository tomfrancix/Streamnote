using System.Collections.Generic;
using System.Linq;
using Streamnote.Relational.Data;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// Mapper for items.
    /// </summary>
    public class ItemMapper
    {
        private readonly TopicMapper TopicMapper;
        private readonly DateTimeHelper DateTimeHelper;
        private readonly BlockMapper BlockMapper;

        public ItemMapper(TopicMapper topicMapper, DateTimeHelper dateTimeHelper, BlockMapper blockMapper)
        {
            TopicMapper = topicMapper;
            DateTimeHelper = dateTimeHelper;
            BlockMapper = blockMapper;
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
                Image = item.Images is { Count: > 0 } ? item.Images[0].Bytes : null,
                ImageContentType = item.Images is { Count: > 0 } ? item.Images[0].ImageContentType : null,
                IsPublic = item.IsPublic,
                FullName = item.User.FirstName + " " + item.User.LastName,
                UserName = item.User.UserName,
                UserImageContentType = item.User.ImageContentType,
                UserImage = item.User.Image ,
                CreatedByLoggedInUser = item.User.Id == userId,
                CommentCount = item.CommentCount,
                LikeCount = item.LikeCount,
                LoggedInUserLikesThis = item.Likes.Any(l => l.User.Id == userId),
                Topics = TopicMapper.MapDescriptors(item.Topics, userId),
                ImageLocation = item.Images is { Count: > 0 } ? item.Images[^1].FullS3Location : null,
                Blocks = BlockMapper.MapDescriptors(item.Blocks)
            };
        }
    }
}
