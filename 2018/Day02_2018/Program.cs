using System;
using System.IO;
using System.Linq;

var ids = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

int twos = 0, threes = 0;
foreach (var id in ids)
{
    var counts = id.GroupBy(c => c).Select(g => g.Count()).ToHashSet();
    if (counts.Contains(2)) twos++;
    if (counts.Contains(3)) threes++;
}
int part1 = twos * threes;

string part2 = "";
for (int i = 0; i < ids.Length && part2 == ""; i++)
{
    for (int j = i + 1; j < ids.Length; j++)
    {
        var common = ids[i].Zip(ids[j], (a, b) => (a, b))
            .Where(p => p.a == p.b)
            .Select(p => p.a)
            .ToArray();
        if (common.Length == ids[i].Length - 1)
        {
            part2 = new string(common);
            break;
        }
    }
}

Console.WriteLine($"Day 2 -> Part 1: {part1} | Part 2: {part2}");
