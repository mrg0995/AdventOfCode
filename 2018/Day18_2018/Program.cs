using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var grid = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

string[] Step(string[] g)
{
    int h = g.Length, w = g[0].Length;
    var next = new string[h];
    for (int y = 0; y < h; y++)
    {
        var row = new char[w];
        for (int x = 0; x < w; x++)
        {
            int trees = 0, lumber = 0;
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && nx < w && ny >= 0 && ny < h)
                    {
                        if (g[ny][nx] == '|') trees++;
                        else if (g[ny][nx] == '#') lumber++;
                    }
                }

            char c = g[y][x];
            row[x] = c switch
            {
                '.' => trees >= 3 ? '|' : '.',
                '|' => lumber >= 3 ? '#' : '|',
                _ => (lumber >= 1 && trees >= 1) ? '#' : '.'
            };
        }
        next[y] = new string(row);
    }
    return next;
}

int Value(string[] g)
{
    int trees = g.Sum(r => r.Count(c => c == '|'));
    int lumber = g.Sum(r => r.Count(c => c == '#'));
    return trees * lumber;
}

var g1 = (string[])grid.Clone();
for (int i = 0; i < 10; i++) g1 = Step(g1);
int part1 = Value(g1);

var seen = new Dictionary<string, int>();
var g2 = (string[])grid.Clone();
const int target = 1_000_000_000;
for (int minute = 0; minute < target; minute++)
{
    string key = string.Join("\n", g2);
    if (seen.TryGetValue(key, out int start))
    {
        int period = minute - start;
        int rem = (target - minute) % period;
        for (int r = 0; r < rem; r++) g2 = Step(g2);
        break;
    }
    seen[key] = minute;
    g2 = Step(g2);
}

Console.WriteLine($"Day 18 -> Part 1: {part1} | Part 2: {Value(g2)}");