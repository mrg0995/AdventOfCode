using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

static class Day08
{
    const int Width = 25;
    const int Height = 6;
    const int LayerSize = Width * Height;

    public static (int Part1, string Part2) Solve(string raw)
    {
        string digits = raw.Trim();
        var layers = new List<string>();
        for (int i = 0; i + LayerSize <= digits.Length; i += LayerSize)
        {
            layers.Add(digits.Substring(i, LayerSize));
        }

        var bestLayer = layers.MinBy(l => l.Count(c => c == '0'))!;
        int p1 = bestLayer.Count(c => c == '1') * bestLayer.Count(c => c == '2');

        var sb = new StringBuilder();
        for (int r = 0; r < Height; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                int idx = r * Width + c;
                char ch = ' ';
                foreach (var layer in layers)
                {
                    if (layer[idx] == '0') { ch = ' '; break; }
                    if (layer[idx] == '1') { ch = '#'; break; }
                }
                sb.Append(ch);
            }
            if (r < Height - 1) sb.AppendLine();
        }

        return (p1, sb.ToString());
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day08_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 8 - Part 1: {p1}\nDay 8 - Part 2:\n{p2}");
    }
}
