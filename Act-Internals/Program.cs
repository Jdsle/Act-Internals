using System;
using System.Reflection.PortableExecutable;

namespace Act;

internal class Program
{
    static void Main(string[] args)
    {
        List<Datapack.ChunkHeader> headers = Datapack.ReadHeaders("WSB.dir");
        Dbg.PrintHeaderList(ref headers);

        // using headers[0], because there should really only be one DIR chunk
        List<Datapack.ChunkHeader> DIR = Datapack.HeadersByName(headers, "DIR");

        List<Datapack.DirectoryEntry> Directory = Datapack.ReadDirectories("WSB.dir", DIR[0].endOffset);
        Dbg.PrintDirectoryList(ref Directory);
    }
}
