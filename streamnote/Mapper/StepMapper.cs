using System.Collections.Generic;
using Streamnote.Relational.Helpers;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// Mapper for steps.
    /// </summary>
    public class StepMapper
    {
        private readonly UserMapper UserMapper;
        private readonly DateTimeHelper DateTimeHelper;

        public StepMapper(DateTimeHelper dateTimeHelper, UserMapper userMapper)
        {
            DateTimeHelper = dateTimeHelper;
            UserMapper = userMapper;
        }

        /// <summary>
        /// Map a list of steps.
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<StepDescriptor> MapDescriptors(List<Step> steps, string userId)
        {
            var stepDescriptors = new List<StepDescriptor>();

            foreach (var step in steps)
            {
                stepDescriptors.Add(MapDescriptor(step, userId));
            }

            return stepDescriptors;
        }

        /// <summary>
        /// Map a single step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public StepDescriptor MapDescriptor(Step step, string userId)
        {
            var time = (step.Created > step.Modified) ? step.Created : step.Modified;

            var timeString = DateTimeHelper.GetFriendlyDateTime(time);

            return new StepDescriptor
            {
                Id = step.Id,
                Created = step.Created,
                Modified = step.Modified,
                Content = step.Content,
                IsCompleted = step.IsCompleted,
                StepIdentifier = "stepIdentifier" + step.Id
            };
        }
    }
}
