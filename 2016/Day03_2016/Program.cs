using System;
using System.IO;
using System.Linq;

string[] lines = File.ReadAllLines(@"Your_Route");

int validPart1 = 0;
foreach (var line in lines)
{
    int[] sides = ParseLine(line);
    if (IsValidTriangle(sides[0], sides[1], sides[2]))
    {
        validPart1++;
    }
}
Console.WriteLine($"Triángulos válidos (Parte 1): {validPart1}");

int validPart2 = 0;
for (int i = 0; i < lines.Length; i += 3)
{
    int[] row1 = ParseLine(lines[i]);
    int[] row2 = ParseLine(lines[i + 1]);
    int[] row3 = ParseLine(lines[i + 2]);

    for (int col = 0; col < 3; col++)
    {
        if (IsValidTriangle(row1[col], row2[col], row3[col]))
        {
            validPart2++;
        }
    }
}
Console.WriteLine($"Triángulos válidos (Parte 2): {validPart2}");

static int[] ParseLine(string line)
{
    return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(int.Parse)
               .ToArray();
}

static bool IsValidTriangle(int a, int b, int c)
{
    int[] sides = { a, b, c };
    Array.Sort(sides);
    return sides[0] + sides[1] > sides[2];
}