using System;
using System.IO;

string data = File.ReadAllText("Your_Route").Trim();

long part1 = GetDecompressedLength(data, recursive: false);
long part2 = GetDecompressedLength(data, recursive: true);

Console.WriteLine($"Resultado Parte 1: {part1}");
Console.WriteLine($"Resultado Parte 2: {part2}");

static long GetDecompressedLength(string text, bool recursive)
{
    long length = 0;
    int i = 0;
    int n = text.Length;

    while (i < n)
    {
        if (text[i] == '(')
        {
            int markerEnd = text.IndexOf(')', i);
            string marker = text.Substring(i + 1, markerEnd - (i + 1));
            string[] parts = marker.Split('x');
            int chars = int.Parse(parts[0]);
            int repeat = int.Parse(parts[1]);

            i = markerEnd + 1;

            string sub = text.Substring(i, chars);

            if (recursive)
            {
                length += GetDecompressedLength(sub, true) * repeat;
            }
            else
            {
                length += sub.Length * repeat;
            }

            i += chars;
        }
        else
        {
            length++;
            i++;
        }
    }

    return length;
}