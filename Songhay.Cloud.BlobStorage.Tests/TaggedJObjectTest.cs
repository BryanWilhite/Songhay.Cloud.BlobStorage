using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Songhay.Cloud.BlobStorage.Models;
using Songhay.Cloud.BlobStorage.Repositories;
using Songhay.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Tests
{
    [TestClass]
    public class TaggedJObjectTest
    {
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

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "integration-test-container")]
        [TestProperty("json", @"{""id"":""ShouldSaveTaggedJObjectToRepo"", ""content"": ""Hello world!""}")]
        public async Task ShouldDeleteTaggedJObjectInRepo()
        {
            #region test properties:

            var json = this.TestContext.Properties["json"].ToString();
            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();

            #endregion

            var connectionString = this._configurationRoot["SonghayCloudStorage"];
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var container = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(blobContainerName);

            var repo = new TaggedJObjectRepository(container, "id");
            var data = new TaggedJObject(json, "id");

            var test = await repo.HasBlobAsync(data.Tag);
            Assert.IsTrue(test, "The expected blob is not here.");

            await repo.DeleteAsync(data.Tag);
            test = await repo.HasBlobAsync(data.Tag);
            Assert.IsFalse(test, "The blob is not expected.");
        }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("json", @"{""id"":""1234"", ""content"": ""Hello world!""}")]
        public void ShouldGetTaggedJObject()
        {
            var json = this.TestContext.Properties["json"].ToString();

            var data = new TaggedJObject(json, "id");
            this.TestContext.WriteLine("tag: {0}", data.Tag);
            this.TestContext.WriteLine(data.ToString().EscapeInterpolation());
            Assert.IsNotNull(data.Tag, "The expected tag is not here.");
        }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "integration-test-container")]
        [TestProperty("respositoryKey", "ShouldSaveTaggedJObjectToRepo")]
        public async Task ShouldLoadTaggedJObjectFromRepo()
        {
            #region test properties:

            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();
            var respositoryKey = this.TestContext.Properties["respositoryKey"].ToString();

            #endregion

            var connectionString = this._configurationRoot["SonghayCloudStorage"];
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var container = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(blobContainerName);

            var repo = new TaggedJObjectRepository(container, "id");
            var data = await repo.LoadSingleAsync(respositoryKey);
            this.TestContext.WriteLine(data.ToString().EscapeInterpolation());
        }

        [TestCategory("Integration")]
        [TestMethod]
        [TestProperty("blobContainerName", "integration-test-container")]
        [TestProperty("json", @"{""id"":""ShouldSaveTaggedJObjectToRepo"", ""content"": ""Hello world!""}")]
        public async Task ShouldSaveTaggedJObjectToRepo()
        {
            #region test properties:

            var json = this.TestContext.Properties["json"].ToString();
            var blobContainerName = this.TestContext.Properties["blobContainerName"].ToString();

            #endregion

            var connectionString = this._configurationRoot["SonghayCloudStorage"];
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var container = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(blobContainerName);

            var repo = new TaggedJObjectRepository(container, "id");
            var data = new TaggedJObject(json, "id");

            await repo.SaveAsync(data);
            var test = await repo.HasBlobAsync(data.Tag);
            Assert.IsTrue(test, "The expected blob is not here.");
        }

        IConfigurationRoot _configurationRoot;
    }
}
