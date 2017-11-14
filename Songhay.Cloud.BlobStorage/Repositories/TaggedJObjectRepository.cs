using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Diagnostics;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Repositories
{
    /// <summary>
    /// Defines a JSON blob Repository for Azure Storage
    /// </summary>
    public class TaggedJObjectRepository : ITaggedJObjectRepositoryAsync
    {
        static TaggedJObjectRepository() => traceSource = TraceSources.Instance.GetConfiguredTraceSource().WithAllSourceLevels();
        static readonly TraceSource traceSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedJObjectRepository"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="tagPropertyName">Name of the tag property.</param>
        public TaggedJObjectRepository(CloudBlobContainer container, string tagPropertyName = "Tag")
        {
            traceSource.TraceVerbose("Constructing TaggedJObjectRepository...");
            if (container != null) traceSource.TraceVerbose("container.Name: {0}", container.Name);
            traceSource.TraceVerbose("tagPropertyName: {0}", tagPropertyName);

            this._container = container;
            this._tagPropertyName = tagPropertyName;
        }

        /// <summary>
        /// Deletes the specified repository key.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        public async Task DeleteAsync(string repositoryKey)
        {
            traceSource.TraceVerbose("Deleting {0}...", repositoryKey);

            var reference = this.GetBlockBlobReference(repositoryKey);
            await reference.DeleteAsync();
        }

        /// <summary>
        /// Determines whether the specified repository key has BLOB.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        /// <returns></returns>
        public async Task<bool> HasBlobAsync(string repositoryKey)
        {
            traceSource.TraceVerbose("Verifying BLOB {0}...", repositoryKey);

            var reference = this.GetBlockBlobReference(repositoryKey);
            return await reference.ExistsAsync();
        }

        /// <summary>
        /// Loads the set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task<JObject> LoadSetAsync(Func<Uri, bool> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter", "The expected Azure Blob set filter is not here.");
            traceSource.TraceVerbose("Loading set...");

            var jO = JObject.Parse("{\"set\":[]}");
            var list = new List<TaggedJObject>();
            var items = await this._container.ListBlobsAsync(useFlatBlobListing: true);
            var tasks = items.Select(async i =>
            {
                if (!filter(i.Uri)) return;
                var path = this._container.GetRelativePath(i.Uri);
                traceSource.TraceVerbose("Getting Container reference to `{0}`...", path);
                var reference = this._container.GetBlockBlobReference(path);
                await this.CheckReferenceAsync(reference, i.Uri);
                var o = await this.GetTaggedJObjectAsync(reference);
                traceSource.TraceVerbose("Adding reference `{0}`...", reference.Name);
                list.Add(o);
            });
            await Task.WhenAll(tasks);

            jO["set"] = JArray.FromObject(list.ToArray());

            return jO;
        }

        /// <summary>
        /// Loads the single.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        /// <returns></returns>
        public async Task<TaggedJObject> LoadSingleAsync(string repositoryKey)
        {
            traceSource.TraceVerbose("Loading single reference `{0}`...", repositoryKey);

            var reference = this.GetBlockBlobReference(repositoryKey);
            var o = await this.GetTaggedJObjectAsync(reference);
            return o;
        }

        /// <summary>
        /// Saves the specified BLOB.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <exception cref="System.ArgumentNullException">blob;The expected blob is not here.</exception>
        public async Task SaveAsync(TaggedJObject blob)
        {
            if (blob == null) throw new ArgumentNullException("blob", "The expected blob is not here.");
            traceSource.TraceVerbose("Saving `{0}`...", blob.Tag);

            var reference = this.GetBlockBlobReference(blob.Tag);
            var json = blob.ToString();
            await reference.SaveJsonAsync(json);
        }

        /// <summary>
        /// Checks the reference.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="key">The key.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        protected async Task CheckReferenceAsync(CloudBlockBlob blob, object key)
        {
            traceSource.TraceVerbose("Checking BLOB reference for `{0}` [key: {1}]...", blob.Uri.OriginalString, key);

            var blobExists = await blob.ExistsAsync();

            if (!blobExists)
            {
                var message = string.Format("The item “{0}” could not be found. Container: {1} {2}", key, this._container.Name, this._container.Uri);
                throw new FileNotFoundException(message);
            }
        }

        /// <summary>
        /// Gets the block BLOB reference.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        /// <returns></returns>
        protected CloudBlockBlob GetBlockBlobReference(string repositoryKey)
        {
            traceSource.TraceVerbose("Getting BLOB reference `{0}`...", repositoryKey);

            if (string.IsNullOrEmpty(repositoryKey)) throw new ArgumentNullException("repositoryKey", "The expected repo key is not here,");
            if (!repositoryKey.EndsWith(".json")) repositoryKey = string.Format("{0}.json", repositoryKey);
            var reference = this._container.GetBlockBlobReference(repositoryKey);
            return reference;
        }

        /// <summary>
        /// Gets the tagged j object.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        protected async Task<TaggedJObject> GetTaggedJObjectAsync(CloudBlockBlob reference)
        {
            traceSource.TraceVerbose("Getting block BLOB reference `{0}`...", reference.Name);

            var json = await reference.DownloadTextAsync();
            var o = new TaggedJObject(json, this._tagPropertyName);
            return o;
        }

        readonly CloudBlobContainer _container;
        readonly string _tagPropertyName;
    }
}
