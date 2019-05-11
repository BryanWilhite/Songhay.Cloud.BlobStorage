#if NETSTANDARD

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Songhay.Models;
using System;
using System.IO;
using System.Linq;

namespace Songhay.Cloud.BlobStorage.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ConfigurationBuilder"/>
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Converts <see cref="ConfigurationBuilder"/> to <see cref="CloudStorageAccount"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configurationBasePath">The configuration base path.</param>
        /// <param name="conventionalSettingsFileName">Name of the conventional settings file.</param>
        /// <param name="cloudStorageSetName">Name of the cloud storage set.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">
        /// The expected Configuration builder is not here.
        /// or
        /// The expected cloud storage set is not here.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">The expected configuration base path is not here.</exception>
        public static CloudStorageAccount ToCloudStorageAccount(this ConfigurationBuilder builder, string configurationBasePath, string conventionalSettingsFileName, string cloudStorageSetName, string connectionStringName)
        {
            if (builder == null) throw new NullReferenceException("The expected Configuration builder is not here.");
            if (!Directory.Exists(configurationBasePath)) throw new DirectoryNotFoundException("The expected configuration base path is not here.");

            builder
                .SetBasePath(configurationBasePath)
                .AddJsonFile(conventionalSettingsFileName, optional: false, reloadOnChange: true);

            var meta = new ProgramMetadata();
            builder.Build().Bind(nameof(ProgramMetadata), meta);

            if (meta.CloudStorageSet == null) throw new NullReferenceException("The expected cloud storage set is not here.");

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

#endif
