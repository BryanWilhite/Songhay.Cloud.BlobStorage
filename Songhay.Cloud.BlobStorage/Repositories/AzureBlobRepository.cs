using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Extensions;
using Songhay.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Repositories
{
    /// <summary>
    /// Defines a JSON blob Repository for Azure Storage
    /// </summary>
    public class AzureBlobRepository : IRepositoryAsync
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobRepository"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="container">The container.</param>
        /// <param name="options">The options.</param>
        public AzureBlobRepository(AzureBlobKeys keys, CloudBlobContainer container)
        {
            this._keys = keys;
            this._container = container;
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        public async Task DeleteEntityAsync<TEntity>(object key) where TEntity : class, new()
        {
            var blob = this.GetBlockBlobReference<TEntity>(key);
            await blob.DeleteAsync();
        }

        /// <summary>
        /// Determines whether the specified key has entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        public async Task<bool> HasEntityAsync<TEntity>(object key) where TEntity : class, new()
        {
            var reference = this.GetBlockBlobReference<TEntity>(key);
            return await reference.ExistsAsync();
        }

        /// <summary>
        /// Loads all.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public async Task<IEnumerable<TEntity>> LoadAllAsync<TEntity>() where TEntity : class, new()
        {
            var list = new List<TEntity>();
            var directory = this._container.GetDirectoryReference(typeof(TEntity).Name);
            var blobs = await directory.Container.ListBlobsAsync(useFlatBlobListing: true);

            blobs.ForEachInEnumerable(async i =>
            {
                var name = (string)i.GetPropertyValue("Name");
                if (string.IsNullOrEmpty(name)) return;

                var reference = this._container.GetBlockBlobReference(name);
                await this.CheckReferenceAsync(reference, i.Uri);
                var entity = await this.GetEntityAsync<TEntity>(reference);
                list.Add(entity);
            });

            return list;
        }

        /// <summary>
        /// Loads the single.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        public async Task<TEntity> LoadSingleAsync<TEntity>(object key) where TEntity : class, new()
        {
            var reference = this.GetBlockBlobReference<TEntity>(key);
            await this.CheckReferenceAsync(reference, key);
            var entity = await this.GetEntityAsync<TEntity>(reference);
            return entity;
        }

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="item">The item.</param>
        public async Task SaveEntityAsync<TEntity>(TEntity item) where TEntity : class, new()
        {
            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var key = this._keys.GetKeyValue(item).ToString();
            var reference = this.GetBlockBlobReference<TEntity>(key);
            reference.Properties.ContentType = "application/json";
            await reference.UploadTextAsync(json);
        }

        /// <summary>
        /// Checks the reference.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="key">The key.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        protected async Task CheckReferenceAsync(CloudBlockBlob blob, object key)
        {
            var blobExists = await blob.ExistsAsync();
            if (!blobExists)
            {
                var message = string.Format("The item “{0}” could not be found. Container: {1} {2}", key, this._container.Name, this._container.Uri);
                throw new FileNotFoundException(message);
            }
        }

        /// <summary>
        /// Gets the azure BLOB keys.
        /// </summary>
        /// <returns></returns>
        protected AzureBlobKeys GetAzureBlobKeys()
        {
            return this._keys;
        }

        /// <summary>
        /// Gets the BLOB path.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected string GetBlobPath<TEntity>(string key)
        {
            return string.Format("{0}/{1}.json", typeof(TEntity).Name, key);
        }

        /// <summary>
        /// Gets the block BLOB reference.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected CloudBlockBlob GetBlockBlobReference<TEntity>(object key) where TEntity : class, new()
        {
            var path = this.GetBlobPath<TEntity>((string)key);
            var reference = this._container.GetBlockBlobReference(path);
            return reference;
        }

        /// <summary>
        /// Gets the cloud BLOB container.
        /// </summary>
        /// <returns></returns>
        protected CloudBlobContainer GetCloudBlobContainer()
        {
            return this._container;
        }

        /// <summary>
        /// Gets the index reference.
        /// </summary>
        /// <returns></returns>
        protected CloudBlockBlob GetIndexReference()
        {
            var reference = this._container.GetBlockBlobReference(indexKey);
            return reference;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="reference">The reference.</param>
        protected async Task<TEntity> GetEntityAsync<TEntity>(CloudBlockBlob reference) where TEntity : class, new()
        {
            var json = await reference.DownloadTextAsync();
            var entity = JsonConvert.DeserializeObject<TEntity>(json);
            return entity;
        }

        protected const string indexKey = "index.json";
        readonly AzureBlobKeys _keys;
        readonly CloudBlobContainer _container;
    }
}
