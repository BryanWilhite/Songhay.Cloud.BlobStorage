using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Extensions;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Cloud.BlobStorage.Repositories;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Tests.Orderers;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Cloud.BlobStorage.Tests
{
    public class TaggedJObjectTest
    {
        static TaggedJObjectTest()
        {
            TraceSources.ConfiguredTraceSourceName = $"trace-{nameof(TaggedJObjectTest)}";
            traceSource = TraceSources
                .Instance
                .GetConfiguredTraceSource()
                .WithSourceLevels()
                .EnsureTraceSource();
        }

        static readonly TraceSource traceSource;

        public TaggedJObjectTest(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;

            var basePath = FrameworkAssemblyUtility.GetPathFromAssembly(this.GetType().Assembly, "../../../");
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("app-settings.songhay-system.json", optional : false, reloadOnChange : true);

            var meta = new ProgramMetadata();
            builder.Build().Bind(nameof(ProgramMetadata), meta);

            cloudStorageAccount = meta.GetCloudStorageAccount("SonghayCloudStorage", "classic");
        }

        [Trait("Category", "Integration")]
        [Theory, TestOrder(ordinal: 3), InlineData("integration-test-container", @"{""id"":""ShouldSaveTaggedJObjectToRepo"", ""content"": ""Hello world!""}")]
        public async Task ShouldDeleteTaggedJObjectInRepo(string blobContainerName, string json)
        {
            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                var container = cloudStorageAccount.GetContainerReference(blobContainerName);

                var repo = new TaggedJObjectRepository(container, "id");
                var data = new TaggedJObject(json, "id");

                var test = await repo.HasBlobAsync(data.Tag);
                Assert.True(test, "The expected blob is not here.");

                await repo.DeleteAsync(data.Tag);
                test = await repo.HasBlobAsync(data.Tag);
                Assert.False(test, "The blob is not expected.");

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        [Theory, InlineData(@"{""id"":""1234"", ""content"": ""Hello world!""}")]
        public void ShouldInstanceTaggedJObject(string json)
        {
            var data = new TaggedJObject(json, "id");
            this._testOutputHelper.WriteLine($"tag: {data.Tag}");
            this._testOutputHelper.WriteLine(data.ToString().EscapeInterpolation());
            Assert.NotNull(data.Tag);
        }

        [Trait("Category", "Integration")]
        [Theory, TestOrder(ordinal: 2), InlineData("integration-test-container", "ShouldSaveTaggedJObjectToRepo")]
        public async Task ShouldLoadTaggedJObjectFromRepo(string blobContainerName, string respositoryKey)
        {
            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                var container = cloudStorageAccount.GetContainerReference(blobContainerName);

                var repo = new TaggedJObjectRepository(container, "id");
                var data = await repo.LoadSingleAsync(respositoryKey);
                this._testOutputHelper.WriteLine(data.ToString().EscapeInterpolation());

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        [Trait("Category", "Integration")]
        [Theory, TestOrder(ordinal: 1), InlineData("integration-test-container", @"{""id"":""ShouldSaveTaggedJObjectToRepo"", ""content"": ""Hello world!""}")]
        public async Task ShouldSaveTaggedJObjectToRepo(string blobContainerName, string json)
        {
            using (var writer = new StringWriter())
            using (var listener = new TextWriterTraceListener(writer))
            {
                traceSource.Listeners.Add(listener);

                var container = cloudStorageAccount.GetContainerReference(blobContainerName);

                var repo = new TaggedJObjectRepository(container, "id");
                var data = new TaggedJObject(json, "id");

                await repo.SaveAsync(data);
                var test = await repo.HasBlobAsync(data.Tag);
                Assert.True(test, "The expected blob is not here.");

                listener.Flush();
                this._testOutputHelper.WriteLine(writer.ToString());
            }
        }

        readonly ITestOutputHelper _testOutputHelper;
        readonly CloudStorageAccount cloudStorageAccount;
    }
}