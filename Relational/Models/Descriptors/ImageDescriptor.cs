using System;
using System.Collections.Generic;

namespace Streamnote.Relational.Models.Descriptors
{
    public class ImageDescriptor
    {
        public string UserImageContentType { get; set; }
        public byte[] UserImage { get; set; }
    }
}
