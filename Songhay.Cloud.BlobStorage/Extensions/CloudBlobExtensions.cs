using Microsoft.WindowsAzure.Storage.Blob;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Xml;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of Azure storage BLOB types.
    /// </summary>
    /// <remarks>
    /// Microsoft may change the names/namespaces of their Azure Storage API
    /// so this static class is meant to encapsulate various API storage types,
    /// breaking naming conventions.
    /// </remarks>
    public static class CloudBlobExtensions
    {
        static CloudBlobExtensions() => traceSource = TraceSources.Instance.GetConfiguredTraceSource().WithSourceLevels();

        static readonly TraceSource traceSource;

        /// <summary>
        /// Downloads the BLOB.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="localRoot">The local root.</param>
        public static async Task DownloadBlobAsync(this ICloudBlob blob, string localRoot)
        {
            if (blob == null)
            {
                traceSource?.TraceError("The expected cloud blob is not here.");
                return;
            }

            if (string.IsNullOrEmpty(localRoot))
            {
                traceSource?.TraceError("The expected local root path is not here.");
                return;
            }

            var localPath = Path.Combine(localRoot, blob.Name.Replace("/", @"\"));

            traceSource?.TraceVerbose($"Downloading {blob.Name} to {localPath}...");
            await blob.DownloadToFileAsync(localPath, FileMode.OpenOrCreate);
        }

        /// <summary>
        /// Saves the JSON.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="json">The JSON.</param>
        public static async Task SaveJsonAsync(this CloudBlockBlob blob, string json)
        {
            if (blob == null)
            {
                traceSource?.TraceError("The expected cloud blob is not here.");
                return;
            }

            blob.Properties.ContentType = MimeTypes.ApplicationJson;
            await blob.UploadTextAsync(json);
        }

        /// <summary>
        /// Converts the <see cref="ICloudBlob"/> to a <see cref="string"/> representation.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <returns></returns>
        public static async Task<string> ToStringFromBlobAsync(this ICloudBlob blob)
        {
            if (blob == null)
            {
                traceSource?.TraceError("The expected cloud blob is not here.");
                return null;
            }

            var s = default(string);
            using (var ms = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(ms);
                ms.Position = 0;
                s = XmlUtility.GetText(ms);
            }
            return s;
        }
    }
}
