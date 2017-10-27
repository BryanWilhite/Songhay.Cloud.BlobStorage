﻿using Microsoft.WindowsAzure.Storage.Blob;
using Songhay.Diagnostics;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="CloudBlobContainer"/>.
    /// </summary>
    public static class CloudBlobContainerExtensions
    {
        static CloudBlobContainerExtensions() => traceSource = TraceSources.Instance.GetConfiguredTraceSource().WithAllSourceLevels();
        static readonly TraceSource traceSource;

        /// <summary>
        /// Returns the <see cref="CloudBlobContainer"/>
        /// with <see cref="CloudBlobContainer.CreateIfNotExists()"/> results.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="generationAction">The generation action.</param>
        /// <returns></returns>
        public static async Task<CloudBlobContainer> WithGenerationAsync(this CloudBlobContainer container, Action<CloudBlobContainer, bool> generationAction = null)
        {
            if (container == null) return null;
            var wasGenerated = await container.CreateIfNotExistsAsync();
            if (wasGenerated) traceSource.TraceVerbose(string.Format("Generated container {0}.", container.Name));
            if (generationAction != null) generationAction.Invoke(container, wasGenerated);
            return container;
        }

        /// <summary>
        /// Gets the path relative to the BLOB container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="blobUri">The BLOB URI.</param>
        /// <returns></returns>
        public static string GetRelativePath(this CloudBlobContainer container, Uri blobUri)
        {
            if (container == null) return null;
            if (blobUri == null) return null;

            var containerName = container.Name;
            var subFolders = blobUri.Segments.Where(i => (i != "/") && i != string.Concat(containerName, "/"));
            return string.Join(string.Empty, subFolders);
        }

        /// <summary>
        /// Gets the cloud BLOB or default.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static CloudBlob GetCloudBlobOrDefault(this CloudBlobContainer container, IListBlobItem item)
        {
            if (container == null) return default(CloudBlob);
            if (item == null) return default(CloudBlob);

            var blobName = (string)item.GetPropertyValue("Name");
            if (string.IsNullOrEmpty(blobName)) return default(CloudBlob);
            traceSource.TraceVerbose("Getting reference to blob `{0}`…", blobName);
            var blob = container.GetBlobReference(blobName);
            return blob;
        }

        /// <summary>
        /// Gets the cloud block BLOB or default.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="item">The item.</param>
        public static CloudBlockBlob GetCloudBlockBlobOrDefault(this CloudBlobContainer container, IListBlobItem item)
        {
            if (container == null) return default(CloudBlockBlob);
            if (item == null) return default(CloudBlockBlob);

            var blobName = (string)item.GetPropertyValue("Name");
            if (string.IsNullOrEmpty(blobName)) return default(CloudBlockBlob);
            traceSource.TraceVerbose("Getting reference to block-blob `{0}`…", blobName);
            var blob = container.GetBlockBlobReference(blobName);
            return blob;
        }

        /// <summary>
        /// Lists up to 25,000 items
        /// in the <see cref="CloudBlobContainer"/>.
        /// </summary>
        /// <param name="container">the container</param>
        public static async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer container)
        {
            return await container.ListBlobsAsync(pageLimit: 5);
        }

        /// <summary>
        /// Lists 5,000 items by default
        /// in the <see cref="CloudBlobContainer"/>.
        /// </summary>
        /// <param name="container">the container</param>
        /// <param name="pageLimit">limits items to a multiple of 5000</param>
        public static async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer container, int pageLimit)
        {
            if (container == null) return Enumerable.Empty<IListBlobItem>();

            var continuationToken = default(BlobContinuationToken);
            var pageNumber = 0;
            if (pageLimit < 1) pageLimit = 1;
            var results = new List<IListBlobItem>();

            do
            {
                var response = await container.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
                ++pageNumber;
            } while ((continuationToken != null) && (pageNumber < pageLimit));

            return results;
        }

        /// <summary>
        /// Uploads the BLOB.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="blobContainerPath">The BLOB container path.</param>
        /// <exception cref="System.NullReferenceException">The expected cloud container is not here.</exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static async Task UploadBlob(this CloudBlobContainer container, string localFile, string blobContainerPath)
        {
            if (container == null) throw new NullReferenceException("The expected cloud container is not here.");

            if (!File.Exists(localFile)) throw new FileNotFoundException(string.Format("The expected local file, “{0},” is not here.", localFile));
            if (string.IsNullOrEmpty(blobContainerPath)) blobContainerPath = string.Empty;

            var fileInfo = new FileInfo(localFile);
            if (await container.CreateIfNotExistsAsync()) traceSource.TraceVerbose(string.Format("Generated container {0}.", container.Name));

            var @ref = Path.Combine(blobContainerPath, fileInfo.Name)
                .Replace("\\", "/")
                .TrimStart('/');
            traceSource.TraceVerbose("Getting block-blob reference {0}…", @ref);
            var blob = container.GetBlockBlobReference(@ref);
            blob.Properties.ContentType = AzureStorageUtility.GetMimeType(fileInfo.Extension);
            traceSource.TraceVerbose("Uploading {0}…", localFile);
            await blob.UploadFromFileAsync(localFile);
        }
    }
}
