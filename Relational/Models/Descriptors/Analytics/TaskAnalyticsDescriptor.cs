namespace Streamnote.Relational.Models.Descriptors.Analytics
{
    public class TaskAnalyticsDescriptor
    {
        public int Count { get; set; }   
        public int AcceptedCount { get; set; }
        public int DeliveredCount { get; set; }
        public int FinishedCount { get; set; }
        public int StartedCount { get; set; }
        public int UnstartedCount { get; set; }
        public string Date { get; set; }
    }
}
