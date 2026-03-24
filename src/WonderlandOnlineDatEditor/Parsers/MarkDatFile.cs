using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// Mark.dat — map marker/quest icon data.
/// Format: 2,177 fixed-size records of 553 bytes each.
/// Each record contains two reversed Big5 text regions:
///   - Name/description around bytes 240-270 (right-aligned, reversed)
///   - Label/category around bytes 500-553 (right-aligned, reversed)
/// </summary>
public class MarkRow
{
    public int Index { get; set; }
    public byte Flag { get; set; }
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
}

public class MarkDatFile
{
    private const int RecordSize = 553;

    public string FilePath { get; }
    public int RecordCount { get; }
    public List<MarkRow> Rows { get; } = new();

    private MarkDatFile(string path, int recordCount)
    {
        FilePath = path;
        RecordCount = recordCount;
    }

    public static MarkDatFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        int totalRecords = data.Length / RecordSize;
        var file = new MarkDatFile(path, totalRecords);

        var big5 = Encoding.GetEncoding("big5");

        for (int i = 0; i < totalRecords; i++)
        {
            int off = i * RecordSize;
            if (off + RecordSize > data.Length) break;

            var row = new MarkRow { Index = i };

            // Flag byte: first non-zero byte in the record (usually byte 0 or 258)
            row.Flag = data[off];
            if (row.Flag == 0)
            {
                // Check around byte 258 for the flag
                for (int j = 250; j < 270 && j < RecordSize; j++)
                {
                    if (data[off + j] != 0) { row.Flag = data[off + j]; break; }
                }
            }

            // Extract text region 1 (Name): roughly bytes 230-275
            row.Name = ExtractReversedBig5(data, off, 230, 275, big5);

            // Extract text region 2 (Label): roughly bytes 490-553
            row.Label = ExtractReversedBig5(data, off, 490, RecordSize, big5);

            // Only add records that have some content
            if (!string.IsNullOrEmpty(row.Name) || !string.IsNullOrEmpty(row.Label) || row.Flag != 0)
                file.Rows.Add(row);
        }

        return file;
    }

    private static string ExtractReversedBig5(byte[] data, int recordOffset, int searchStart, int searchEnd, Encoding big5)
    {
        int absStart = recordOffset + searchStart;
        int absEnd = recordOffset + searchEnd;
        if (absEnd > data.Length) absEnd = data.Length;

        // Find the non-zero region within the search range
        int nzStart = -1, nzEnd = -1;
        for (int j = absStart; j < absEnd; j++)
        {
            if (data[j] != 0)
            {
                if (nzStart == -1) nzStart = j;
                nzEnd = j + 1;
            }
        }

        if (nzStart == -1) return "";

        int len = nzEnd - nzStart;
        byte[] reversed = new byte[len];
        for (int j = 0; j < len; j++)
            reversed[j] = data[nzEnd - 1 - j];

        try
        {
            string decoded = big5.GetString(reversed);
            // Strip known padding patterns
            decoded = decoded.Replace("'s", "").Replace(",挩", "").TrimEnd('\0');
            // Clean up non-printable chars
            var clean = new StringBuilder();
            foreach (char c in decoded)
            {
                if (c >= 0x20) clean.Append(c);
            }
            return clean.ToString().Trim();
        }
        catch
        {
            return "";
        }
    }

    public List<MarkRow> ToRows() => Rows;
}
