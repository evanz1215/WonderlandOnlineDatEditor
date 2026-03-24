using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// Generic raw binary viewer — shows file data as hex rows in a DataGrid.
/// Used for unknown/unsupported file formats (Talk.dat, Mark.dat, SkillData.MBTM, Formula.dat, Gec.Dat, Lbd.dat).
/// </summary>
public class RawRow
{
    public int Offset { get; set; }
    public string OffsetHex { get; set; } = "";
    public string Hex { get; set; } = "";
    public string Ascii { get; set; } = "";

    // Decoded as common types (little-endian) for first 8 bytes of each row
    public string UInt16s { get; set; } = "";
    public string UInt32s { get; set; } = "";
}

public class RawBinaryFile
{
    private const int RowSize = 16;

    public string FilePath { get; }
    public string FileType { get; }
    public List<RawRow> Rows { get; } = new();

    private RawBinaryFile(string path, string fileType)
    {
        FilePath = path;
        FileType = fileType;
    }

    public static RawBinaryFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        string name = Path.GetFileName(path);
        var file = new RawBinaryFile(path, name);

        for (int off = 0; off < data.Length; off += RowSize)
        {
            int len = Math.Min(RowSize, data.Length - off);
            var row = new RawRow
            {
                Offset = off,
                OffsetHex = $"0x{off:X6}",
            };

            // Hex column
            var hex = new StringBuilder(RowSize * 3);
            var ascii = new StringBuilder(RowSize);
            for (int i = 0; i < len; i++)
            {
                byte b = data[off + i];
                if (i > 0) hex.Append(' ');
                hex.Append(b.ToString("X2"));
                ascii.Append(b >= 0x20 && b < 0x7F ? (char)b : '.');
            }
            row.Hex = hex.ToString();
            row.Ascii = ascii.ToString();

            // UInt16 decode
            var u16 = new StringBuilder();
            for (int i = 0; i + 1 < len; i += 2)
            {
                if (u16.Length > 0) u16.Append(' ');
                ushort v = (ushort)(data[off + i] | (data[off + i + 1] << 8));
                u16.Append(v);
            }
            row.UInt16s = u16.ToString();

            // UInt32 decode
            var u32 = new StringBuilder();
            for (int i = 0; i + 3 < len; i += 4)
            {
                if (u32.Length > 0) u32.Append(' ');
                uint v = (uint)(data[off + i] | (data[off + i + 1] << 8) |
                               (data[off + i + 2] << 16) | (data[off + i + 3] << 24));
                u32.Append(v);
            }
            row.UInt32s = u32.ToString();

            file.Rows.Add(row);
        }

        return file;
    }

    public List<RawRow> ToRows() => Rows;
}
