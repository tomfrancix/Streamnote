using System.Collections.Generic;
using System.Linq;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// The mapper for topics.
    /// </summary>
    public class TopicMapper
    {
        /// <summary>
        /// Map a list of topics.
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<TopicDescriptor> MapDescriptors(List<Topic> topics, string userId)
        {
            var topicDescriptors = new List<TopicDescriptor>();

            foreach (var topic in topics)
            {
                topicDescriptors.Add(MapDescriptor(topic, userId));
            }

            return topicDescriptors;
        }
        
        /// <summary>
        /// Map a single topic.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TopicDescriptor MapDescriptor(Topic topic, string userId)
        {
            return new TopicDescriptor
            {
                Id = topic.Id,
                Name = topic.Name,
                ItemCount = topic.ItemCount,
                UserFollowsTopic = topic.Users.Any(u => u.Id == userId),
                TopicElementId = "topic" + topic.Id
            };
        }
    }
}
