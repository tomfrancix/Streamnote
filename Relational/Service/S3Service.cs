using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Streamnote.Relational.Interfaces.Services;

namespace Streamnote.Relational.Service
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 Client;
        private readonly IConfiguration Configuration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration"></param>
        public S3Service(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Puts the object asynchronously.
        /// </summary>              
        /// <param name="contentType">Type of the content.</param>
        /// <param name="bytes">The bytes.</param> 
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        public async Task UploadImage(string imageName, string contentType, byte[] bytes, IDictionary<string, string> metadata)
        {
            var awsConfiguration = Configuration.GetSection("AWS").GetChildren().ToList();

            var accessKey = Configuration.GetSection("AWS:AWSAccessKey")?.Value;
            var secretKey = Configuration.GetSection("AWS:AWSSecretKey")?.Value;
            RegionEndpoint region = RegionEndpoint.EUWest1;
            var bucketName = Configuration.GetSection("AWS:AWSS3BucketName")?.Value;

            var s3Client = new AmazonS3Client(accessKey, secretKey, region);

            
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead,
                Key = string.Format("Images/{0}", imageName)
            };

            using (var ms = new MemoryStream(bytes))
            {
                request.InputStream = ms;
                await s3Client.PutObjectAsync(request);
            } 
        }
    }
}
