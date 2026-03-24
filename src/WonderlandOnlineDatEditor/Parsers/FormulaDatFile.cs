using System;
using System.Collections.Generic;
using System.IO;

namespace WonderlandOnlineDatEditor.Parsers;

/// <summary>
/// Formula.dat — game formula coefficients.
/// Format: [Count 2B][Padding 6B][Unknown 1B][Doubles...]
/// Doubles are 8-byte IEEE 754 little-endian starting at offset 9.
/// Values are combat/EXP formula parameters (typically 0.09–30.0).
/// </summary>
public class FormulaRow
{
    public int Index { get; set; }
    public int Offset { get; set; }
    public double Value { get; set; }
}

public class FormulaDatFile
{
    private const int DataOffset = 9;

    public string FilePath { get; }
    public ushort Count { get; }
    public List<FormulaRow> Rows { get; } = new();

    private FormulaDatFile(string path, ushort count)
    {
        FilePath = path;
        Count = count;
    }

    public static FormulaDatFile Load(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        ushort count = (ushort)(data[0] | (data[1] << 8));
        var file = new FormulaDatFile(path, count);

        int idx = 0;
        for (int off = DataOffset; off + 8 <= data.Length; off += 8)
        {
            double val = BitConverter.ToDouble(data, off);
            file.Rows.Add(new FormulaRow
            {
                Index = idx++,
                Offset = off,
                Value = val,
            });
        }

        return file;
    }

    public List<FormulaRow> ToRows() => Rows;
}
