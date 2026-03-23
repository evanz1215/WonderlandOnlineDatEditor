namespace WonderlandOnlineDatEditor.Parsers;

using WonderlandOnlineDatEditor.Core;

public class SceneRecord
{
    private static readonly XorKeys Keys = DatFileTypes.Info[DatFileType.Scene].Keys;

    public ushort SceneID { get; set; }
    public string Name { get; set; } = "";
    public byte UnknownByte1 { get; set; }
    public byte UnknownByte2 { get; set; }
    public string SoundMediaName { get; set; } = "";
    public byte Control { get; set; }
    public byte UnknownByte3 { get; set; }
    public byte SceneEffects { get; set; }
    public byte UnknownByte4 { get; set; }
    public byte UnknownByte5 { get; set; }
    public ushort SceneID1 { get; set; }
    public ushort[] Words1 { get; set; } = new ushort[9];
    public byte UnknownByte6 { get; set; }
    public byte UnknownByte7 { get; set; }
    public ushort SceneID2 { get; set; }
    public ushort[] Words2 { get; set; } = new ushort[9];
    public byte UnknownByte8 { get; set; }
    public byte UnknownByte9 { get; set; }
    public ushort[] Words3 { get; set; } = new ushort[7];
    public byte InstanceMaxPlayers { get; set; }
    public byte InstanceGuildRestricted { get; set; }
    public byte UnknownByte10 { get; set; }
    public byte UnknownByte11 { get; set; }
    public byte UnknownByte12 { get; set; }
    public ushort InstanceMarkID { get; set; }
    public ushort InstanceLevelRequirement { get; set; }
    public ushort InstanceTimeLimit { get; set; }
    public ushort InstanceCoordinateX { get; set; }
    public ushort InstanceCoordinateY { get; set; }
    public uint UnknownDword1 { get; set; }
    public uint UnknownDword2 { get; set; }
    public uint UnknownDword3 { get; set; }
    public uint UnknownDword4 { get; set; }
    public uint UnknownDword5 { get; set; }

    // Control bit flags
    public bool ProhibitTents => (Control & 0x40) != 0;
    public bool ProhibitTeam => (Control & 0x10) != 0;
    public bool ProhibitStalls => (Control & 0x04) != 0;
    public bool ProhibitPK => (Control & 0x02) != 0;
    public bool NoSpawnReturn => (Control & 0x08) != 0;

    public static SceneRecord Decode(byte[] data, int offset)
    {
        var r = new SceneRecord();
        int ptr = offset;

        r.SceneID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.Name = XorCodec.DecodeSkillName(data, ptr); ptr += 21;
        r.UnknownByte1 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte2 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        // Sound media name: 1 byte length + 7 bytes name
        int sndLen = data[ptr]; ptr++;
        byte[] sndBytes = new byte[7];
        Array.Copy(data, ptr, sndBytes, 0, 7); ptr += 7;
        r.SoundMediaName = System.Text.Encoding.ASCII.GetString(sndBytes, 0, Math.Min(sndLen, 7)).TrimEnd('\0');
        r.Control = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte3 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SceneEffects = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte4 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte5 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SceneID1 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        for (int i = 0; i < 9; i++) { r.Words1[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.UnknownByte6 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte7 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.SceneID2 = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        for (int i = 0; i < 9; i++) { r.Words2[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.UnknownByte8 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte9 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        for (int i = 0; i < 7; i++) { r.Words3[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        r.InstanceMaxPlayers = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.InstanceGuildRestricted = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte10 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte11 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte12 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.InstanceMarkID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.InstanceLevelRequirement = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.InstanceTimeLimit = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.InstanceCoordinateX = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.InstanceCoordinateY = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownDword1 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword2 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword3 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword4 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;
        r.UnknownDword5 = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4;

        return r;
    }
}
