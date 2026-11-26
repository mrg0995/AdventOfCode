using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var ops = new Dictionary<string, Func<int[], int, int, int>>
{
    ["addr"] = (r, a, b) => r[a] + r[b],
    ["addi"] = (r, a, b) => r[a] + b,
    ["mulr"] = (r, a, b) => r[a] * r[b],
    ["muli"] = (r, a, b) => r[a] * b,
    ["banr"] = (r, a, b) => r[a] & r[b],
    ["bani"] = (r, a, b) => r[a] & b,
    ["borr"] = (r, a, b) => r[a] | r[b],
    ["bori"] = (r, a, b) => r[a] | b,
    ["setr"] = (r, a, b) => r[a],
    ["seti"] = (r, a, b) => a,
    ["gtir"] = (r, a, b) => a > r[b] ? 1 : 0,
    ["gtri"] = (r, a, b) => r[a] > b ? 1 : 0,
    ["gtrr"] = (r, a, b) => r[a] > r[b] ? 1 : 0,
    ["eqir"] = (r, a, b) => a == r[b] ? 1 : 0,
    ["eqri"] = (r, a, b) => r[a] == b ? 1 : 0,
    ["eqrr"] = (r, a, b) => r[a] == r[b] ? 1 : 0,
};

string raw = File.ReadAllText("input.txt").Replace("\r\n", "\n");
var sections = raw.Split("\n\n\n");
var sampleBlocks = sections[0].Trim().Split("\n\n");
var progLines = sections.Length > 1
    ? sections[1].Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries)
    : Array.Empty<string>();

int threeOrMore = 0;
var candidates = Enumerable.Range(0, 16).ToDictionary(i => i, _ => ops.Keys.ToHashSet());

foreach (var block in sampleBlocks)
{
    var nums = Regex.Matches(block, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray();
    int[] before = [nums[0], nums[1], nums[2], nums[3]];
    int op = nums[4], a = nums[5], b = nums[6], c = nums[7];
    int[] after = [nums[8], nums[9], nums[10], nums[11]];

    var matches = new HashSet<string>();
    foreach (var (name, fn) in ops)
    {
        var r = (int[])before.Clone();
        r[c] = fn(r, a, b);
        if (r.SequenceEqual(after)) matches.Add(name);
    }

    if (matches.Count >= 3) threeOrMore++;
    candidates[op].IntersectWith(matches);
}

var opMap = new Dictionary<int, string>();
while (opMap.Count < 16)
{
    foreach (var (code, set) in candidates)
    {
        if (set.Count == 1)
        {
            string name = set.First();
            opMap[code] = name;
            foreach (var other in candidates.Values) other.Remove(name);
        }
    }
}

var regs = new int[4];
foreach (var line in progLines)
{
    var p = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    regs[p[3]] = ops[opMap[p[0]]](regs, p[1], p[2]);
}

Console.WriteLine($"Day 16 -> Part 1: {threeOrMore} | Part 2: {regs[0]}");