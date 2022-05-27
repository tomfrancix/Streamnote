using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Streamnote.Relational.Interfaces.Services
{
    public interface IS3Service

    {
        /// <summary>
        /// Lists all the objects in the given bucket with their key starting with the given prefix.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="bucketName">Name of the bucket (e.g. "bucket-name").</param>
        /// <param name="prefix">The prefix (e.g. "/folder/x").</param>
        /// <returns></returns>
        Task<IEnumerable<S3Object>> ListAllObjectsAsync(IAmazonS3 client, string bucketName,
            string prefix);

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
        Task PutObjectAsync(string bucketName, string path, string contentType, byte[] bytes, IDictionary<string, string> metadata);

        /// <summary>
        /// Checks whether a file exists (does not download the whole file, only tries to get its metadata) asynchronously.
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<bool> ObjectExistsAsync(string bucketName, string path);

        /// <summary>
        /// Gets the object metadata (does not download the file content).
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<GetObjectMetadataResponse> GetObjectMetadataAsync(string bucketName, string path);
    }
}
