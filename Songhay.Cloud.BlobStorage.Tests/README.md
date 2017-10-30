# Songhay.Cloud.BlobStorage.Tests

The TaggedJObjectTest [suite](./TaggedJObjectTest.cs) is in need of some kind of way to express intent for an “ordered” test. Until this MSTest2 issue is resolved this is the intent:

```plaintext
ShouldSaveTaggedJObjectToRepo()
ShouldLoadTaggedJObjectFromRepo()
ShouldDeleteTaggedJObjectInRepo()
```