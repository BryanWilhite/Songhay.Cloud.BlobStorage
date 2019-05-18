using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Cloud.BlobStorage.Tests.Models;
using Songhay.Cloud.BlobStorage.Tests.Repositories;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Cloud.BlobStorage.Tests
{
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

        public CloudBlobContainerExtensionsTest(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;

            var basePath = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, "../../../");
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("app-settings.songhay-system.json", optional: false, reloadOnChange: true);

            var meta = new ProgramMetadata();
            builder.Build().Bind(nameof(ProgramMetadata), meta);
            cloudStorageAccount = meta.GetCloudStorageAccount("SonghayCloudStorage", "classic");
        }

        [Trait("Category", "Integration")]
        [Theory, InlineData("songhayblog-azurewebsites-net")]
        public async Task ShouldListBlobsAsync(string blobContainerName)
        {
            using(var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                var container = cloudStorageAccount.GetContainerReference(blobContainerName);
                var keys = new AzureBlobKeys();
                keys.Add<BlogEntry>(i => i.Slug);

                var repository = new BlogRepository(keys, container);
                Assert.NotNull(repository);

                var entries = await repository.LoadAllAsync<BlogEntry>();
                Assert.True(entries.Any(), "The expected Blog entries are not here.");

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        readonly ITestOutputHelper _testOutputHelper;
        readonly CloudStorageAccount cloudStorageAccount;
    }
}
