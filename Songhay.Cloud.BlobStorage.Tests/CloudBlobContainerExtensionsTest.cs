﻿using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Models;
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
        static CloudBlobContainerExtensionsTest() => traceSource = TraceSources.Instance.GetConfiguredTraceSource().WithAllSourceLevels();
        static readonly TraceSource traceSource;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            var basePath = FrameworkFileUtility.FindParentDirectory(Directory.GetCurrentDirectory(), this.GetType().Namespace, 5);

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("app-settings.songhay-system.json", optional: false, reloadOnChange: true);

            this._configurationRoot = builder.Build();
        }

        [TestMethod]
        [TestProperty("blobContainerName", "songhayblog-azurewebsites-net")]
        public async Task ShouldListBlobsAsync()
        {
            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();

            var connectionString = this._configurationRoot["SonghayCloudStorage"];
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
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

        IConfigurationRoot _configurationRoot;
    }
}