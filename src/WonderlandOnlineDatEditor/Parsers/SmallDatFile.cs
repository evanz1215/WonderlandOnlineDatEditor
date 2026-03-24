using System;
using System.Collections.Generic;
using System.IO;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// Gec.Dat / Lbd.dat — small encrypted data files.
/// Format: [Count 2B][Records of N bytes each]
/// Gec.Dat: 3 records × 18 bytes (9 × uint16 per record)
/// Lbd.dat: 3 records × 39 bytes ([uint16 value][uint8 count] triplets)
/// XOR keys unknown — displayed as raw uint16 values.
/// </summary>
public class SmallDatRow
{
    public int Index { get; set; }
    public string Values { get; set; } = "";
    public string RawHex { get; set; } = "";
}

public class SmallDatFile
{
    public string FilePath { get; }
    public string FileType { get; }
    public ushort Count { get; }
    public int RecordSize { get; }
    public List<SmallDatRow> Rows { get; } = new();

    private SmallDatFile(string path, string fileType, ushort count, int recordSize)
    {
        FilePath = path;
        FileType = fileType;
        Count = count;
        RecordSize = recordSize;
    }

    public static SmallDatFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        string name = Path.GetFileName(path);

        ushort count = (ushort)(data[0] | (data[1] << 8));
        int recordSize = count > 0 ? (data.Length - 2) / count : 0;

        var file = new SmallDatFile(path, name, count, recordSize);

        for (int i = 0; i < count; i++)
        {
            int off = 2 + i * recordSize;
            if (off + recordSize > data.Length) break;

            // Read uint16 values
            var values = new List<string>();
            for (int j = 0; j + 1 < recordSize; j += 2)
            {
                ushort v = (ushort)(data[off + j] | (data[off + j + 1] << 8));
                values.Add(v.ToString());
            }

            // Raw hex
            var hex = new System.Text.StringBuilder();
            for (int j = 0; j < recordSize; j++)
            {
                if (j > 0) hex.Append(' ');
                hex.Append(data[off + j].ToString("X2"));
            }

            file.Rows.Add(new SmallDatRow
            {
                Index = i,
                Values = string.Join(", ", values),
                RawHex = hex.ToString(),
            });
        }

        return file;
    }

    public List<SmallDatRow> ToRows() => Rows;
}
