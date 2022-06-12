namespace Streamnote.Relational.Models.Descriptors.Analytics
{
    public class ChartDescriptor
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string ChartIdentifier { get; set; }
        public string[] XValues { get; set; }
        public ChartDataset[] Datasets { get; set; }
        public string[] Colors { get; set; }
    }

    public class ChartDataset
    {
       public string Label { get; set; }
       public string[] BackgroundColor { get; set; }
       public string BorderColor { get; set; }
       public int[] Data { get; set; }
       public bool Fill { get; set; }
    }
}
