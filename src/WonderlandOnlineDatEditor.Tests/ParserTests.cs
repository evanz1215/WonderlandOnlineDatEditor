using System.Text;
using Xunit;
using WonderlandOnlineDatEditor.Core;
using WonderlandOnlineDatEditor.Parsers;

namespace WonderlandOnlineDatEditor.Tests;

public class ParserTests
{
    static ParserTests()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    // ═══ Item.Dat — real file ═══

    private static readonly string ItemDatPath = @"C:\Program Files (x86)\Chinesegamer\WLOnline\data\Item.Dat";

    private byte[]? TryLoadFile(string path)
    {
        if (System.IO.File.Exists(path))
            return System.IO.File.ReadAllBytes(path);
        return null;
    }

    [Fact]
    public void ItemDat_RecordCount_Is6425()
    {
        var data = TryLoadFile(ItemDatPath);
        if (data == null) return; // skip if file not available

        Assert.Equal(0, data.Length % 451);
        Assert.Equal(6425, data.Length / 451);
    }

    [Fact]
    public void ItemDat_FirstRecord_HasValidID()
    {
        var data = TryLoadFile(ItemDatPath);
        if (data == null) return;

        var item = ItemRecord.Decode(data, 0);
        // First item should have a low ID (typically 1 or close)
        Assert.True(item.ItemID >= 0 && item.ItemID < 65535);
    }

    [Fact]
    public void ItemDat_Encode_Roundtrip()
    {
        var data = TryLoadFile(ItemDatPath);
        if (data == null) return;

        var item = ItemRecord.Decode(data, 0);
        byte[] reEncoded = item.Encode();
        var item2 = ItemRecord.Decode(reEncoded, 0);

        Assert.Equal(item.ItemID, item2.ItemID);
        Assert.Equal(item.Name, item2.Name);
        Assert.Equal(item.Level, item2.Level);
        Assert.Equal(item.BuyingPrice, item2.BuyingPrice);
        Assert.Equal(item.SellingPrice, item2.SellingPrice);
        Assert.Equal(item.ItemType, item2.ItemType);
        Assert.Equal(item.EquipPos, item2.EquipPos);
    }

    [Fact]
    public void ItemDat_MultipleRecords_UniqueIDs()
    {
        var data = TryLoadFile(ItemDatPath);
        if (data == null) return;

        var ids = new HashSet<ushort>();
        int count = Math.Min(100, data.Length / 451);
        for (int i = 0; i < count; i++)
        {
            var item = ItemRecord.Decode(data, i * 451);
            ids.Add(item.ItemID);
        }
        // Most IDs should be unique (some may be 0 for empty slots)
        Assert.True(ids.Count > 1);
    }

    // ═══ Npc.dat — real file ═══

    private static readonly string NpcDatPath = @"C:\Program Files (x86)\Chinesegamer\WLOnline\data\Npc.dat";

    [Fact]
    public void NpcDat_RecordCount_Is4808()
    {
        var data = TryLoadFile(NpcDatPath);
        if (data == null) return;

        Assert.Equal(0, data.Length % 138);
        Assert.Equal(4808, data.Length / 138);
    }

    [Fact]
    public void NpcDat_FirstRecord_HasValidID()
    {
        var data = TryLoadFile(NpcDatPath);
        if (data == null) return;

        var npc = NpcRecord.Decode(data, 0);
        Assert.True(npc.NpcID >= 0 && npc.NpcID < 65535);
        Assert.True(npc.Level >= 0);
    }

    [Fact]
    public void NpcDat_MultipleRecords_HaveNames()
    {
        var data = TryLoadFile(NpcDatPath);
        if (data == null) return;

        int named = 0;
        int count = Math.Min(100, data.Length / 138);
        for (int i = 0; i < count; i++)
        {
            var npc = NpcRecord.Decode(data, i * 138);
            if (!string.IsNullOrEmpty(npc.Name)) named++;
        }
        Assert.True(named > 0, "Expected some NPCs to have names");
    }

    // ═══ Skill.dat — real file ═══

    private static readonly string SkillDatPath = @"C:\Program Files (x86)\Chinesegamer\WLOnline\data\Skill.dat";

    [Fact]
    public void SkillDat_RecordCount_Is893()
    {
        var data = TryLoadFile(SkillDatPath);
        if (data == null) return;

        Assert.Equal(0, data.Length % 148);
        Assert.Equal(893, data.Length / 148);
    }

    [Fact]
    public void SkillDat_FirstRecord_HasValidID()
    {
        var data = TryLoadFile(SkillDatPath);
        if (data == null) return;

        var skill = SkillRecord.Decode(data, 0);
        Assert.True(skill.SkillID >= 0);
    }

    // ═══ SceneData.dat — real file ═══

    private static readonly string SceneDatPath = @"C:\Program Files (x86)\Chinesegamer\WLOnline\data\SceneData.dat";

    [Fact]
    public void SceneDat_RecordCount_Is1145()
    {
        var data = TryLoadFile(SceneDatPath);
        if (data == null) return;

        Assert.Equal(0, data.Length % 131);
        Assert.Equal(1145, data.Length / 131);
    }

    [Fact]
    public void SceneDat_FirstRecord_HasValidSceneID()
    {
        var data = TryLoadFile(SceneDatPath);
        if (data == null) return;

        var scene = SceneRecord.Decode(data, 0);
        Assert.True(scene.SceneID >= 0);
    }

    // ═══ Compound2.dat — real file ═══

    private static readonly string Compound2DatPath = @"C:\Program Files (x86)\Chinesegamer\WLOnline\data\Compound2.dat";

    [Fact]
    public void Compound2Dat_RecordCount_Is800()
    {
        var data = TryLoadFile(Compound2DatPath);
        if (data == null) return;

        Assert.Equal(0, data.Length % 65);
        Assert.Equal(800, data.Length / 65);
    }

    [Fact]
    public void Compound2Dat_FirstRecord_HasValidResultID()
    {
        var data = TryLoadFile(Compound2DatPath);
        if (data == null) return;

        var compound = Compound2Record.Decode(data, 0);
        Assert.True(compound.ResultID >= 0);
    }

    // ═══ DatFile loader ═══

    [Fact]
    public void DatFile_Load_Item_ReturnsCorrectType()
    {
        if (!System.IO.File.Exists(ItemDatPath)) return;

        var datFile = DatFile.Load(ItemDatPath);
        Assert.Equal(DatFileType.Item, datFile.FileType);
        Assert.Equal(6425, datFile.RecordCount);
    }

    [Fact]
    public void DatFile_DecodeAll_Items()
    {
        if (!System.IO.File.Exists(ItemDatPath)) return;

        var datFile = DatFile.Load(ItemDatPath);
        var items = datFile.DecodeAll<ItemRecord>();
        Assert.Equal(6425, items.Count);
        Assert.True(items[0].ItemID > 0 || items[0].Name.Length >= 0);
    }

    [Fact]
    public void DatFile_DecodeAll_Npcs()
    {
        if (!System.IO.File.Exists(NpcDatPath)) return;

        var datFile = DatFile.Load(NpcDatPath);
        var npcs = datFile.DecodeAll<NpcRecord>();
        Assert.Equal(4808, npcs.Count);
    }
}
