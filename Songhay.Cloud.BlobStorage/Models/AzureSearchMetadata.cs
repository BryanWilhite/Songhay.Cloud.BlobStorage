using Songhay.Models;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// Defines metadata needed for Azure Search.
    /// </summary>
    public class AzureSearchMetadata : RestApiMetadata
    {
        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        /// <value>
        /// The name of the index.
        /// </value>
        public string IndexName { get; set; }
    }
}
