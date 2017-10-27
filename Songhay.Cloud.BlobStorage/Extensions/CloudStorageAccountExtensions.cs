using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Songhay.Diagnostics;
using Songhay.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="CloudStorageAccount"/>.
    /// </summary>
    public static class CloudStorageAccountExtensions
    {
        static CloudStorageAccountExtensions() => traceSource = TraceSources.Instance.GetConfiguredTraceSource().WithAllSourceLevels();
        static readonly TraceSource traceSource;

        /// <summary>
        /// Downloads the BLOB.
        /// </summary>
        /// <param name="cloudStorageAccount">The cloud storage account.</param>
        /// <param name="localRoot">The local root.</param>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        /// <param name="blobContainerPath">The BLOB container path.</param>
        public static async Task DownloadBlobAsync(this CloudStorageAccount cloudStorageAccount, string localRoot, string blobContainerName, string blobContainerPath)
        {
            var container = await cloudStorageAccount
                .GetContainerReference(blobContainerName)
                .WithGenerationAsync();

            if (container == null) return;

            if (string.IsNullOrEmpty(blobContainerPath)) blobContainerPath = string.Empty;

            var blob = await container.GetBlobReferenceFromServerAsync(blobContainerPath);
            await blob.DownloadBlobAsync(Path.Combine(localRoot, blobContainerName));
        }

        /// <summary>
        /// Gets the container reference.
        /// </summary>
        /// <param name="cloudStorageAccount">The cloud storage account.</param>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        /// <returns></returns>
        public static CloudBlobContainer GetContainerReference(this CloudStorageAccount cloudStorageAccount, string blobContainerName)
        {
            if (cloudStorageAccount == null)
            {
                traceSource.TraceError("The expected cloud storage account is not here.");
                return null;
            }

            var container = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(blobContainerName);
            return container;
        }

        /// <summary>
        /// Determines whether BLOB in container at the specified BLOB location.
        /// </summary>
        /// <param name="cloudStorageAccount">The cloud storage account.</param>
        /// <param name="blobName">The BLOB location.</param>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        /// <returns>
        ///   <c>true</c> if BLOB in container at the specified BLOB location; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> IsBlobInContainer(this CloudStorageAccount cloudStorageAccount, string blobName, string blobContainerName)
        {
            var container = cloudStorageAccount.GetContainerReference(blobContainerName);
            if (container == null) return false;

            var test = false;

            traceSource.TraceVerbose(string.Format("Looking for {0} in {1}…", blobName, blobContainerName));
            try
            {
                var blob = await container.GetBlobReferenceFromServerAsync(blobName);
                await blob.FetchAttributesAsync();
                test = true;
            }
            catch(Exception ex)
            {
                traceSource.TraceError(ex);
            }

            return test;
        }

        /// <summary>
        /// Uploads the BLOB.
        /// </summary>
        /// <param name="cloudStorageAccount">The cloud storage account.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="blobContainerName">Name of the BLOB container.</param>
        /// <param name="blobContainerPath">The BLOB container path.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static async Task UploadBlobAsync(this CloudStorageAccount cloudStorageAccount, string localFile, string blobContainerName, string blobContainerPath)
        {
            var container = cloudStorageAccount.GetContainerReference(blobContainerName);
            if (container == null) return;

            await container.UploadBlob(localFile, blobContainerPath);
        }
    }
}
