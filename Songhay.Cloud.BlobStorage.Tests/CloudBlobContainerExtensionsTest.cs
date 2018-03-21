using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Cloud.BlobStorage.Tests.Extensions;
using Songhay.Cloud.BlobStorage.Tests.Models;
using Songhay.Cloud.BlobStorage.Tests.Repositories;
using Songhay.Diagnostics;
using Songhay.Extensions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Tests
{
    [TestClass]
    public class CloudBlobContainerExtensionsTest
    {
        static CloudBlobContainerExtensionsTest() => traceSource = TraceSources
            .Instance
            .GetTraceSourceFromConfiguredName()
            .WithAllSourceLevels()
            .EnsureTraceSource();

        static readonly TraceSource traceSource;

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
            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();

            var container = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(blobContainerName);
            var keys = new AzureBlobKeys();
            keys.Add<BlogEntry>(i => i.Slug);

            var repository = new BlogRepository(keys, container);
            Assert.IsNotNull(repository, "The expected repository is not here.");

            var entries = await repository.LoadAllAsync<BlogEntry>();
            Assert.IsTrue(entries.Any(), "The expected Blog entries are not here.");
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        static CloudStorageAccount cloudStorageAccount;
    }
}
