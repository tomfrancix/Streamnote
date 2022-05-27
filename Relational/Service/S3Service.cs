using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
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
        /// <param name="client"></param>
        public S3Service(IConfiguration configuration)
        {
            Configuration = configuration;
            AWSConfigs options = Configuration["AWS"];
            options.Credentials = new EnvironmentVariablesAWSCredentials();
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();
            IAmazonS3 Client = options.CreateServiceClient<IAmazonS3>();
        }

        /// <summary>
        /// Lists all the objects in the given bucket with their key starting with the given prefix.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="bucketName">Name of the bucket (e.g. "bucket-name").</param>
        /// <param name="prefix">The prefix (e.g. "/folder/x").</param>
        /// <returns></returns>
        public async Task<IEnumerable<S3Object>> ListAllObjectsAsync(IAmazonS3 client, string bucketName, string prefix)
        {
            var objects = new List<S3Object>();

            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = prefix,
                MaxKeys = 1000
            };

            do
            {
                var response = await client.ListObjectsAsync(request);

                // Process response.
                objects.AddRange(response.S3Objects);

                // If response is truncated, set the marker to get the next set of keys.
                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);

            return objects;
        }

        /// <summary>
        /// Puts the object asynchronously.
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="path">The path.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="acl">The acl.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        public async Task PutObjectAsync(string bucketName, string path, string contentType, byte[] bytes, IDictionary<string, string> metadata)
        {
            if (string.IsNullOrEmpty(bucketName)) return;

            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(contentType) && bytes != null)
            {
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = path,
                    ContentType = contentType
                };

                putObjectRequest.Metadata.Add("Content-Length", bytes.Length.ToString());

                if (metadata != null)
                {
                    foreach (var meta in metadata)
                    {
                        putObjectRequest.Metadata.Add(meta.Key, meta.Value);
                    }
                }

                using (var stream = new MemoryStream(bytes))
                {
                    putObjectRequest.InputStream = stream;

                    await Client.PutObjectAsync(putObjectRequest);
                }
            }
        }

        /// <summary>
        /// Checks whether a file exists (does not download the whole file, only tries to get its metadata) asynchronously.
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task<bool> ObjectExistsAsync(string bucketName, string path)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(path)) return false;

            var result = false;

            try
            {
                await GetObjectMetadataAsync(bucketName, path);

                result = true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.Message.Contains("Key Not Found"))
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the object metadata (does not download the file content).
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string bucketName, string path)
        {
            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(path)) return null;

            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = path
            };

            var response = await Client.GetObjectMetadataAsync(request);

            return response;
        }
    }
}
