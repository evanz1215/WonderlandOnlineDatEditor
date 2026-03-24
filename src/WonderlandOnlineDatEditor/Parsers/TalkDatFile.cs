using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// Talk.dat — NPC dialogue database.
/// Format: 17,661 fixed-size records of 292 bytes each.
/// Record layout:
///   [TalkID 2B][TextLen 1B][0x00 1B][TextArea 288B]
/// TextArea stores reversed Big5 text, right-aligned with 35-byte trailer:
///   [zeros padding][reversed(Big5_text + 66×5 + EFEC×5 + 350BE007×5)]
/// Record 0 is a header (first uint32 = version/count).
/// </summary>
public class TalkRow
{
    public int Index { get; set; }
    public ushort TalkID { get; set; }
    public int TextLength { get; set; }
    public string Text { get; set; } = "";
}

public class TalkDatFile
{
    private const int RecordSize = 292;
    private const int HeaderSize = 4;
    private const int TextAreaSize = 288;
    private const int TrailerSize = 35; // 5×0x66 + 5×EFEC(2B) + 5×350BE007(4B)

    public string FilePath { get; }
    public int RecordCount { get; }
    public List<TalkRow> Rows { get; } = new();

    private TalkDatFile(string path, int recordCount)
    {
        FilePath = path;
        RecordCount = recordCount;
    }

    public static TalkDatFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        int totalRecords = data.Length / RecordSize;
        var file = new TalkDatFile(path, totalRecords);

        var big5 = Encoding.GetEncoding("big5");

        // Skip record 0 (header), parse records 1..N
        for (int i = 1; i < totalRecords; i++)
        {
            int off = i * RecordSize;
            if (off + RecordSize > data.Length) break;

            ushort talkID = (ushort)(data[off] | (data[off + 1] << 8));
            int textLen = data[off + 2];
            // byte[3] is always 0x00

            string text = "";
            if (textLen > 0 && textLen <= TextAreaSize - TrailerSize)
            {
                // Text area: bytes [off+4 .. off+291]
                // Content is reversed, right-aligned: [zeros][reversed(text + trailer)]
                // The actual text bytes (reversed) are at the END of the text area
                // We need: last (textLen + TrailerSize) bytes of text area, reversed, then take first textLen bytes

                int textAreaStart = off + HeaderSize;
                int contentLen = textLen + TrailerSize;
                int contentStart = textAreaStart + TextAreaSize - contentLen;

                if (contentStart >= textAreaStart && contentStart + contentLen <= off + RecordSize)
                {
                    // Reverse the content bytes
                    byte[] reversed = new byte[contentLen];
                    for (int j = 0; j < contentLen; j++)
                        reversed[j] = data[contentStart + contentLen - 1 - j];

                    // First textLen bytes are the actual Big5 text
                    try
                    {
                        text = big5.GetString(reversed, 0, textLen);
                    }
                    catch
                    {
                        text = "(decode error)";
                    }
                }
            }

            file.Rows.Add(new TalkRow
            {
                Index = i,
                TalkID = talkID,
                TextLength = textLen,
                Text = text,
            });
        }

        return file;
    }

    public List<TalkRow> ToRows() => Rows;
}
