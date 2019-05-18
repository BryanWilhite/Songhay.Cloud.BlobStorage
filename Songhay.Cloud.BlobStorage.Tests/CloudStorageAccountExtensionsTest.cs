using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Cloud.BlobStorage.Tests
{
    /// <summary>
    /// Tests for <see cref="AzureBlobStorageContext"/>
    /// </summary>
    public class CloudStorageAccountExtensionsTest
    {
        static CloudStorageAccountExtensionsTest()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(CloudStorageAccountExtensionsTest)}";
            traceSource = TraceSources
                .Instance
                .GetConfiguredTraceSource()
                .WithSourceLevels()
                .EnsureTraceSource();
        }

        static readonly TraceSource traceSource;

        public CloudStorageAccountExtensionsTest(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;

            this._basePath = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, "../../../");
            var builder = new ConfigurationBuilder()
                .SetBasePath(this._basePath)
                .AddJsonFile("app-settings.songhay-system.json", optional: false, reloadOnChange: true);

            var meta = new ProgramMetadata();
            builder.Build().Bind(nameof(ProgramMetadata), meta);

            Assert.NotNull(meta.CloudStorageSet);
            var key = "SonghayCloudStorage";
            var test = meta.CloudStorageSet.TryGetValue(key, out var set);
            Assert.True(test, $"The expected cloud storage set, {key}, is not here.");
            Assert.True(set.Any(), $"The expected cloud storage set items for {key} are not here.");

            var connectionString = set.First().Value;
            cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        }

        [Trait("Category", "Integration")]
        [Theory, InlineData("player-audio", "hiphoppoetryjam/png/gina loring.png", "local-storage")]
        public async Task ShouldDownloadBlob(string blobContainerName, string blobContainerPath, string localStorageRoot)
        {
            var root = new DirectoryInfo(this._basePath).ToCombinedPath(localStorageRoot);

            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);
                await cloudStorageAccount.DownloadBlobAsync(root, blobContainerName, blobContainerPath);

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        [Trait("Category", "Integration")]
        [Theory, InlineData("player-audio", "john_marciano0/jpg/background.jpg")]
        public async Task ShouldHaveBlobInContainer(string blobContainerName, string blobContainerPath)
        {
            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                var test = await cloudStorageAccount.IsBlobInContainerAsync(blobContainerPath, blobContainerName);
                Assert.True(test, "The expected blob name is not here.");

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        [Trait("Category", "Integration")]
        [Theory, InlineData("player-video")]
        public async Task ShouldListBlobsInContainer(string blobContainerName)
        {
            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                var container = cloudStorageAccount.GetContainerReference(blobContainerName);
                var blobs = await container.ListBlobsAsync(useFlatBlobListing: true);
                blobs.ForEachInEnumerable(i => this._testOutputHelper.WriteLine(i.Uri.OriginalString));

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        [Trait("Category", "Integration")]
        [Theory, InlineData("shared-social-twitter", "", @"local-storage\11dy.jpg")]
        public async Task ShouldUploadBlob(string blobContainerName, string blobContainerPath, string localFile)
        {
            localFile = new DirectoryInfo(this._basePath).ToCombinedPath(localFile);

            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                await cloudStorageAccount.UploadBlobAsync(localFile, blobContainerName, blobContainerPath);

                var fileInfo = new FileInfo(localFile);
                var test = await cloudStorageAccount.IsBlobInContainerAsync(fileInfo.Name, blobContainerName);

                Assert.True(test, $"The expected Blob, `{fileInfo.Name}`, is not here.");

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        readonly string _basePath;
        readonly ITestOutputHelper _testOutputHelper;
        readonly CloudStorageAccount cloudStorageAccount;
    }
}
