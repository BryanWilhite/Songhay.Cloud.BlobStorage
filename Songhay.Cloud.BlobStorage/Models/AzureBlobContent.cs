using System.IO;

namespace Songhay.Cloud.BlobStorage.Models
{
    /// <summary>
    /// Defines an Azure BLOB.
    /// </summary>
    /// <remarks>
    /// This is based on “Upload and Download Files with Web API and Azure Blob Storage”
    /// [http://arcware.net/upload-and-download-files-with-web-api-and-azure-blob-storage/]
    /// </remarks>
    public class AzureBlobContent
    {
        /// <summary>
        /// Gets or sets the BLOB stream.
        /// </summary>
        /// <value>
        /// The BLOB stream.
        /// </value>
        public MemoryStream BlobStream { get; set; }

        /// <summary>
        /// Gets or sets the name of the BLOB file.
        /// </summary>
        /// <value>
        /// The name of the BLOB file.
        /// </value>
        public string BlobFileName { get; set; }

        /// <summary>
        /// Gets or sets the type of the BLOB content.
        /// </summary>
        /// <value>
        /// The type of the BLOB content.
        /// </value>
        public string BlobContentType { get; set; }

        /// <summary>
        /// Gets or sets the length of the BLOB.
        /// </summary>
        /// <value>
        /// The length of the BLOB.
        /// </value>
        public long BlobLength { get; set; }
    }
}
