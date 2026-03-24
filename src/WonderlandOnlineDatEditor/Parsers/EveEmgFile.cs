using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// Flat row for DataGrid — one row per NPC/object in a map.
/// </summary>
public class EveNpcRow
{
    public ushort MapID { get; set; }
    public ushort SceneID { get; set; }
    public ushort ClickId { get; set; }
    public string Name { get; set; } = "";
    public uint NpcId { get; set; }
    public uint X { get; set; }
    public uint Y { get; set; }
    public byte Rotation { get; set; }
    public int EventCount { get; set; }
    public int WalkStepCount { get; set; }
    public int WalkPatternCount { get; set; }
}

/// <summary>
/// Flat row for DataGrid — one row per Entry/Exit point.
/// </summary>
public class EveEntryRow
{
    public ushort MapID { get; set; }
    public ushort SceneID { get; set; }
    public ushort ClickID { get; set; }
    public string Name { get; set; } = "";
    public uint X { get; set; }
    public uint Y { get; set; }
    public uint TargetDWord1 { get; set; }
    public uint TargetDWord2 { get; set; }
    public uint TargetDWord3 { get; set; }
    public uint TargetDWord4 { get; set; }
}

/// <summary>
/// Flat row for DataGrid — one row per Warp point.
/// </summary>
public class EveWarpRow
{
    public ushort MapID { get; set; }
    public ushort SceneID { get; set; }
    public ushort ClickID { get; set; }
    public string Name { get; set; } = "";
    public ushort TargetMapID { get; set; }
    public uint X { get; set; }
    public uint Y { get; set; }
    public byte NeededToPass { get; set; }
}

/// <summary>
/// Flat row for map summary view.
/// </summary>
public class EveMapRow
{
    public ushort MapID { get; set; }
    public ushort SceneID { get; set; }
    public uint DataOffset { get; set; }
    public ushort DataLength { get; set; }
    public int NpcCount { get; set; }
    public int EntryCount { get; set; }
    public int MiningCount { get; set; }
    public int ItemCount { get; set; }
    public int EventCount { get; set; }
    public int GroupCount { get; set; }
    public int WarpCount { get; set; }
    public int InteractiveCount { get; set; }
    public int BattleCount { get; set; }
    public int PreEventCount { get; set; }
    public int ExtGroupCount { get; set; }
}

public class EveEmgFile
{
    public string FilePath { get; }
    private readonly byte[] _data;

    public List<EveMapRow> Maps { get; } = new();
    public List<EveNpcRow> Npcs { get; } = new();
    public List<EveWarpRow> Warps { get; } = new();
    public List<EveEntryRow> Entries { get; } = new();

    private EveEmgFile(string path, byte[] data)
    {
        FilePath = path;
        _data = data;
    }

    public static EveEmgFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        var file = new EveEmgFile(path, data);
        file.Parse();
        return file;
    }

    private void Parse()
    {
        int ptr = 8; // skip 8-byte header
        uint entryCount = ReadUInt32(ptr); ptr += 4;

        // Stage 1: read map entries
        var mapEntries = new List<(ushort mapID, ushort sceneID, uint dataPtr, ushort dataLen)>();
        for (int i = 0; i < entryCount; i++)
        {
            ushort mapID = ReadUInt16(ptr); ptr += 2;
            ushort sceneID = ReadUInt16(ptr); ptr += 2;
            uint dataPtr = ReadUInt32(ptr); ptr += 4;
            ushort dataLen = ReadUInt16(ptr); ptr += 2;
            mapEntries.Add((mapID, sceneID, dataPtr, dataLen));
        }

        // Stage 2 & 3: read category offsets and data for each map
        foreach (var (mapID, sceneID, dataPtr, dataLen) in mapEntries)
        {
            var row = new EveMapRow
            {
                MapID = mapID,
                SceneID = sceneID,
                DataOffset = dataPtr,
                DataLength = dataLen,
            };

            try
            {
                // Category offsets are at the end of each map's data block (44 bytes = 11 × uint32)
                int offsetBase = (int)(dataPtr + dataLen - 44);
                if (offsetBase < 0 || offsetBase + 44 > _data.Length) { Maps.Add(row); continue; }

                uint offNpc = ReadUInt32(offsetBase);
                uint offEntry = ReadUInt32(offsetBase + 4);
                uint offMining = ReadUInt32(offsetBase + 8);
                uint offItems = ReadUInt32(offsetBase + 12);
                uint offEvents = ReadUInt32(offsetBase + 16);
                uint offGroups = ReadUInt32(offsetBase + 20);
                uint offWarp = ReadUInt32(offsetBase + 24);
                uint offInteractive = ReadUInt32(offsetBase + 28);
                uint offBattle = ReadUInt32(offsetBase + 32);
                uint offPreEvent = ReadUInt32(offsetBase + 36);
                uint offGroupExt = ReadUInt32(offsetBase + 40);

                // Read counts for each category
                row.NpcCount = SafeReadCount(dataPtr, offNpc);
                row.EntryCount = SafeReadCount(dataPtr, offEntry);
                row.MiningCount = SafeReadCount(dataPtr, offMining);
                row.ItemCount = SafeReadCount(dataPtr, offItems);
                row.EventCount = SafeReadCount(dataPtr, offEvents);
                row.GroupCount = SafeReadCount(dataPtr, offGroups);
                row.WarpCount = SafeReadCount(dataPtr, offWarp);
                row.InteractiveCount = SafeReadCount(dataPtr, offInteractive);
                row.BattleCount = SafeReadCount(dataPtr, offBattle);
                row.PreEventCount = SafeReadCount(dataPtr, offPreEvent);
                row.ExtGroupCount = SafeReadCount(dataPtr, offGroupExt);

                // Parse NPC entries
                ParseNpcs(mapID, sceneID, dataPtr, offNpc);
                // Parse Entry/Exit points
                ParseEntries(mapID, sceneID, dataPtr, offEntry);
                // Parse Warp points
                ParseWarps(mapID, sceneID, dataPtr, offWarp);
            }
            catch { /* skip broken map data */ }

            Maps.Add(row);
        }
    }

    private int SafeReadCount(uint dataPtr, uint categoryOffset)
    {
        int abs = (int)(dataPtr + categoryOffset);
        if (abs < 0 || abs + 2 > _data.Length) return 0;
        return ReadUInt16(abs);
    }

    private void ParseNpcs(ushort mapID, ushort sceneID, uint dataPtr, uint offNpc)
    {
        int ptr = (int)(dataPtr + offNpc);
        if (ptr + 2 > _data.Length) return;
        ushort count = ReadUInt16(ptr); ptr += 2;

        for (int i = 0; i < count; i++)
        {
            try
            {
                if (ptr + 2 > _data.Length) break;
                var row = new EveNpcRow { MapID = mapID, SceneID = sceneID };
                row.ClickId = ReadUInt16(ptr); ptr += 2;

                // Name: 1 byte length + 19 bytes data = 20 bytes total
                int nameLen = _data[ptr];
                row.Name = nameLen > 0 && nameLen <= 19
                    ? Encoding.ASCII.GetString(_data, ptr + 1, Math.Min(nameLen, 19)).TrimEnd('\0')
                    : "";
                ptr += 20;

                byte unknownByte1 = _data[ptr]; ptr++;
                row.X = ReadUInt32(ptr); ptr += 4;
                row.Y = ReadUInt32(ptr); ptr += 4;

                // Variable-length event array
                int evtLen = _data[ptr]; ptr++;
                row.EventCount = evtLen;
                ptr += evtLen;

                // Variable-length unknown array
                int unkLen = _data[ptr]; ptr++;
                ptr += unkLen;

                ptr++; // unknownbyte2
                row.NpcId = ReadUInt32(ptr); ptr += 4;
                row.Rotation = _data[ptr]; ptr++;
                ptr++; // unknownbyte4
                ptr++; // unknownbyte5

                // Walk steps
                int walkCount = _data[ptr]; ptr++;
                row.WalkStepCount = walkCount;
                ptr += walkCount * 12; // each step: x(4) + y(4) + delay(4)

                ptr += 2; // unknownbyte6, unknownbyte7
                ptr += 8; // unknowndword1, unknowndword2
                ptr += 3; // unknownbyte8-10

                // Walk patterns
                int patternCount = _data[ptr]; ptr++;
                row.WalkPatternCount = patternCount;
                for (int p = 0; p < patternCount; p++)
                {
                    ptr += 4; // 4 bytes header
                    ptr += 8; // 2 dwords
                    ptr += 80; // 10 × (x(4) + y(4))
                }

                ptr += 8; // 4 words

                Npcs.Add(row);
            }
            catch { break; }
        }
    }

    private void ParseEntries(ushort mapID, ushort sceneID, uint dataPtr, uint offEntry)
    {
        int ptr = (int)(dataPtr + offEntry);
        if (ptr + 2 > _data.Length) return;
        ushort count = ReadUInt16(ptr); ptr += 2;

        for (int i = 0; i < count; i++)
        {
            try
            {
                if (ptr + 2 > _data.Length) break;
                var row = new EveEntryRow { MapID = mapID, SceneID = sceneID };
                row.ClickID = ReadUInt16(ptr); ptr += 2;

                // Name: 20 bytes raw
                row.Name = Encoding.ASCII.GetString(_data, ptr, 20).TrimEnd('\0');
                ptr += 20;

                ptr++; // unknownbyte1
                row.X = ReadUInt32(ptr); ptr += 4;
                row.Y = ReadUInt32(ptr); ptr += 4;

                int blen1 = _data[ptr]; ptr++; ptr += blen1;
                int blen2 = _data[ptr]; ptr++; ptr += blen2;
                ptr++; // unknownbyte2

                row.TargetDWord1 = ReadUInt32(ptr); ptr += 4;
                row.TargetDWord2 = ReadUInt32(ptr); ptr += 4;
                ptr++; // unknownbyte3
                row.TargetDWord3 = ReadUInt32(ptr); ptr += 4;
                row.TargetDWord4 = ReadUInt32(ptr); ptr += 4;

                Entries.Add(row);
            }
            catch { break; }
        }
    }

    private void ParseWarps(ushort mapID, ushort sceneID, uint dataPtr, uint offWarp)
    {
        int ptr = (int)(dataPtr + offWarp);
        if (ptr + 2 > _data.Length) return;
        ushort count = ReadUInt16(ptr); ptr += 2;

        for (int i = 0; i < count; i++)
        {
            try
            {
                if (ptr + 2 > _data.Length) break;
                var row = new EveWarpRow { MapID = mapID, SceneID = sceneID };
                row.ClickID = ReadUInt16(ptr); ptr += 2;

                row.Name = Encoding.ASCII.GetString(_data, ptr, 20).TrimEnd('\0');
                ptr += 20;

                row.TargetMapID = ReadUInt16(ptr); ptr += 2;
                row.X = ReadUInt32(ptr); ptr += 4;
                row.Y = ReadUInt32(ptr); ptr += 4;
                ptr++; // unknownbyte1
                row.NeededToPass = _data[ptr]; ptr++;
                ptr++; // unknownbyte3

                Warps.Add(row);
            }
            catch { break; }
        }
    }

    /// <summary>Default view: map summary rows.</summary>
    public List<EveMapRow> ToRows() => Maps;

    private ushort ReadUInt16(int offset)
        => (ushort)(_data[offset] | (_data[offset + 1] << 8));

    private uint ReadUInt32(int offset)
        => (uint)(_data[offset] | (_data[offset + 1] << 8) | (_data[offset + 2] << 16) | (_data[offset + 3] << 24));
}
