using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Cloud.BlobStorage.Tests.Extensions;
using Songhay.Cloud.BlobStorage.Tests.Models;
using Songhay.Cloud.BlobStorage.Tests.Repositories;
using Songhay.Diagnostics;
using Songhay.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Tests
{
    [TestClass]
    public class CloudBlobContainerExtensionsTest
    {
        static CloudBlobContainerExtensionsTest()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(CloudBlobContainerExtensionsTest)}";
            traceSource = TraceSources
                .Instance
                .GetConfiguredTraceSource()
                .WithSourceLevels()
                .EnsureTraceSource();
        }

        static readonly TraceSource traceSource;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            var basePath = FrameworkFileUtility.FindParentDirectory(Directory.GetCurrentDirectory(), this.GetType().Namespace, 5);
            cloudStorageAccount = this.TestContext.ShouldGetCloudStorageAccount(basePath);
        }

        [TestMethod]
        [TestProperty("blobContainerName", "songhayblog-azurewebsites-net")]
        public async Task ShouldListBlobsAsync()
        {
            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                try
                {
                    traceSource.Listeners.Add(listener);

                    var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();

                    var container = cloudStorageAccount.GetContainerReference(blobContainerName);
                    var keys = new AzureBlobKeys();
                    keys.Add<BlogEntry>(i => i.Slug);

                    var repository = new BlogRepository(keys, container);
                    Assert.IsNotNull(repository, "The expected repository is not here.");

                    var entries = await repository.LoadAllAsync<BlogEntry>();
                    Assert.IsTrue(entries.Any(), "The expected Blog entries are not here.");
                }
                finally
                {
                    listener?.Flush();
                }
            }
        }

        static CloudStorageAccount cloudStorageAccount;
    }
}
