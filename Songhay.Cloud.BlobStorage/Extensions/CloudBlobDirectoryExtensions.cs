using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="CloudBlobDirectory"/>
    /// </summary>
    public static class CloudBlobDirectoryExtensions
    {
        /// <summary>
        /// Lists up to 25,000 items
        /// in the <see cref="CloudBlobDirectory"/>.
        /// </summary>
        /// <param name="directory">the directory</param>
        /// <remarks>
        /// The <c>useFlatBlobListing</c> parameter in the overloads defaults to <c>true</c>.
        /// </remarks>
        public static async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(this CloudBlobDirectory directory)
        {
            return await directory.ListBlobsAsync(useFlatBlobListing: true, pageLimit: 5);
        }

        /// <summary>
        /// Lists 5,000 items by default
        /// in the <see cref="CloudBlobDirectory" />.
        /// </summary>
        /// <param name="directory">the directory</param>
        /// <param name="useFlatBlobListing">if set to <c>true</c> use flat BLOB listing.</param>
        public static async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(this CloudBlobDirectory directory, bool useFlatBlobListing)
        {
            return await directory.ListBlobsAsync(useFlatBlobListing, pageLimit: 5);
        }

        /// <summary>
        /// Lists 5,000 items by default
        /// in the <see cref="CloudBlobDirectory" />.
        /// </summary>
        /// <param name="directory">the directory</param>
        /// <param name="useFlatBlobListing">if set to <c>true</c> use flat BLOB listing.</param>
        /// <param name="pageLimit">limits items to a multiple of 5000</param>
        /// <returns></returns>
        public static async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(this CloudBlobDirectory directory, bool useFlatBlobListing, int pageLimit)
        {
            if (directory == null) return Enumerable.Empty<IListBlobItem>();

            var continuationToken = default(BlobContinuationToken);
            var pageNumber = 0;
            if (pageLimit < 1) pageLimit = 1;
            var results = new List<IListBlobItem>();

            var blobListingDetails = BlobListingDetails.All;
            var maxResults = default(int?);
            var options = default(BlobRequestOptions);
            var operationContext = default(OperationContext);

            do
            {
                var response = await directory.ListBlobsSegmentedAsync(
                    useFlatBlobListing: useFlatBlobListing,
                    blobListingDetails: blobListingDetails,
                    maxResults: maxResults,
                    currentToken: continuationToken,
                    options: options,
                    operationContext: operationContext);

                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
                ++pageNumber;
            } while ((continuationToken != null) && (pageNumber < pageLimit));

            return results;
        }
    }
}
