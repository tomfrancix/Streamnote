using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streamnote.Relational.Interfaces.Services
{
    public interface IS3Service
    {

        /// <summary>
        /// Puts the object asynchronously.
        /// </summary>              
        /// <param name="contentType">Type of the content.</param>
        /// <param name="bytes">The bytes.</param> 
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        Task UploadImage(string imageName, string contentType, byte[] bytes, IDictionary<string, string> metadata);
    }
}
