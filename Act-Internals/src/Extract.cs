using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Act;

class Extract
{
    public class WSB
    {
        private const int RIFFHeaderSize = 0xC;

        public static void Start()
        {
            if (!File.Exists("wsb.dir"))
            {
                Console.WriteLine("Failed to open wsb.dir");
                return;
            }

            var headers = Datapack.ReadHeaders("WSB.dir");
            var DIR = Datapack.HeadersByName(headers, "DIR");
            var entries = Datapack.ReadDirectories("wsb.dir", DIR[0].endOffset);

            Extract("wsb.dir", entries, "wsb");
        }

        public static void Extract(string filePath, List<Datapack.DirectoryEntry> entries, string output)
        {
            Directory.CreateDirectory(output);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var br = new BinaryReader(fs))
            {
                foreach (var entry in entries)
                {
                    fs.Seek(entry.offset, SeekOrigin.Begin);

                    byte[] WVST_Header = br.ReadBytes(Datapack.HeaderSize);
                    if (Encoding.ASCII.GetString(WVST_Header, 0, 4) == "WVST")
                    {
                        fs.Seek(entry.offset + Datapack.HeaderSize, SeekOrigin.Begin);
                    }
                    else
                    {
                        Console.WriteLine($"Unexpected chunk header at: 0x{entry.offset:X}");
                        continue;
                    }

                    byte[] RIFF_Header = br.ReadBytes(RIFFHeaderSize);
                    if (Encoding.ASCII.GetString(RIFF_Header, 0, 4) != "RIFF")
                    {
                        Console.WriteLine($"Expected 'RIFF' header at 0x{entry.offset + Datapack.HeaderSize:X} - not found!");
                        continue;
                    }

                    uint fileSize = BitConverter.ToUInt32(RIFF_Header, 4) + 8;
                    byte[] fileData = new byte[fileSize];
                    Array.Copy(RIFF_Header, fileData, RIFFHeaderSize);
                    br.Read(fileData, RIFFHeaderSize, (int)(fileSize - RIFFHeaderSize));

                    string pathOut = Path.Combine(output, entry.name.Trim());
                    try
                    {
                        File.WriteAllBytes(pathOut, fileData);
                        Console.WriteLine($"Extracted: {pathOut} (Size: {fileSize} bytes)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to save file: {pathOut}. Error: {ex.Message}");
                    }
                }
            }
        }
    }
}
