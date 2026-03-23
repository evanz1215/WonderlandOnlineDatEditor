using Xunit;
using WonderlandOnlineDatEditor.Core;

namespace WonderlandOnlineDatEditor.Tests;

public class DatFileTypeTests
{
    [Theory]
    [InlineData("Item.Dat", DatFileType.Item)]
    [InlineData("item.dat", DatFileType.Item)]
    [InlineData("Npc.dat", DatFileType.Npc)]
    [InlineData("Skill.dat", DatFileType.Skill)]
    [InlineData("SceneData.dat", DatFileType.Scene)]
    [InlineData("Compound2.dat", DatFileType.Compound2)]
    public void DetectType_ByFileName(string fileName, DatFileType expected)
    {
        var result = DatFileTypes.DetectType(fileName, 1000);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DetectType_UnknownFile_ReturnsNull()
    {
        var result = DatFileTypes.DetectType("Unknown.dat", 999);
        Assert.Null(result);
    }

    [Fact]
    public void Info_ItemRecordSize_Is451()
    {
        Assert.Equal(451, DatFileTypes.Info[DatFileType.Item].RecordSize);
    }

    [Fact]
    public void Info_NpcRecordSize_Is138()
    {
        Assert.Equal(138, DatFileTypes.Info[DatFileType.Npc].RecordSize);
    }

    [Fact]
    public void Info_SkillRecordSize_Is148()
    {
        Assert.Equal(148, DatFileTypes.Info[DatFileType.Skill].RecordSize);
    }

    [Fact]
    public void Info_SceneRecordSize_Is131()
    {
        Assert.Equal(131, DatFileTypes.Info[DatFileType.Scene].RecordSize);
    }

    [Fact]
    public void Info_Compound2RecordSize_Is65()
    {
        Assert.Equal(65, DatFileTypes.Info[DatFileType.Compound2].RecordSize);
    }

    [Fact]
    public void Info_AllTypesHaveKeys()
    {
        foreach (var (type, info) in DatFileTypes.Info)
        {
            Assert.NotNull(info.Keys);
            Assert.True(info.RecordSize > 0);
            Assert.False(string.IsNullOrEmpty(info.FileName));
        }
    }
}
