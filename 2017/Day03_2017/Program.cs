using System;
using System.Collections.Generic;
using System.IO;

string raw = File.ReadAllText("361527").Trim();
int target = int.Parse(raw);

int part1 = ManhattanDistance(target);
int part2 = FirstStressTestValueGreaterThan(target);

Console.WriteLine($"Parte 1: {part1}");
Console.WriteLine($"Parte 2: {part2}");

static int ManhattanDistance(int target)
{
    if (target <= 1)
    {
        return 0;
    }

    int ring = 0;
    int side = 1;
    while (side * side < target)
    {
        side += 2;
        ring++;
    }

    int maxVal = side * side;
    int step = side - 1;

    int minOffset = int.MaxValue;
    for (int i = 0; i < 4; i++)
    {
        int sideCenter = maxVal - ring - i * step;
        int offset = Math.Abs(target - sideCenter);
        if (offset < minOffset)
        {
            minOffset = offset;
        }
    }

    return ring + minOffset;
}

static int FirstStressTestValueGreaterThan(int target)
{
    var grid = new Dictionary<(int X, int Y), int>
    {
        [(0, 0)] = 1
    };

    int x = 1;
    int y = 0;
    int layer = 1;

    while (true)
    {
        while (y < layer)
        {
            int val = SumNeighbors(grid, x, y);
            grid[(x, y)] = val;
            if (val > target) return val;
            y++;
        }

        while (x > -layer)
        {
            int val = SumNeighbors(grid, x, y);
            grid[(x, y)] = val;
            if (val > target) return val;
            x--;
        }

        while (y > -layer)
        {
            int val = SumNeighbors(grid, x, y);
            grid[(x, y)] = val;
            if (val > target) return val;
            y--;
        }

        while (x <= layer)
        {
            int val = SumNeighbors(grid, x, y);
            grid[(x, y)] = val;
            if (val > target) return val;
            x++;
        }

        layer++;
    }
}

static int SumNeighbors(Dictionary<(int X, int Y), int> grid, int x, int y)
{
    int sum = 0;
    for (int dx = -1; dx <= 1; dx++)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0) continue;
            if (grid.TryGetValue((x + dx, y + dy), out int v))
            {
                sum += v;
            }
        }
    }
    return sum;
}
