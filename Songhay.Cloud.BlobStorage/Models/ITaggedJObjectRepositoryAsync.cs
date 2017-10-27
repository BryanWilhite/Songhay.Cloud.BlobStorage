using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// Defines the repository pattern for <see cref="TaggedJObject"/>.
    /// </summary>
    public interface ITaggedJObjectRepositoryAsync
    {
        /// <summary>
        /// Deletes the specified repository key.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        Task DeleteAsync(string repositoryKey);

        /// <summary>
        /// Determines whether the specified repository key has BLOB.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        /// <returns></returns>
        Task<bool> HasBlobAsync(string repositoryKey);

        /// <summary>
        /// Loads the set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        Task<JObject> LoadSetAsync(Func<Uri, bool> filter);

        /// <summary>
        /// Loads the single.
        /// </summary>
        /// <param name="repositoryKey">The repository key.</param>
        /// <returns></returns>
        Task<TaggedJObject> LoadSingleAsync(string repositoryKey);

        /// <summary>
        /// Saves the specified BLOB.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        Task SaveAsync(TaggedJObject blob);
    }
}
