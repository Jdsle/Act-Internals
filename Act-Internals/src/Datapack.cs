using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Act;

public class Datapack
{
    // ---------
    // Constants
    // ---------

    // 4 characters for the header's name
    public const int HeaderNameSize = 0x4;

    public const int HeaderSize = 0x10;
    public const int EntrySize = 0x10;

    // -------------
    // Chunk Headers
    // -------------

    public struct ChunkHeader
    {
        public string name;
        public long startOffset;
        public long endOffset;

        public ChunkHeader(string name, long offset)
        {
            this.name = name;
            this.startOffset = offset;
            this.endOffset = this.startOffset + HeaderSize;
        }
    }

    public static List<ChunkHeader> ReadHeaders(string filePath)
    {
        var headers = new List<ChunkHeader>();

        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            while (fs.Position < fs.Length)
            {
                long startOffset = fs.Position;

                byte[] buffer = br.ReadBytes(HeaderSize);
                if (buffer.Length < HeaderSize) break;

                string signature = Encoding.ASCII.GetString(buffer, HeaderSize - 4, 4);
                if (signature != "CHNK") continue;

                string name = Encoding.ASCII.GetString(buffer, 0, HeaderNameSize).Trim();
                ChunkHeader header = new ChunkHeader(name, startOffset);

                headers.Add(header);
            }
        }

        return headers;
    }

    public static List<ChunkHeader> HeadersByName(List<ChunkHeader> headers, string name)
    {
        return headers.FindAll(header => header.name == name);
    }

    // -----------------
    // Directory Entries
    // -----------------

    public struct DirectoryEntry
    {
        public string name;
        public uint offset;

        public DirectoryEntry(string name, uint offset)
        {
            this.name = name;
            this.offset = offset;
        }
    }

    public static List<DirectoryEntry> ReadDirectories(string filePath, long startOffset)
    {
        var entries = new List<DirectoryEntry>();

        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            fs.Seek(startOffset, SeekOrigin.Begin);

            while (fs.Position < fs.Length)
            {
                byte[] entry = br.ReadBytes(EntrySize);
                if (entry.Length < EntrySize) break;

                string filename = Encoding.ASCII.GetString(entry, 0, 12).TrimEnd('\0');
                uint offset = BitConverter.ToUInt32(entry, 12);

                var dir = new DirectoryEntry(filename, offset);
                entries.Add(dir);

                byte[] headerBuffer = br.ReadBytes(EntrySize);
                if (headerBuffer.Length < EntrySize) break;

                string signature = Encoding.ASCII.GetString(headerBuffer, EntrySize - 4, 4);
                if (signature == "CHNK")
                {
                    fs.Seek(-EntrySize, SeekOrigin.Current);
                    break;
                }
                else
                {
                    fs.Seek(-EntrySize, SeekOrigin.Current);
                }
            }
        }

        return entries;
    }
}