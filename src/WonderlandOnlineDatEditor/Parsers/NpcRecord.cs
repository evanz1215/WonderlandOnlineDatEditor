namespace WonderlandOnlineDatEditor.Parsers;

using WonderlandOnlineDatEditor.Core;

public class NpcRecord
{
    private static readonly XorKeys Keys = DatFileTypes.Info[DatFileType.Npc].Keys;

    public string Name { get; set; } = "";
    public byte Type { get; set; }
    public ushort NpcID { get; set; }
    public ushort ImageNum { get; set; }
    public ushort ImageNumSmall { get; set; }
    public uint ColorCode1 { get; set; }
    public uint ColorCode2 { get; set; }
    public uint ColorCode3 { get; set; }
    public uint ColorCode4 { get; set; }
    public byte Catchable { get; set; }
    public byte UnknownByte2 { get; set; }
    public byte UnknownByte3 { get; set; }
    public byte Level { get; set; }
    public uint HP { get; set; }
    public uint SP { get; set; }
    public ushort STR { get; set; }
    public ushort CON { get; set; }
    public ushort INT { get; set; }
    public ushort WIS { get; set; }
    public ushort AGI { get; set; }
    public byte ImageNumEnlarge { get; set; }
    public byte Element { get; set; }
    public ushort[] SkillIDs { get; set; } = new ushort[3];
    public ushort[] DropItemIDs { get; set; } = new ushort[5];
    public byte UnknownByte5 { get; set; }
    public ushort UnknownWord14 { get; set; }
    public ushort UnknownWord15 { get; set; }
    public ushort UnknownWord16 { get; set; }
    public ushort UnknownWord17 { get; set; }
    public byte GeneralAttack1 { get; set; }
    public ushort UnknownWord18 { get; set; }
    public byte GeneralAttack2 { get; set; }
    public byte UnknownByte8 { get; set; }
    public ushort TalkImage { get; set; }
    public ushort UnknownWord20 { get; set; }
    public ushort UnknownWord21 { get; set; }
    public ushort SPD { get; set; }
    public ushort GeneralAttack3 { get; set; }
    public byte UnknownByte9 { get; set; }
    public byte Transferrable { get; set; }
    public byte PK_NPC { get; set; }
    public ushort UnknownWord24 { get; set; }
    public byte UnknownByte12 { get; set; }
    public byte NPCQuestID { get; set; }
    public byte HumanNPC { get; set; }
    public byte UnknownByte15 { get; set; }
    public byte HP_times2 { get; set; }
    public ushort UnknownWord25 { get; set; }
    public ushort Tradeable { get; set; }
    public ushort UnknownWord27 { get; set; }
    public ushort UnknownWord28 { get; set; }
    public ushort UnknownWord29 { get; set; }
    public uint UnknownDword2 { get; set; }
    public uint UnknownDword3 { get; set; }
    public ushort UnknownWord30 { get; set; }

    public static NpcRecord Decode(byte[] data, int offset)
    {
        var r = new NpcRecord();
        int ptr = offset;

        r.Name = XorCodec.DecodeName(data, ptr, boundary: 10); ptr += 21;
        r.Type = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.NpcID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.ImageNum = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.ImageNumSmall = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.ColorCode1 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.ColorCode2 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.ColorCode3 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.ColorCode4 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.Catchable = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte3 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Level = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.HP = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.SP = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.STR = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.CON = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.INT = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.WIS = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.AGI = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.ImageNumEnlarge = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Element = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        for (int i = 0; i < 3; i++) { r.SkillIDs[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        for (int i = 0; i < 5; i++) { r.DropItemIDs[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.UnknownByte5 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownWord14 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord15 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord16 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord17 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.GeneralAttack1 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownWord18 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.GeneralAttack2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte8 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.TalkImage = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord20 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord21 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.SPD = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.GeneralAttack3 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownByte9 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Transferrable = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.PK_NPC = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownWord24 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownByte12 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.NPCQuestID = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.HumanNPC = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte15 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.HP_times2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownWord25 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.Tradeable = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord27 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord28 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord29 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownDword2 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword3 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownWord30 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;

        return r;
    }
}
