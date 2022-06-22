using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;

namespace Streamnote.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChooseCommand();
        }

        public static string ACCESS_KEY = "";
        public static string SECRET_KEY = "";
        public static string BUCKET_NAME = "sn-content";


        public static void ChooseCommand()
        {
            System.Console.Write("Enter a command: (compress-images)");

            var command = System.Console.ReadLine();

            if (command != null)
            {
                switch (command)
                {
                    case "compress-images":
                        CompressImages();
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        System.Console.Write("That is not a command!");
                        break;
                }
            }  
            
            ChooseCommand();
        }

        public static async Task CompressImages()
        {
            if (ACCESS_KEY.Length < 10)
            {

                System.Console.Write("Enter your AWS access key:");
                ACCESS_KEY = System.Console.ReadLine();
            }

            if (SECRET_KEY.Length < 10)
            {
                System.Console.Write("Enter your AWS secret key:");
                SECRET_KEY = System.Console.ReadLine();
            }

            RegionEndpoint region = RegionEndpoint.EUWest1;   

            var s3Client = new AmazonS3Client(ACCESS_KEY, SECRET_KEY, region);

            var dir = new S3DirectoryInfo(s3Client, BUCKET_NAME, "Images");

            foreach (IS3FileSystemInfo file in dir.GetFiles())
            {
                System.Console.WriteLine(file.Name);
                System.Console.WriteLine(file.Extension);
                System.Console.WriteLine(file.LastWriteTime);

                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = BUCKET_NAME,
                    Key = "Images/" + file.Name
                };

                var response = s3Client.GetObject(getObjectRequest);

                var resizedImage = ResizeImage(response.ResponseStream, 600);

                var deleteObjectRequest = new DeleteObjectRequest()
                {
                    BucketName = BUCKET_NAME, 
                    Key = "Images/" + file.Name
                };

                var deleteObjectResponse = s3Client.DeleteObject(deleteObjectRequest);

                var request = new PutObjectRequest
                {
                    BucketName = BUCKET_NAME,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = "Images/" + file.Name
                };

                using (var ms = new MemoryStream(resizedImage))
                {
                    request.InputStream = ms;
                    await s3Client.PutObjectAsync(request);
                }
            } 

            System.Console.WriteLine("All images have been compressed.");
            await Task.Delay(2 * 1000);
            await CompressImages();
        }

        public static byte[] ResizeImage(Stream data, int width)
        {
            using (var stream = data)
            {
                var image = Image.FromStream(stream);

                var height = (width * image.Height) / image.Width;
                var resizedImage = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                using (var resizeStream = new MemoryStream())
                {
                    resizedImage.Save(resizeStream, ImageFormat.Png);
                    return resizeStream.ToArray();
                }
            };
        }
        
    }
}
