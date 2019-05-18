using Microsoft.WindowsAzure.Storage;
using Songhay.Models;
using System;
using System.Linq;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ProgramMetadata"/>
    /// </summary>
    public static class ProgramMetadataExtensions
    {
        /// <summary>
        /// Gets the <see cref="CloudStorageAccount"/>
        /// from <see cref="ProgramMetadata"/>
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="cloudStorageSetName"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public static CloudStorageAccount GetCloudStorageAccount(this ProgramMetadata meta, string cloudStorageSetName, string connectionStringName)
        {
            if (meta?.CloudStorageSet == null) throw new NullReferenceException("The expected cloud storage set is not here.");

            var key = cloudStorageSetName;

            var test = meta.CloudStorageSet.TryGetValue(key, out var set);

            if (!test) throw new NullReferenceException($"The expected cloud storage set, {key}, is not here.");
            if (!set.Any()) throw new NullReferenceException($"The expected cloud storage set items for {key} are not here.");

            test = set.TryGetValue(connectionStringName, out var connectionString);
            if (!test) throw new NullReferenceException($"The expected cloud storage set connection, {connectionStringName}, is not here.");

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            return cloudStorageAccount;
        }
    }
}
