namespace WonderlandOnlineDatEditor.Parsers;

using WonderlandOnlineDatEditor.Core;

public class SkillRecord
{
    private static readonly XorKeys Keys = DatFileTypes.Info[DatFileType.Skill].Keys;

    public string Name { get; set; } = "";
    public byte Type { get; set; }
    public ushort SkillID { get; set; }
    public ushort SP { get; set; }
    public byte ElementType { get; set; }
    public ushort Attack { get; set; }
    public byte EffectLayer { get; set; }
    public byte UnknownByte1 { get; set; }
    public byte UnknownByte2 { get; set; }
    public double Decimal1 { get; set; }
    public double Decimal2 { get; set; }
    public byte UnknownByte3 { get; set; }
    public ushort AttackPower { get; set; }
    public byte NumberOfTurns { get; set; }
    public byte Effect { get; set; }
    public byte UnknownByte6 { get; set; }
    public ushort PrecedingSkillID { get; set; }
    public ushort UnknownWord1 { get; set; }
    public ushort UnknownWord2 { get; set; }
    public ushort UnknownWord3 { get; set; }
    public ushort UnknownWord4 { get; set; }
    public ushort UnknownWord5 { get; set; }
    public string Description { get; set; } = "";
    public ushort SkillTableOrder { get; set; }
    public byte Target { get; set; }
    public ushort ImageNumSmall { get; set; }
    public ushort UnknownWord6 { get; set; }
    public byte UnknownByte7 { get; set; }
    public byte MaxSkillLevel { get; set; }
    public byte AdditionalEffect { get; set; }
    public ushort UnknownWord7 { get; set; }
    public byte SkillPattern1 { get; set; }
    public byte SkillPattern2 { get; set; }
    public byte SkillPattern3 { get; set; }
    public byte SkillPattern4 { get; set; }
    public byte TypeOfInjury { get; set; }
    public byte UnknownByte8 { get; set; }
    public byte UnknownByte9 { get; set; }
    public byte UnknownByte10 { get; set; }
    public byte UnknownByte11 { get; set; }
    public ushort VoiceWav { get; set; }
    public ushort UnknownWord8 { get; set; }
    public ushort UnknownWord9 { get; set; }
    public ushort UnknownWord10 { get; set; }
    public ushort UnknownWord11 { get; set; }
    public uint UnknownDword1 { get; set; }
    public uint UnknownDword2 { get; set; }
    public uint UnknownDword3 { get; set; }
    public uint UnknownDword4 { get; set; }
    public uint UnknownDword5 { get; set; }

    public static SkillRecord Decode(byte[] data, int offset)
    {
        var r = new SkillRecord();
        int ptr = offset;

        r.Name = XorCodec.DecodeSkillName(data, ptr); ptr += 21;
        r.Type = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SkillID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.SP = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.ElementType = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Attack = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.EffectLayer = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte1 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Decimal1 = BitConverter.ToDouble(data, ptr); ptr += 8;
        r.Decimal2 = BitConverter.ToDouble(data, ptr); ptr += 8;
        r.UnknownByte3 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.AttackPower = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.NumberOfTurns = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.Effect = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte6 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.PrecedingSkillID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord1 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord2 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord3 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord4 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord5 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.Description = XorCodec.DecodeSkillDescription(data, ptr); ptr += 31;
        r.SkillTableOrder = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.Target = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.ImageNumSmall = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord6 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownByte7 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.MaxSkillLevel = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.AdditionalEffect = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownWord7 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.SkillPattern1 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SkillPattern2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SkillPattern3 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SkillPattern4 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.TypeOfInjury = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte8 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte9 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte10 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte11 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.VoiceWav = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord8 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord9 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord10 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownWord11 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownDword1 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword2 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword3 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword4 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword5 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;

        return r;
    }
}
