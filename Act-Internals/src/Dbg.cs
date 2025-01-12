using static Act.Datapack;

namespace Act;

public class Dbg
{
    public static void PrintHeaderList(ref List<ChunkHeader> list)
    {
        foreach (var entry in list)
        {
            Console.WriteLine($"[ChnkHeader] name: {entry.name}");
            Console.WriteLine($"[ChnkHeader] startOffset: 0x{entry.startOffset:X}");
            Console.WriteLine($"[ChnkHeader] endOffset: 0x{entry.endOffset:X}");
            Console.WriteLine("");
        }
    }

    public static void PrintDirectoryList(ref List<DirectoryEntry> list)
    {
        foreach (var entry in list)
        {
            Console.WriteLine($"[Directory] name: {entry.name}");
            Console.WriteLine($"[Directory] offset: 0x{entry.offset:X}");
            Console.WriteLine("");
        }
    }
}
