using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// A node entry in Ground.MMG — represents one map's collision data.
/// Struct: [NameLen 1B][Name 9B][Unknown 11B][Offset 4B][Length 4B] = 29 bytes
/// The node index is stored at the end of the file, with the last 2 bytes being the entry count.
/// </summary>
public class GroundNode
{
    public string Name { get; set; } = "";
    public byte[] UnknownBytes { get; set; } = new byte[11];
    public uint DataOffset { get; set; }
    public uint DataLength { get; set; }

    // Parsed collision grid (loaded on demand)
    public uint MaxX { get; set; }
    public uint MaxY { get; set; }
    public ushort GridWidth { get; set; }
    public ushort GridHeight { get; set; }
    public byte[,]? Grid { get; set; }
}

/// <summary>
/// Flat row for DataGrid display.
/// </summary>
public class GroundNodeRow
{
    public int Index { get; set; }
    public string Name { get; set; } = "";
    public uint DataOffset { get; set; }
    public uint DataLength { get; set; }
    public uint MaxX { get; set; }
    public uint MaxY { get; set; }
    public ushort GridWidth { get; set; }
    public ushort GridHeight { get; set; }
}

public class GroundMMGFile
{
    private const int NodeSize = 29;

    public List<GroundNode> Nodes { get; } = new();
    public string FilePath { get; }
    private byte[] _data;

    private GroundMMGFile(string path, byte[] data)
    {
        FilePath = path;
        _data = data;
    }

    public static GroundMMGFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        var file = new GroundMMGFile(path, data);

        // Last 2 bytes = entry count
        ushort count = (ushort)(data[data.Length - 2] | (data[data.Length - 1] << 8));
        int indexStart = data.Length - 2 - (count * NodeSize);

        for (int i = 0; i < count; i++)
        {
            int off = indexStart + i * NodeSize;
            var node = new GroundNode();

            byte nameLen = data[off];
            node.Name = Encoding.ASCII.GetString(data, off + 1, Math.Min((int)nameLen, 9)).TrimEnd('\0');
            Array.Copy(data, off + 10, node.UnknownBytes, 0, 11);
            node.DataOffset = BitConverter.ToUInt32(data, off + 21);
            node.DataLength = BitConverter.ToUInt32(data, off + 25);

            // Parse collision grid header
            if (node.DataOffset + 8 < indexStart)
            {
                int doff = (int)node.DataOffset;
                node.MaxX = BitConverter.ToUInt32(data, doff);
                node.MaxY = BitConverter.ToUInt32(data, doff + 4);

                // Skip palette/color entries
                int paletteCount = data[doff + 8];
                int gridHeaderOff = doff + 8 + (paletteCount * 6) + 1;
                if (gridHeaderOff + 4 <= data.Length)
                {
                    node.GridWidth = BitConverter.ToUInt16(data, gridHeaderOff);
                    node.GridHeight = BitConverter.ToUInt16(data, gridHeaderOff + 2);
                }
            }

            file.Nodes.Add(node);
        }

        return file;
    }

    public List<GroundNodeRow> ToRows()
    {
        var rows = new List<GroundNodeRow>(Nodes.Count);
        for (int i = 0; i < Nodes.Count; i++)
        {
            var n = Nodes[i];
            rows.Add(new GroundNodeRow
            {
                Index = i,
                Name = n.Name,
                DataOffset = n.DataOffset,
                DataLength = n.DataLength,
                MaxX = n.MaxX,
                MaxY = n.MaxY,
                GridWidth = n.GridWidth,
                GridHeight = n.GridHeight,
            });
        }
        return rows;
    }
}
