using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

string regex = File.ReadAllText("input.txt").Trim().Trim('^', '$');
var moves = new Dictionary<char, (int Dx, int Dy)>
{
    ['N'] = (0, -1),
    ['S'] = (0, 1),
    ['E'] = (1, 0),
    ['W'] = (-1, 0)
};

var dist = new Dictionary<(int X, int Y), int> { [(0, 0)] = 0 };
var positions = new List<(int X, int Y)> { (0, 0) };
var branchEnds = new HashSet<(int X, int Y)> { (0, 0) };
var stack = new Stack<(List<(int X, int Y)> StartPos, HashSet<(int X, int Y)> Ends)>();

foreach (char ch in regex)
{
    if (moves.TryGetValue(ch, out var m))
    {
        var nextPos = new List<(int X, int Y)>();
        var seenNext = new HashSet<(int X, int Y)>();
        foreach (var (x, y) in positions)
        {
            var np = (x + m.Dx, y + m.Dy);
            int d = dist[(x, y)] + 1;
            if (!dist.TryGetValue(np, out int curD) || d < curD)
                dist[np] = d;
            if (seenNext.Add(np))
                nextPos.Add(np);
        }
        positions = nextPos;
    }
    else if (ch == '(')
    {
        stack.Push((new List<(int X, int Y)>(positions), new HashSet<(int X, int Y)>(branchEnds)));
        branchEnds = new HashSet<(int X, int Y)>(positions);
    }
    else if (ch == '|')
    {
        branchEnds.UnionWith(positions);
        positions = new List<(int X, int Y)>(stack.Peek().StartPos);
    }
    else if (ch == ')')
    {
        branchEnds.UnionWith(positions);
        var (_, outerEnds) = stack.Pop();
        outerEnds.UnionWith(branchEnds);
        branchEnds = outerEnds;
        positions = branchEnds.ToList();
    }
}

int part1 = dist.Values.Max();
int part2 = dist.Values.Count(d => d >= 1000);
Console.WriteLine($"Day 20 -> Part 1: {part1} | Part 2: {part2}");
