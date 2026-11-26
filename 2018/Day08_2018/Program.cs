using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var nums = File.ReadAllText("input.txt")
    .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
    .Select(int.Parse)
    .ToArray();

int idx = 0;
(int MetaSum, int Value) ParseNode()
{
    int nChildren = nums[idx++], nMeta = nums[idx++];
    int metaSum = 0;
    var childVals = new List<int>(nChildren);

    for (int i = 0; i < nChildren; i++)
    {
        var (cm, cv) = ParseNode();
        metaSum += cm;
        childVals.Add(cv);
    }

    var meta = new int[nMeta];
    for (int i = 0; i < nMeta; i++) meta[i] = nums[idx++];
    metaSum += meta.Sum();

    int val = nChildren == 0
        ? meta.Sum()
        : meta.Where(m => m >= 1 && m <= childVals.Count).Sum(m => childVals[m - 1]);

    return (metaSum, val);
}

var (part1, part2) = ParseNode();
Console.WriteLine($"Day 8 -> Part 1: {part1} | Part 2: {part2}");
