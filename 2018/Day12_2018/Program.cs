using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var lines = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
var init = new HashSet<int>();
string initStr = lines[0].Split(": ")[1];
for (int i = 0; i < initStr.Length; i++)
    if (initStr[i] == '#') init.Add(i);

var rules = new Dictionary<string, bool>();
foreach (var line in lines.Skip(1))
{
    var parts = line.Split(" => ");
    rules[parts[0]] = parts[1] == "#";
}

HashSet<int> Step(HashSet<int> cur)
{
    int lo = cur.Min() - 2, hi = cur.Max() + 2;
    var nxt = new HashSet<int>();
    for (int pos = lo; pos <= hi; pos++)
    {
        var chars = new char[5];
        for (int d = -2; d <= 2; d++) chars[d + 2] = cur.Contains(pos + d) ? '#' : '.';
        if (rules.TryGetValue(new string(chars), out bool alive) && alive)
            nxt.Add(pos);
    }
    return nxt;
}

var cur1 = new HashSet<int>(init);
for (int i = 0; i < 20; i++) cur1 = Step(cur1);
long part1 = cur1.Sum(x => (long)x);

var cur2 = new HashSet<int>(init);
long prevSum = cur2.Sum(x => (long)x);
long? prevDiff = null;
int stable = 0;
long target = 50_000_000_000L;
long part2 = 0;

for (long gen = 1; gen <= target; gen++)
{
    cur2 = Step(cur2);
    long s = cur2.Sum(x => (long)x);
    long diff = s - prevSum;
    if (prevDiff.HasValue && diff == prevDiff.Value)
    {
        if (++stable >= 10) { part2 = s + (target - gen) * diff; break; }
    }
    else
    {
        stable = 0;
        prevDiff = diff;
    }
    prevSum = s;
}

Console.WriteLine($"Day 12 -> Part 1: {part1} | Part 2: {part2}");