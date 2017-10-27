using System;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// Defines the filter for <see cref="ITaggedJObjectRepositoryAsync.LoadSet(string)"/>
    /// </summary>
    /// <remarks>
    /// This class definition intends to filter by BLOB name under a container folder.
    /// </remarks>
    public class AzureBlobSetFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobSetFilter"/> class.
        /// </summary>
        public AzureBlobSetFilter()
        {
            this.Filter = i => true;
        }

        /// <summary>
        /// Gets or sets the name of the directory.
        /// </summary>
        /// <value>
        /// The name of the directory.
        /// </value>
        public string DirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public Func<string, bool> Filter { get; set; }
    }
}
