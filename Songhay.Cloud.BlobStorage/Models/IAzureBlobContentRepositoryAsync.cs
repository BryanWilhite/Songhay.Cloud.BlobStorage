using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// Defines a repository of <see cref="AzureBlobContent"/>.
    /// </summary>
    public interface IAzureBlobContentRepositoryAsync
    {
        /// <summary>
        /// Gets the BLOB.
        /// </summary>
        /// <param name="id">The identifier.</param>
        Task<AzureBlobContent> GetBlobAsync(string id);
    }
}
