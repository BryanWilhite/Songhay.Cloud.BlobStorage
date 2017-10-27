using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Songhay.Models;

namespace Songhay.Cloud
{
    /// <summary>
    /// Static members for Azure Storage
    /// </summary>
    public static class AzureStorageUtility
    {
        /// <summary>
        /// Gets the cloud storage account.
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="azureTableStorageKey">The azure table storage key.</param>
        public static CloudStorageAccount GetCloudStorageAccount(string accountName, string azureTableStorageKey)
        {
            return GetCloudStorageAccount(accountName, azureTableStorageKey, false);
        }

        /// <summary>
        /// Gets the cloud storage account.
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="azureTableStorageKey">The azure table storage key.</param>
        /// <param name="useHttps">if set to <c>true</c> [use HTTPS].</param>
        public static CloudStorageAccount GetCloudStorageAccount(string accountName, string azureTableStorageKey, bool useHttps)
        {
            return new CloudStorageAccount(new StorageCredentials(accountName, azureTableStorageKey), useHttps);
        }

        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        public static string GetMimeType(string fileExtension)
        {
            var mimeType = MimeTypes.ApplicationOctetStream;

            switch (fileExtension)
            {
                case ".css":
                    mimeType = MimeTypes.TextCss;
                    break;
                case ".eot":
                    mimeType = MimeTypes.ApplicationVndMsFontObject;
                    break;
                case ".gif":
                    mimeType = MimeTypes.ImageGif;
                    break;
                case ".html":
                    mimeType = MimeTypes.TextHtml;
                    break;
                case ".ico":
                    mimeType = MimeTypes.ImageXIcon;
                    break;
                case ".jpeg":
                case ".jpg":
                    mimeType = MimeTypes.ImageJpeg;
                    break;
                case ".js":
                    mimeType = MimeTypes.TextJavascript;
                    break;
                case ".json":
                case ".map":
                    mimeType = MimeTypes.ApplicationJson;
                    break;
                case ".otf":
                    mimeType = MimeTypes.ApplicationXFontOtf;
                    break;
                case ".png":
                    mimeType = MimeTypes.ImagePng;
                    break;
                case ".ps1":
                case ".scss":
                case ".txt":
                    mimeType = MimeTypes.TextPlain;
                    break;
                case ".svg":
                    mimeType = MimeTypes.ImageSvgXml;
                    break;
                case ".ttf":
                    mimeType = MimeTypes.ApplicationXFontTtf;
                    break;
                case ".wof":
                    mimeType = MimeTypes.ApplicationXFontWoff;
                    break;
                case ".xml":
                    mimeType = MimeTypes.ApplicationXml;
                    break;
            }

            return mimeType;
        }
    }
}
