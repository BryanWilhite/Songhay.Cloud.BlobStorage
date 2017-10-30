# Songhay.Cloud.BlobStorage

`Songhay.Cloud.BlobStorage` is a commitment to `WindowsAzure.Storage` [[NuGet](https://www.nuget.org/packages/WindowsAzure.Storage/)], building upon it to define repositories that turn a container of JSON files into a repository (of the “[repository pattern](https://www.infragistics.com/community/blogs/dhananjay_kumar/archive/2016/03/07/how-to-implement-the-repository-pattern-in-asp-net-mvc-application.aspx)”).

The `AzureBlobContentRepository` [class](./Songhay.Cloud.BlobStorage/Repositories/AzureBlobContentRepository.cs) is a wrapper for [BLOBs](https://en.wikipedia.org/wiki/Binary_large_object), featuring `CloudBlob.DownloadToStreamAsync()` [[docs](https://docs.microsoft.com/en-us/dotnet/api/microsoft.windowsazure.storage.blob.cloudblob.downloadtostreamasync?view=azure-dotnet)].

The `AzureBlobRepository` [class](./Songhay.Cloud.BlobStorage/Repositories/AzureBlobRepository.cs) is a wrapper for BLOBs that are serialized JSON that hydrate into the expected `TEntity`.

The `TaggedJObjectRepository` [class](./Songhay.Cloud.BlobStorage/Repositories/TaggedJObjectRepository.cs) is also a wrapper for BLOBs in a container but these objects hydrate into the general-purpose `JObject` [from Newtonsoft](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm). The “tagged” JObject is arbitrary JSON that has one property that serves as a primitive, primary key.

[@BryanWilhite](https://twitter.com/bryanwilhite)