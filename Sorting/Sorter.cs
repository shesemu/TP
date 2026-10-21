namespace Sorting;
public interface IArrayReader { int[] Read(string path); }
public sealed class FileArrayReader : IArrayReader {
    public int[] Read(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string text=File.ReadAllText(path);
        return text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries)
            .Select(token=>int.Parse(token,System.Globalization.CultureInfo.InvariantCulture)).ToArray();
    }
}
public static class Sorter {
    public static void BubbleSort(ref int[] values) {
        ArgumentNullException.ThrowIfNull(values);
        for(int end=values.Length-1;end>0;end--) {
            bool changed=false;
            for(int j=0;j<end;j++) {
                if(values[j]<=values[j+1])continue;
                (values[j],values[j+1])=(values[j+1],values[j]);changed=true;
            }
            if(!changed)break;
        }
    }
}
public sealed class SortService {
    private readonly IArrayReader reader;
    public SortService(IArrayReader reader) { this.reader=reader??throw new ArgumentNullException(nameof(reader)); }
    public int[] SortFile(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        int[] values=reader.Read(path);
        Sorter.BubbleSort(ref values);
        return values;
    }
}
