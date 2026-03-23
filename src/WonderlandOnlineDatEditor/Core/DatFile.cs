using System.IO;

namespace WonderlandOnlineDatEditor.Core;

using WonderlandOnlineDatEditor.Parsers;

public class DatFile
{
    public string FilePath { get; }
    public DatFileType FileType { get; }
    public DatFileInfo Info { get; }
    public byte[] RawData { get; }
    public int RecordCount { get; }

    private DatFile(string path, DatFileType type, byte[] data)
    {
        FilePath = path;
        FileType = type;
        Info = DatFileTypes.Info[type];
        RawData = data;
        RecordCount = data.Length / Info.RecordSize;
    }

    public static DatFile Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("File not found", path);

        byte[] data = File.ReadAllBytes(path);
        string fileName = Path.GetFileName(path);
        var type = DatFileTypes.DetectType(fileName, data.Length)
            ?? throw new InvalidOperationException($"Unknown dat file type: {fileName} ({data.Length} bytes)");

        var info = DatFileTypes.Info[type];
        if (data.Length % info.RecordSize != 0)
            throw new InvalidOperationException($"File size {data.Length} is not divisible by record size {info.RecordSize}");

        return new DatFile(path, type, data);
    }

    public static DatFile Load(string path, DatFileType forceType)
    {
        byte[] data = File.ReadAllBytes(path);
        var info = DatFileTypes.Info[forceType];
        if (data.Length % info.RecordSize != 0)
            throw new InvalidOperationException($"File size {data.Length} is not divisible by record size {info.RecordSize}");
        return new DatFile(path, forceType, data);
    }

    public List<T> DecodeAll<T>() where T : class
    {
        var list = new List<T>(RecordCount);
        for (int i = 0; i < RecordCount; i++)
        {
            int offset = i * Info.RecordSize;
            object record = FileType switch
            {
                DatFileType.Item => ItemRecord.Decode(RawData, offset),
                DatFileType.Npc => NpcRecord.Decode(RawData, offset),
                DatFileType.Skill => SkillRecord.Decode(RawData, offset),
                DatFileType.Scene => SceneRecord.Decode(RawData, offset),
                DatFileType.Compound2 => Compound2Record.Decode(RawData, offset),
                _ => throw new NotSupportedException()
            };
            list.Add((T)record);
        }
        return list;
    }

    public void Save(string path, byte[] data)
    {
        File.WriteAllBytes(path, data);
    }
}
