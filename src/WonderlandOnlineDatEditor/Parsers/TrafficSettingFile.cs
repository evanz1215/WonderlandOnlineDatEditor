using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// A traffic route entry from TrafficSetting.txt.
/// Format: TrafficID,NameBig5Hex,FromScene,FromX,FromY,ToScene,ToX,ToY,Cost,Unknown
/// </summary>
public class TrafficRow
{
    public int Index { get; set; }
    public string Section { get; set; } = "";
    public string RawLine { get; set; } = "";
    public string[] Fields { get; set; } = Array.Empty<string>();

    // Parsed fields for TrafficID section
    public int? TrafficID { get; set; }
    public string? Name { get; set; }
    public int? FromScene { get; set; }
    public int? ToScene { get; set; }
}

public class TrafficSettingFile
{
    public List<TrafficRow> Rows { get; } = new();
    public string FilePath { get; }

    private TrafficSettingFile(string path)
    {
        FilePath = path;
    }

    public static TrafficSettingFile Load(string path)
    {
        var file = new TrafficSettingFile(path);
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        string currentSection = "";
        int index = 0;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                continue;

            // Detect section headers
            if (line.StartsWith("[") || line.EndsWith("]") ||
                line.Equals("TrafficID", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("TrafficCapsuleID", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("TrafficExchangeItem", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = line.Trim('[', ']');
                continue;
            }

            string[] fields = line.Split(',');
            var row = new TrafficRow
            {
                Index = index++,
                Section = currentSection,
                RawLine = line,
                Fields = fields,
            };

            // Parse known fields for TrafficID section
            if (currentSection.Equals("TrafficID", StringComparison.OrdinalIgnoreCase) && fields.Length >= 4)
            {
                if (int.TryParse(fields[0], out int tid))
                    row.TrafficID = tid;
                // Field 1 is hex-encoded Big5 name - try to decode
                row.Name = TryDecodeBig5Hex(fields[1]);
                if (fields.Length >= 3 && int.TryParse(fields[2], out int from))
                    row.FromScene = from;
                if (fields.Length >= 6 && int.TryParse(fields[5], out int to))
                    row.ToScene = to;
            }

            file.Rows.Add(row);
        }

        return file;
    }

    private static string? TryDecodeBig5Hex(string hexStr)
    {
        if (string.IsNullOrEmpty(hexStr)) return null;
        try
        {
            // Each pair of hex chars = 1 byte
            hexStr = hexStr.Trim();
            if (hexStr.Length % 2 != 0) return hexStr;
            byte[] bytes = new byte[hexStr.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hexStr.Substring(i * 2, 2), 16);
            return Encoding.GetEncoding("big5").GetString(bytes).TrimEnd('\0');
        }
        catch
        {
            return hexStr; // Return raw if decode fails
        }
    }

    public List<TrafficRow> ToRows() => Rows;
}
