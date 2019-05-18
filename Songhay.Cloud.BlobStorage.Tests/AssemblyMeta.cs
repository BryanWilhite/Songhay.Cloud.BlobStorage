using Songhay.Tests.Orderers;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(TestCaseOrderer.TypeName, TestCaseOrderer.AssemblyName)]
