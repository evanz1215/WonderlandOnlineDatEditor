using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// A node entry in Wem.MMG — represents one map object/decoration sprite.
/// Struct: [NameLen 1B][Name 9B][Unknown 4B][Offset 4B][Length 4B] = 22 bytes
/// The node index is stored at the end of the file, with the last 2 bytes being the entry count.
/// </summary>
public class WemNode
{
    public string Name { get; set; } = "";
    public byte[] UnknownBytes { get; set; } = new byte[4];
    public uint DataOffset { get; set; }
    public uint DataLength { get; set; }
}

/// <summary>
/// Flat row for DataGrid display.
/// </summary>
public class WemNodeRow
{
    public int Index { get; set; }
    public string Name { get; set; } = "";
    public uint DataOffset { get; set; }
    public uint DataLength { get; set; }
}

public class WemMMGFile
{
    private const int NodeSize = 22;

    public List<WemNode> Nodes { get; } = new();
    public string FilePath { get; }
    private byte[] _data;

    private WemMMGFile(string path, byte[] data)
    {
        FilePath = path;
        _data = data;
    }

    public static WemMMGFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        var file = new WemMMGFile(path, data);

        // Last 2 bytes = entry count
        ushort count = (ushort)(data[data.Length - 2] | (data[data.Length - 1] << 8));
        int indexStart = data.Length - 2 - (count * NodeSize);

        for (int i = 0; i < count; i++)
        {
            int off = indexStart + i * NodeSize;
            var node = new WemNode();

            byte nameLen = data[off];
            node.Name = Encoding.ASCII.GetString(data, off + 1, Math.Min((int)nameLen, 9)).TrimEnd('\0');
            Array.Copy(data, off + 10, node.UnknownBytes, 0, 4);
            node.DataOffset = BitConverter.ToUInt32(data, off + 14);
            node.DataLength = BitConverter.ToUInt32(data, off + 18);

            file.Nodes.Add(node);
        }

        return file;
    }

    public List<WemNodeRow> ToRows()
    {
        var rows = new List<WemNodeRow>(Nodes.Count);
        for (int i = 0; i < Nodes.Count; i++)
        {
            var n = Nodes[i];
            rows.Add(new WemNodeRow
            {
                Index = i,
                Name = n.Name,
                DataOffset = n.DataOffset,
                DataLength = n.DataLength,
            });
        }
        return rows;
    }
}
