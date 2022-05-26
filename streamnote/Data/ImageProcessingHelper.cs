using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using SkiaSharp;

namespace streamnote.Data
{
    /// <summary>
    /// Helper for image processing.
    /// </summary>
    public class ImageProcessingHelper
    {
        /// <summary>
        /// Resize an image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public static byte[] ResizeImageFile(IFormFile image, int targetSize) // Set targetSize to 1024
        {
            Console.WriteLine("Resizing image");

            byte[] bytes;

            using (var input = File.OpenRead(image.FileName))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        int width, height;
                        if (original.Width > original.Height)
                        {
                            width = targetSize;
                            height = original.Height * targetSize / original.Width;
                        }
                        else
                        {
                            width = original.Width * targetSize / original.Height;
                            height = targetSize;
                        }

                        var skImageInfo = new SKImageInfo(width, height);

                        using (var resized = original.Resize(skImageInfo, SKFilterQuality.Medium))
                        {
                            bytes = resized.Bytes;
                        }
                    }
                }
            }

            return bytes;
        }

        /// <summary>
        /// Calculate the new dimensions for an image.
        /// </summary>
        /// <param name="oldSize"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        public static Size CalculateDimensions(Size oldSize, int targetSize)
        {
            Size newSize = new Size();
            if (oldSize.Height > oldSize.Width)
            {
                newSize.Width = (int)(oldSize.Width * ((float)targetSize / (float)oldSize.Height));
                newSize.Height = targetSize;
            }
            else
            {
                newSize.Width = targetSize;
                newSize.Height = (int)(oldSize.Height * ((float)targetSize / (float)oldSize.Width));
            }
            return newSize;
        }
    }
}
