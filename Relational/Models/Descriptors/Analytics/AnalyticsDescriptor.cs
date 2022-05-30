using System.Collections.Generic;

namespace Streamnote.Relational.Models.Descriptors.Analytics
{
    public class AnalyticsDescriptor
    {
        public TaskAnalyticsDescriptor TotalTaskAnalytics { get; set; }
        public List<TaskAnalyticsDescriptor> TaskAnalyticsOverTime { get; set; }
    }
}
