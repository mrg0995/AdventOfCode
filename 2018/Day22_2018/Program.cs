using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var nums = Regex.Matches(File.ReadAllText("input.txt"), @"\d+").Select(m => int.Parse(m.Value)).ToArray();
int depth = nums[0], tx = nums[1], ty = nums[2];
int maxX = tx + 60, maxY = ty + 60;

var erosion = new int[maxX + 1, maxY + 1];
for (int y = 0; y <= maxY; y++)
{
    for (int x = 0; x <= maxX; x++)
    {
        long geo;
        if ((x == 0 && y == 0) || (x == tx && y == ty)) geo = 0;
        else if (y == 0) geo = x * 16807L;
        else if (x == 0) geo = y * 48271L;
        else geo = (long)erosion[x - 1, y] * erosion[x, y - 1];

        erosion[x, y] = (int)((geo + depth) % 20183);
    }
}

int TypeAt(int x, int y) => erosion[x, y] % 3;

int part1 = 0;
for (int y = 0; y <= ty; y++)
    for (int x = 0; x <= tx; x++)
        part1 += TypeAt(x, y);

bool Allowed(int regionType, int tool) => regionType switch
{
    0 => tool == 1 || tool == 2,
    1 => tool == 1 || tool == 0,
    2 => tool == 2 || tool == 0,
    _ => false
};

var pq = new PriorityQueue<(int Minutes, int X, int Y, int Tool), int>();
var best = new Dictionary<(int X, int Y, int Tool), int>();
pq.Enqueue((0, 0, 0, 2), 0);
int part2 = -1;

int[] dx = [1, -1, 0, 0];
int[] dy = [0, 0, 1, -1];

while (pq.Count > 0)
{
    var (min, x, y, tool) = pq.Dequeue();
    if (x == tx && y == ty && tool == 2)
    {
        part2 = min;
        break;
    }

    var key = (x, y, tool);
    if (best.TryGetValue(key, out int prev) && prev <= min) continue;
    best[key] = min;

    int curType = TypeAt(x, y);
    for (int t = 0; t <= 2; t++)
    {
        if (t != tool && Allowed(curType, t))
            pq.Enqueue((min + 7, x, y, t), min + 7);
    }

    for (int d = 0; d < 4; d++)
    {
        int nx = x + dx[d], ny = y + dy[d];
        if (nx >= 0 && nx <= maxX && ny >= 0 && ny <= maxY && Allowed(TypeAt(nx, ny), tool))
            pq.Enqueue((min + 1, nx, ny, tool), min + 1);
    }
}

Console.WriteLine($"Day 22 -> Part 1: {part1} | Part 2: {part2}");
