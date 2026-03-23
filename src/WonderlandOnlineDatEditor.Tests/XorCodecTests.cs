using Xunit;
using WonderlandOnlineDatEditor.Core;

namespace WonderlandOnlineDatEditor.Tests;

public class XorCodecTests
{
    // Item.Dat keys
    private static readonly XorKeys ItemKeys = new(0x9A, 0xEFC3, 0x0B80F4B4, 9);
    // Npc.dat keys
    private static readonly XorKeys NpcKeys = new(0xC8, 0x5209, 0x0BAEB716, 9);

    // ═══ Byte encode/decode roundtrip ═══

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(200)]
    public void Byte_Roundtrip(byte value)
    {
        byte encoded = XorCodec.EncodeByte(value, ItemKeys);
        byte decoded = XorCodec.DecodeByte(encoded, ItemKeys);
        Assert.Equal(value, decoded);
    }

    [Theory]
    [InlineData((ushort)0)]
    [InlineData((ushort)1)]
    [InlineData((ushort)12072)]
    [InlineData((ushort)65000)]
    public void Word_Roundtrip(ushort value)
    {
        ushort encoded = XorCodec.EncodeWord(value, ItemKeys);
        ushort decoded = XorCodec.DecodeWord(encoded, ItemKeys);
        Assert.Equal(value, decoded);
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(1u)]
    [InlineData(100000u)]
    [InlineData(0xFFFFFFu)]
    public void DWord_Roundtrip(uint value)
    {
        uint encoded = XorCodec.EncodeDWord(value, ItemKeys);
        uint decoded = XorCodec.DecodeDWord(encoded, ItemKeys);
        Assert.Equal(value, decoded);
    }

    // ═══ Known decode values (from original code) ═══

    [Fact]
    public void DecodeByte_Item_KnownValue()
    {
        // (0x9A ^ 0x9A) - 9 = 0 - 9 => underflow to 247...
        // Actually: (val ^ 0x9A) - 9, so if raw=0xA3 => (0xA3^0x9A)-9 = 0x39-9 = 48
        byte raw = 0xA3;
        byte decoded = XorCodec.DecodeByte(raw, ItemKeys);
        Assert.Equal(48, decoded);
    }

    [Fact]
    public void DecodeWord_Npc_KnownValue()
    {
        // NPC: (val ^ 0x5209) - 9
        // If NpcID raw = 0x520A => (0x520A ^ 0x5209) - 9 = 3 - 9 => underflow
        // More realistic: raw = 0x5213 => (0x5213 ^ 0x5209) - 9 = 0x1A - 9 = 17
        ushort raw = 0x5213;
        ushort decoded = XorCodec.DecodeWord(raw, NpcKeys);
        Assert.Equal(17, decoded);
    }

    // ═══ ReadUInt16 / ReadUInt32 ═══

    [Fact]
    public void ReadUInt16_LittleEndian()
    {
        byte[] data = { 0x34, 0x12 };
        Assert.Equal((ushort)0x1234, XorCodec.ReadUInt16(data, 0));
    }

    [Fact]
    public void ReadUInt32_LittleEndian()
    {
        byte[] data = { 0x78, 0x56, 0x34, 0x12 };
        Assert.Equal(0x12345678u, XorCodec.ReadUInt32(data, 0));
    }

    [Fact]
    public void WriteUInt16_LittleEndian()
    {
        byte[] data = new byte[2];
        XorCodec.WriteUInt16(data, 0, 0x1234);
        Assert.Equal(0x34, data[0]);
        Assert.Equal(0x12, data[1]);
    }

    [Fact]
    public void WriteUInt32_LittleEndian()
    {
        byte[] data = new byte[4];
        XorCodec.WriteUInt32(data, 0, 0x12345678);
        Assert.Equal(0x78, data[0]);
        Assert.Equal(0x56, data[1]);
        Assert.Equal(0x34, data[2]);
        Assert.Equal(0x12, data[3]);
    }

    // ═══ Different key sets ═══

    [Fact]
    public void DifferentKeys_ProduceDifferentEncoded()
    {
        byte value = 42;
        byte enc1 = XorCodec.EncodeByte(value, ItemKeys);
        byte enc2 = XorCodec.EncodeByte(value, NpcKeys);
        Assert.NotEqual(enc1, enc2);
    }
}
