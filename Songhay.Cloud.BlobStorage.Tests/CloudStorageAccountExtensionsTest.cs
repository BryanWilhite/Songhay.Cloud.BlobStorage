using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Cloud.BlobStorage.Tests.Extensions;
using Songhay.Diagnostics;
using Songhay.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Tests
{
    /// <summary>
    /// Tests for <see cref="AzureBlobStorageContext"/>
    /// </summary>
    [TestClass]
    public class CloudStorageAccountExtensionsTest
    {
        static CloudStorageAccountExtensionsTest()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(CloudStorageAccountExtensionsTest)}";
            traceSource = TraceSources
                .Instance
                .GetTraceSourceFromConfiguredName()
                .WithSourceLevels()
                .EnsureTraceSource();
        }

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

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "player-audio")]
        [TestProperty("blobContainerPath", "hiphoppoetryjam/png/gina loring.png")]
        [TestProperty("localStorageRoot", "local-storage")]
        public async Task ShouldDownloadBlob()
        {
            var projectRoot = this.TestContext.ShouldGetAssemblyDirectoryParent(this.GetType(), expectedLevels: 3);

            #region test properties:

            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();
            var blobContainerPath = this.TestContext.Properties["blobContainerPath"].ToString();
            var localStorageRoot = this.TestContext.Properties["localStorageRoot"].ToString();

            localStorageRoot = Path.Combine(projectRoot,
                this.GetType().Namespace.Replace(".Tests", string.Empty),
                this.GetType().Namespace, localStorageRoot);
            this.TestContext.ShouldFindDirectory(localStorageRoot);

            #endregion

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                try
                {
                    traceSource.Listeners.Add(listener);
                    await cloudStorageAccount.DownloadBlobAsync(localStorageRoot, blobContainerName, blobContainerPath);
                }
                finally
                {
                    listener?.Flush();
                }
            }
        }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "player-audio")]
        [TestProperty("blobContainerPath", "john_marciano0/jpg/background.jpg")]
        public async Task ShouldHaveBlobInContainer()
        {
            var projectRoot = this.TestContext.ShouldGetAssemblyDirectoryParent(this.GetType(), expectedLevels: 3);

            #region test properties:

            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();
            var blobContainerPath = this.TestContext.Properties["blobContainerPath"].ToString();

            #endregion

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                try
                {
                    traceSource.Listeners.Add(listener);

                    var test = await cloudStorageAccount.IsBlobInContainerAsync(blobContainerPath, blobContainerName);
                    Assert.IsTrue(test, "The expected blob name is not here.");
                }
                finally
                {
                    listener?.Flush();
                }
            }
        }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "player-video")]
        public async Task ShouldListBlobsInContainer()
        {
            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                try
                {
                    traceSource.Listeners.Add(listener);

                    var container = cloudStorageAccount.GetContainerReference(blobContainerName);
                    var blobs = await container.ListBlobsAsync(useFlatBlobListing: true);
                    blobs.ForEachInEnumerable(i => this.TestContext.WriteLine(i.Uri.OriginalString));
                }
                finally
                {
                    listener?.Flush();
                }
            }
        }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "shared-social-twitter")]
        [TestProperty("blobContainerPath", "")]
        [TestProperty("localFile", @"local-storage\11dy.jpg")]
        public async Task ShouldUploadBlob()
        {
            var projectRoot = this.TestContext.ShouldGetAssemblyDirectoryParent(this.GetType(), expectedLevels: 3);

            #region test properties:

            var localFile = this.TestContext.Properties["localFile"].ToString();

            localFile = Path.Combine(projectRoot,
                this.GetType().Namespace.Replace(".Tests", string.Empty),
                this.GetType().Namespace, localFile);
            this.TestContext.ShouldFindFile(localFile);

            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();
            var blobContainerPath = this.TestContext.Properties["blobContainerPath"].ToString();

            #endregion

            using (var listener = new TextWriterTraceListener(Console.Out))
            {
                try
                {
                    traceSource.Listeners.Add(listener);

                    await cloudStorageAccount.UploadBlobAsync(localFile, blobContainerName, blobContainerPath);

                    var fileInfo = new FileInfo(localFile);
                    var test = await cloudStorageAccount.IsBlobInContainerAsync(fileInfo.Name, blobContainerName);

                    Assert.IsTrue(test, $"The expected Blob, `{fileInfo.Name}`, is not here.");
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
