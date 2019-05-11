#if NETSTANDARD

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ConnectionStringsCollection"/>
    /// </summary>
    public static class IConfigurationRootExtensions
    {
        /// <summary>
        /// Gets the cloud storage account.
        /// </summary>
        /// <param name="configuration">The collection.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The expected collection is not here.</exception>
        public static CloudStorageAccount GetCloudStorageAccount(this IConfigurationRoot configuration, string key)
        {
            if (configuration == null) throw new ArgumentNullException("The expected configuration is not here.");

            var cnn = configuration[key];
            if (cnn == null) throw new ArgumentNullException("The expected setting is not here.");

            var account = CloudStorageAccount.Parse(cnn);
            return account;
        }
    }
}

#endif
