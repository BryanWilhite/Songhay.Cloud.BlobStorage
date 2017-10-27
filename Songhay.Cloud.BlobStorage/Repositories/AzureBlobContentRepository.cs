using Microsoft.WindowsAzure.Storage.Blob;
using Songhay.Cloud.BlobStorage.Models;
using System.IO;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Repositories
{
    /// <summary>
    /// Defines a repository of <see cref="AzureBlobContent"/>.
    /// </summary>
    public class AzureBlobContentRepository : IAzureBlobContentRepositoryAsync
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobContentRepository"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public AzureBlobContentRepository(CloudBlobContainer container)
        {
            this._container = container;
        }

        /// <summary>
        /// Gets the BLOB.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public async Task<AzureBlobContent> GetBlobAsync(string id)
        {
            var blob = this._container.GetBlobReference(id);
            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms);

            var download = new AzureBlobContent
            {
                BlobStream = ms,
                BlobFileName = Path.GetFileName(blob.Name),
                BlobLength = blob.Properties.Length,
                BlobContentType = blob.Properties.ContentType
            };

            return download;
        }

        readonly CloudBlobContainer _container;
    }
}
