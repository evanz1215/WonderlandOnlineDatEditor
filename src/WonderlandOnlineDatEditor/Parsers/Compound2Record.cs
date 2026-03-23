namespace WonderlandOnlineDatEditor.Parsers;

using WonderlandOnlineDatEditor.Core;

public class Compound2Record
{
    private static readonly XorKeys Keys = DatFileTypes.Info[DatFileType.Compound2].Keys;

    public ushort ResultID { get; set; }
    public ushort PlanID { get; set; }
    public byte UnknownByte { get; set; }
    public ushort ToolID { get; set; }
    public byte AmountReceived { get; set; }
    public byte UnknownByte0 { get; set; }
    public byte UnknownByte1 { get; set; }
    public byte UnknownByte2 { get; set; }
    public ushort[] MaterialIDs { get; set; } = new ushort[5];
    public byte[] MaterialAmounts { get; set; } = new byte[5];
    public byte UnknownByte3 { get; set; }
    public ushort BuildTime { get; set; }
    public byte UnknownByte4 { get; set; }
    public byte UnknownByte5 { get; set; }
    public byte UnknownByte6 { get; set; }
    public byte UnknownByte7 { get; set; }
    public byte UnknownByte8 { get; set; }
    public byte UnknownByte9 { get; set; }
    public ushort[] UnknownWords { get; set; } = new ushort[5];
    public uint[] UnknownDwords { get; set; } = new uint[5];

    public static Compound2Record Decode(byte[] data, int offset)
    {
        var r = new Compound2Record();
        int ptr = offset;

        r.ResultID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.PlanID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownByte = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.ToolID = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.AmountReceived = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte0 = data[ptr]; ptr++;  // raw, not XOR'd per original code
        r.UnknownByte1 = data[ptr]; ptr++;
        r.UnknownByte2 = data[ptr]; ptr++;
        for (int i = 0; i < 5; i++)
        {
            r.MaterialIDs[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
            r.MaterialAmounts[i] = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        }
        r.UnknownByte3 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.BuildTime = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2;
        r.UnknownByte4 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte5 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte6 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte7 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte8 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        r.UnknownByte9 = XorCodec.DecodeByte(data[ptr], Keys); ptr++;
        for (int i = 0; i < 5; i++) { r.UnknownWords[i] = XorCodec.DecodeWord(XorCodec.ReadUInt16(data, ptr), Keys); ptr += 2; }
        for (int i = 0; i < 5; i++) { r.UnknownDwords[i] = XorCodec.DecodeDWord(XorCodec.ReadUInt32(data, ptr), Keys); ptr += 4; }

        return r;
    }
}
