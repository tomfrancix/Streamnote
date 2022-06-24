namespace Streamnote.Relational.Models.Descriptors
{
    /// <summary>
    /// The post block descriptor class.
    /// </summary>
    public class PostBlockDescriptor
    {
        public virtual int Id { get; set; }

        public virtual BlockType Type { get; set; }

        public virtual string Text { get; set; }

        public virtual string FullS3Location { get; set; }
        public virtual byte[] Bytes { get; set; }
        public virtual string ImageContentType { get; set; }
        public string OutputContainerIdentifier { get; set; }
        public string OutputTextIdentifier { get; set; }
        public string EditorIdentifier { get; set; }
        public string MainEditorIdentifier { get; set; }
        public string EditorOutputIdentifier { get; set; }
        public string OpenBlockEditorButtonIdentifier { get; set; }
    }
}
