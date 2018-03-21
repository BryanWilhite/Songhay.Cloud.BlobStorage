using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Songhay.Models;
using System.Linq;

namespace Songhay.Cloud.BlobStorage.Tests.Extensions
{
    /// <summary>
    /// Extensions of <see cref="TestContext"/>
    /// </summary>
    public static class TestContextExtensions
    {
        public static CloudStorageAccount ShouldGetCloudStorageAccount(this TestContext context, string basePath)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("app-settings.songhay-system.json", optional: false, reloadOnChange: true);

            var meta = new ProgramMetadata();
            builder.Build().Bind(nameof(ProgramMetadata), meta);
            context.WriteLine($"{meta}");

            Assert.IsNotNull(meta.CloudStorageSet, "The expected cloud storage set is not here.");

            var key = "SonghayCloudStorage";
            var test = meta.CloudStorageSet.TryGetValue(key, out var set);
            Assert.IsTrue(test, $"The expected cloud storage set, {key}, is not here.");
            Assert.IsTrue(set.Any(), $"The expected cloud storage set items for {key} are not here.");

            var connectionString = set.First().Value;
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            return cloudStorageAccount;
        }
    }
}
