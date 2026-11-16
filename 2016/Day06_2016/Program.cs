using System;
using System.IO;
using System.Linq;

string[] lines = File.ReadAllLines(@"Your_Route")
                     .Where(l => !string.IsNullOrWhiteSpace(l))
                     .ToArray();

int messageLength = lines[0].Length;
char[] part1 = new char[messageLength];
char[] part2 = new char[messageLength];

for (int i = 0; i < messageLength; i++)
{
    var columnChars = lines.Select(l => l[i]);

    var sortedGroups = columnChars
        .GroupBy(c => c)
        .OrderByDescending(g => g.Count())
        .Select(g => g.Key)
        .ToList();

    part1[i] = sortedGroups.First();
    part2[i] = sortedGroups.Last();
}

Console.WriteLine("Parte 1 (Más común): " + new string(part1));
Console.WriteLine("Parte 2 (Menos común): " + new string(part2));