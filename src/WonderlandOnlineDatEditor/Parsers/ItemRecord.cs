namespace WonderlandOnlineDatEditor.Parsers;

using WonderlandOnlineDatEditor.Core;

public class ItemRecord
{
    private static readonly XorKeys Keys = DatFileTypes.Info[DatFileType.Item].Keys;

    public string Name { get; set; } = "";
    public byte ItemType { get; set; }
    public ushort ItemID { get; set; }
    public ushort IconNum { get; set; }
    public ushort LargeIconNum { get; set; }
    public ushort[] EquipImageNum { get; set; } = new ushort[4];
    public ushort[] StatType { get; set; } = new ushort[2];
    public byte UnknownByte1 { get; set; }
    public byte UnknownByte2 { get; set; }
    public ushort[] StatVal { get; set; } = new ushort[2];
    public ushort UnknownVal { get; set; }
    public ushort UnknownVal1 { get; set; }
    public byte UnknownByte3 { get; set; }
    public byte ItemRank { get; set; }
    public byte EquipPos { get; set; }
    public byte SpecialStatus { get; set; }
    public uint[] ColorDef { get; set; } = new uint[16];
    public byte Unused { get; set; }
    public byte Level { get; set; }
    public uint BuyingPrice { get; set; }
    public uint SellingPrice { get; set; }
    public byte EquipLimit { get; set; }
    public ushort Control { get; set; }
    public uint UnknownDWord1 { get; set; }
    public byte SetID { get; set; }
    public uint AntiSeal { get; set; }
    public ushort SkillID { get; set; }
    public ushort[] MaterialTypes { get; set; } = new ushort[5];
    public string Description { get; set; } = "";
    public byte TentWidth { get; set; }
    public byte TentHeight { get; set; }
    public byte TentDepth { get; set; }
    public ushort UnknownWord2 { get; set; }
    public byte InvWidth { get; set; }
    public byte InvHeight { get; set; }
    public byte UnknownByte5 { get; set; }
    public ushort[] InTentImages { get; set; } = new ushort[2];
    public ushort NpcID { get; set; }
    public byte UnknownByte6 { get; set; }
    public byte UnknownByte7 { get; set; }
    public byte UnknownByte8 { get; set; }
    public byte UnknownByte9 { get; set; }
    public byte UnknownByte10 { get; set; }
    public byte UnknownByte11 { get; set; }
    public ushort Duration { get; set; }
    public ushort UnknownWord4 { get; set; }
    public ushort CapsuleForm { get; set; }
    public ushort UnknownWord6 { get; set; }
    public ushort UnknownWord7 { get; set; }
    public uint UnknownDWord2 { get; set; }
    public uint UnknownDWord3 { get; set; }
    public uint UnknownDWord4 { get; set; }
    public ushort UnknownWord8 { get; set; }

    public static ItemRecord Decode(byte[] data, int offset)
    {
        var r = new ItemRecord();
        int ptr = offset;

        r.Name = XorCodec.DecodeName(data, ptr); ptr += 21;
        r.ItemType = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.ItemID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.IconNum = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.LargeIconNum = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        for (int i = 0; i < 4; i++) { r.EquipImageNum[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        for (int i = 0; i < 2; i++) { r.StatType[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.UnknownByte1 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;

        // Special stat XOR: ^0xF4B4 - 109
        var statKeys = new XorKeys(0, 0xF4B4, 0, 109);
        r.StatVal[0] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), statKeys); ptr += 2;
        r.UnknownVal = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), statKeys); ptr += 2;
        r.StatVal[1] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), statKeys); ptr += 2;
        r.UnknownVal1 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), statKeys); ptr += 2;

        r.UnknownByte3 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.ItemRank = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.EquipPos = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SpecialStatus = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        for (int i = 0; i < 16; i++) { r.ColorDef[i] = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4; }
        r.Unused = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Level = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.BuyingPrice = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.SellingPrice = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.EquipLimit = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Control = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownDWord1 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.SetID = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.AntiSeal = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.SkillID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        for (int i = 0; i < 5; i++) { r.MaterialTypes[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.Description = XorCodec.DecodeDescription(data, ptr); ptr += 255;
        r.TentWidth = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.TentHeight = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.TentDepth = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownWord2 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.InvWidth = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.InvHeight = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte5 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        for (int i = 0; i < 2; i++) { r.InTentImages[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.NpcID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownByte6 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte7 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte8 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte9 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte10 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte11 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Duration = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord4 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.CapsuleForm = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord6 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord7 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownDWord2 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDWord3 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDWord4 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownWord8 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;

        return r;
    }

    public byte[] Encode()
    {
        byte[] data = new byte[451];
        int ptr = 0;

        XorCodec.EncodeName(data, ptr, Name); ptr += 21;
        data[ptr] = XorCodec.EncodeByte(ItemType, Keys); ptr++;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(ItemID, Keys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(IconNum, Keys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(LargeIconNum, Keys)); ptr += 2;
        for (int i = 0; i < 4; i++) { XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(EquipImageNum[i], Keys)); ptr += 2; }
        for (int i = 0; i < 2; i++) { XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(StatType[i], Keys)); ptr += 2; }
        data[ptr] = XorCodec.EncodeByte(UnknownByte1, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte2, Keys); ptr++;

        var statKeys = new XorKeys(0, 0xF4B4, 0, 109);
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(StatVal[0], statKeys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownVal, statKeys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(StatVal[1], statKeys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownVal1, statKeys)); ptr += 2;

        data[ptr] = XorCodec.EncodeByte(UnknownByte3, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(ItemRank, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(EquipPos, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(SpecialStatus, Keys); ptr++;
        for (int i = 0; i < 16; i++) { XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(ColorDef[i], Keys)); ptr += 4; }
        data[ptr] = XorCodec.EncodeByte(Unused, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(Level, Keys); ptr++;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(BuyingPrice, Keys)); ptr += 4;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(SellingPrice, Keys)); ptr += 4;
        data[ptr] = XorCodec.EncodeByte(EquipLimit, Keys); ptr++;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(Control, Keys)); ptr += 2;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(UnknownDWord1, Keys)); ptr += 4;
        data[ptr] = XorCodec.EncodeByte(SetID, Keys); ptr++;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(AntiSeal, Keys)); ptr += 4;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(SkillID, Keys)); ptr += 2;
        for (int i = 0; i < 5; i++) { XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(MaterialTypes[i], Keys)); ptr += 2; }
        XorCodec.EncodeDescription(data, ptr, Description); ptr += 255;
        data[ptr] = XorCodec.EncodeByte(TentWidth, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(TentHeight, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(TentDepth, Keys); ptr++;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownWord2, Keys)); ptr += 2;
        data[ptr] = XorCodec.EncodeByte(InvWidth, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(InvHeight, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte5, Keys); ptr++;
        for (int i = 0; i < 2; i++) { XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(InTentImages[i], Keys)); ptr += 2; }
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(NpcID, Keys)); ptr += 2;
        data[ptr] = XorCodec.EncodeByte(UnknownByte6, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte7, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte8, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte9, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte10, Keys); ptr++;
        data[ptr] = XorCodec.EncodeByte(UnknownByte11, Keys); ptr++;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(Duration, Keys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownWord4, Keys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(CapsuleForm, Keys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownWord6, Keys)); ptr += 2;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownWord7, Keys)); ptr += 2;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(UnknownDWord2, Keys)); ptr += 4;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(UnknownDWord3, Keys)); ptr += 4;
        XorCodec.WriteUInt32(data, ptr, XorCodec.EncodeDWord(UnknownDWord4, Keys)); ptr += 4;
        XorCodec.WriteUInt16(data, ptr, XorCodec.EncodeWord(UnknownWord8, Keys)); ptr += 2;

        return data;
    }
}
