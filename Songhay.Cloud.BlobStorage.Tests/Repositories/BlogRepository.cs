using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Cloud.BlobStorage.Repositories;
using Songhay.Cloud.BlobStorage.Tests.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Tests.Repositories
{
    /// <summary>
    /// Defines a JSON blob Repository for Azure Storage
    /// with JSON index support.
    /// </summary>
    public class BlogRepository : AzureBlobRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogRepository"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="container">The container.</param>
        public BlogRepository(AzureBlobKeys keys, CloudBlobContainer container)
            : base(keys, container)
        {
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <exception cref="System.NullReferenceException">The expected repository container is not here.</exception>
        public async Task<IEnumerable<BlogEntry>> GetIndexAsync()
        {
            if (this.GetCloudBlobContainer() == null) throw new NullReferenceException("The expected repository container is not here.");

            var reference = this.GetIndexReference();
            await this.CheckReferenceAsync(reference, indexKey);

            var json = await reference.DownloadTextAsync();
            var entity = JsonConvert.DeserializeObject<IEnumerable<BlogEntry>>(json);
            return entity;
        }

        /// <summary>
        /// Sets the index.
        /// </summary>
        /// <param name="json">The json.</param>
        public async Task SetIndexAsync(string json)
        {
            var reference = this.GetIndexReference();
            reference.Properties.ContentType = "application/json";
            await reference.UploadTextAsync(json);
        }
    }
}
