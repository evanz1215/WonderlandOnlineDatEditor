using System.IO;

namespace WonderlandOnlineDatEditor.Core;

public enum DatFileType
{
    Item,
    Npc,
    Skill,
    Scene,
    Compound2
}

public static class DatFileTypes
{
    public static readonly Dictionary<DatFileType, DatFileInfo> Info = new()
    {
        [DatFileType.Item] = new("Item.Dat", 451, new XorKeys(0x9A, 0xEFC3, 0x0B80F4B4, 9)),
        [DatFileType.Npc] = new("Npc.dat", 138, new XorKeys(0xC8, 0x5209, 0x0BAEB716, 9)),
        [DatFileType.Skill] = new("Skill.dat", 148, new XorKeys(0xFD, 0x6EA0, 0x0BDEDEBF, 4)),
        [DatFileType.Scene] = new("SceneData.dat", 131, new XorKeys(0x2C, 0xEA6C, 0x062B7BA7, 9)),
        [DatFileType.Compound2] = new("Compound2.dat", 65, new XorKeys(0xD3, 0xFBBC, 0x0A06F965, 3)),
    };

    public static DatFileType? DetectType(string fileName, long fileSize)
    {
        string name = Path.GetFileName(fileName);
        foreach (var (type, info) in Info)
        {
            if (string.Equals(name, info.FileName, StringComparison.OrdinalIgnoreCase))
                return type;
        }
        // Fallback: try by record size divisibility
        foreach (var (type, info) in Info)
        {
            if (fileSize > 0 && fileSize % info.RecordSize == 0)
                return type;
        }
        return null;
    }
}

public record DatFileInfo(string FileName, int RecordSize, XorKeys Keys);
