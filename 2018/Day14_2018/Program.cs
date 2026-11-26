using System;
using System.Collections.Generic;
using System.IO;

int n = 825401;
string targetStr = n.ToString();
var target = new int[targetStr.Length];
for (int i = 0; i < targetStr.Length; i++) target[i] = targetStr[i] - '0';

var scores = new List<int> { 3, 7 };
int e1 = 0, e2 = 1;
int? part2 = null;

bool EndsWithTarget()
{
    if (scores.Count < target.Length) return false;
    int offset = scores.Count - target.Length;
    for (int i = 0; i < target.Length; i++)
        if (scores[offset + i] != target[i]) return false;
    return true;
}

while (scores.Count < n + 10 || part2 is null)
{
    int sum = scores[e1] + scores[e2];
    if (sum >= 10)
    {
        scores.Add(1);
        if (part2 is null && EndsWithTarget()) part2 = scores.Count - target.Length;
        scores.Add(sum % 10);
    }
    else
    {
        scores.Add(sum);
    }

    if (part2 is null && EndsWithTarget()) part2 = scores.Count - target.Length;

    e1 = (e1 + 1 + scores[e1]) % scores.Count;
    e2 = (e2 + 1 + scores[e2]) % scores.Count;
}

string part1 = string.Concat(scores.GetRange(n, 10));
Console.WriteLine($"Day 14 -> Part 1: {part1} | Part 2: {part2}");
