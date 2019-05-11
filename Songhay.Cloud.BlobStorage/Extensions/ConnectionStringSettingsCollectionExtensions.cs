#if NET452 || NET462

using Microsoft.WindowsAzure.Storage;
using System;
using System.Configuration;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ConnectionStringsCollection"/>
    /// </summary>
    public static class ConnectionStringSettingsCollectionExtensions
    {
        /// <summary>
        /// Gets the cloud storage account.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The expected collection is not here.</exception>
        public static CloudStorageAccount GetCloudStorageAccount(this ConnectionStringSettingsCollection collection, string key)
        {
            if (collection == null) throw new ArgumentNullException("The expected collection is not here.");

            var cnn = collection[key];
            if (cnn == null) throw new ArgumentNullException("The expected settings are not here.");

            var account = CloudStorageAccount.Parse(cnn.ConnectionString);
            return account;
        }
    }
}

#endif
