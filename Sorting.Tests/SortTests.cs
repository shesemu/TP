using NUnit.Framework;
using Sorting;
namespace SortingTests;
public sealed class FakeReader : IArrayReader {
    public int[] Data = [];
    public string? LastPath;
    public Exception? Failure;
    public int[] Read(string path) { LastPath=path;if(Failure is not null)throw Failure;return Data; }
}
[TestFixture]
public class SortTests {
    private static IEnumerable<TestCaseData> Cases() {
        yield return new TestCaseData(new int[]{3,1,2},new int[]{1,2,3});
        yield return new TestCaseData(Array.Empty<int>(),Array.Empty<int>());
        yield return new TestCaseData(new int[]{7},new int[]{7});
        yield return new TestCaseData(new int[]{1,2,3},new int[]{1,2,3});
        yield return new TestCaseData(new int[]{3,2,1},new int[]{1,2,3});
        yield return new TestCaseData(new int[]{2,-1,2,0},new int[]{-1,0,2,2});
        yield return new TestCaseData(new int[]{int.MaxValue,int.MinValue,0},new int[]{int.MinValue,0,int.MaxValue});
    }
    [TestCaseSource(nameof(Cases))]
    public void SortsAndKeepsReference(int[] input,int[] expected) {
        int[] original=input;Sorter.BubbleSort(ref input);
        Assert.Multiple(()=>{Assert.That(input,Is.EqualTo(expected));Assert.That(input,Is.SameAs(original));});
    }
    [Test] public void NullRejected() {
        int[] input=null!;
        Assert.Throws<ArgumentNullException>(()=>Sorter.BubbleSort(ref input));
    }
    [Test] public void ServiceUsesReaderDouble() {
        var reader=new FakeReader{Data=[3,1,2]};var service=new SortService(reader);
        Assert.That(service.SortFile("example.txt"),Is.EqualTo(new[]{1,2,3}));
        Assert.That(reader.LastPath,Is.EqualTo("example.txt"));
    }
    [Test] public void ReaderFailurePropagates() {
        var service=new SortService(new FakeReader{Failure=new FileNotFoundException("test")});
        Assert.Throws<FileNotFoundException>(()=>service.SortFile("missing.txt"));
    }
    [TestCase("")][TestCase(" ")][TestCase(null)]
    public void InvalidPathRejected(string? path) {
        var reader=new FakeReader();var service=new SortService(reader);
        Assert.That(()=>service.SortFile(path!),Throws.InstanceOf<ArgumentException>());
        Assert.That(reader.LastPath,Is.Null);
    }
    [Test] public void RealReaderParsesWhitespace() {
        string path=Path.Combine(TestContext.CurrentContext.WorkDirectory,Guid.NewGuid()+".txt");
        try { File.WriteAllText(path,"3\t-1\n2  0");Assert.That(new FileArrayReader().Read(path),Is.EqualTo(new[]{3,-1,2,0})); }
        finally { File.Delete(path); }
    }
    [Test] public void RealReaderRejectsNonInteger() {
        string path=Path.Combine(TestContext.CurrentContext.WorkDirectory,Guid.NewGuid()+".txt");
        try { File.WriteAllText(path,"1 abc 3");Assert.Throws<FormatException>(()=>new FileArrayReader().Read(path)); }
        finally { File.Delete(path); }
    }
}
