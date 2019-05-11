#if NETSTANDARD

using Songhay.Cloud.BlobStorage.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="AzureBlobContent"/>
    /// </summary>
    public static class AzureBlobContentExtensions
    {
        /// <summary>
        /// Converts <see cref="AzureBlobContent"/> to <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static HttpResponseMessage ToHttpResponseMessage(this AzureBlobContent content)
        {
            if (content == null) throw new ArgumentNullException("content", "The expected Azure BLOB content is not here.");

            content.BlobStream.Position = 0;

            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(content.BlobStream)
            };

            message.Content.Headers.ContentLength = content.BlobLength;
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(content.BlobContentType);
            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = HttpUtility.UrlDecode(content.BlobFileName),
                Size = content.BlobLength
            };

            return message;
        }
    }
}

#endif
