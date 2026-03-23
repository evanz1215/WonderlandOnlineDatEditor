namespace WonderlandOnlineDatEditor.Core;

public record XorKeys(byte Byte8, ushort Word16, uint DWord32, int Subtract);

public static class XorCodec
{
    public static byte DecodeByte(byte val, XorKeys k) => (byte)((val ^ k.Byte8) - k.Subtract);
    public static ushort DecodeWord(ushort val, XorKeys k) => (ushort)((val ^ k.Word16) - k.Subtract);
    public static uint DecodeDWord(uint val, XorKeys k) => (uint)((val ^ k.DWord32) - k.Subtract);

    public static byte EncodeByte(byte val, XorKeys k) => (byte)((val + k.Subtract) ^ k.Byte8);
    public static ushort EncodeWord(ushort val, XorKeys k) => (ushort)((val + k.Subtract) ^ k.Word16);
    public static uint EncodeDWord(uint val, XorKeys k) => (uint)((val + k.Subtract) ^ k.DWord32);

    public static ushort ReadUInt16(byte[] data, int offset)
        => (ushort)(data[offset] | (data[offset + 1] << 8));

    public static uint ReadUInt32(byte[] data, int offset)
        => (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));

    public static void WriteUInt16(byte[] data, int offset, ushort value)
    {
        data[offset] = (byte)(value & 0xFF);
        data[offset + 1] = (byte)((value >> 8) & 0xFF);
    }

    public static void WriteUInt32(byte[] data, int offset, uint value)
    {
        data[offset] = (byte)(value & 0xFF);
        data[offset + 1] = (byte)((value >> 8) & 0xFF);
        data[offset + 2] = (byte)((value >> 16) & 0xFF);
        data[offset + 3] = (byte)((value >> 24) & 0xFF);
    }

    // Decode Big5 name: first byte is length, then 20 bytes reversed
    public static string DecodeName(byte[] data, int offset)
    {
        int len = data[offset];
        if (len == 0 || len > 20) return string.Empty;
        byte[] nameBytes = new byte[len];
        for (int i = 0; i < len; i++)
            nameBytes[i] = data[offset + (20 - i)];
        return System.Text.Encoding.GetEncoding("big5").GetString(nameBytes).TrimEnd('\0');
    }

    // Encode name back to reversed Big5 format
    public static void EncodeName(byte[] data, int offset, string name)
    {
        byte[] nameBytes = System.Text.Encoding.GetEncoding("big5").GetBytes(name);
        int len = Math.Min(nameBytes.Length, 20);
        // Clear the 21-byte name field
        for (int i = 0; i < 21; i++) data[offset + i] = 0;
        data[offset] = (byte)len;
        for (int i = 0; i < len; i++)
            data[offset + (20 - i)] = nameBytes[i];
    }

    // Decode description (same pattern but 255 bytes, reversed)
    public static string DecodeDescription(byte[] data, int offset)
    {
        int len = data[offset];
        if (len == 0 || len > 254) return string.Empty;
        byte[] descBytes = new byte[len];
        for (int i = 0; i < len; i++)
            descBytes[i] = data[offset + (254 - i)];
        return System.Text.Encoding.GetEncoding("big5").GetString(descBytes).TrimEnd('\0');
    }

    public static void EncodeDescription(byte[] data, int offset, string desc)
    {
        byte[] descBytes = System.Text.Encoding.GetEncoding("big5").GetBytes(desc);
        int len = Math.Min(descBytes.Length, 254);
        for (int i = 0; i < 255; i++) data[offset + i] = 0;
        data[offset] = (byte)len;
        for (int i = 0; i < len; i++)
            data[offset + (254 - i)] = descBytes[i];
    }

    // Decode skill name (20 bytes, swap [i] with [19-i])
    public static string DecodeSkillName(byte[] data, int offset)
    {
        int len = data[offset];
        if (len == 0 || len > 20) return string.Empty;
        // First reverse the 20-byte array in-place (swap pairs)
        byte[] tmp = new byte[20];
        Array.Copy(data, offset + 1, tmp, 0, 20);
        for (int i = 0; i < 10; i++)
            (tmp[i], tmp[19 - i]) = (tmp[19 - i], tmp[i]);
        byte[] nameBytes = new byte[len];
        Array.Copy(tmp, 0, nameBytes, 0, len);
        return System.Text.Encoding.GetEncoding("big5").GetString(nameBytes).TrimEnd('\0');
    }

    // Decode skill description (30 bytes, swap [i] with [29-i])
    public static string DecodeSkillDescription(byte[] data, int offset)
    {
        int len = data[offset];
        if (len == 0 || len > 30) return string.Empty;
        byte[] tmp = new byte[30];
        Array.Copy(data, offset + 1, tmp, 0, 30);
        for (int i = 0; i < 15; i++)
            (tmp[i], tmp[29 - i]) = (tmp[29 - i], tmp[i]);
        byte[] descBytes = new byte[len];
        Array.Copy(tmp, 0, descBytes, 0, len);
        return System.Text.Encoding.GetEncoding("big5").GetString(descBytes).TrimEnd('\0');
    }
}
