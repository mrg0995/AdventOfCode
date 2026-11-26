using System;
using System.IO;

const int size = 7347;
var sat = new int[size + 1, size + 1];

for (int y = 1; y <= size; y++)
{
    int rowSum = 0;
    for (int x = 1; x <= size; x++)
    {
        int rackId = x + 10;
        int p = (rackId * y + size) * rackId;
        int power = (p / 100) % 10 - 5;
        rowSum += power;
        sat[y, x] = sat[y - 1, x] + rowSum;
    }
}

int RectSum(int x1, int y1, int s)
{
    int x2 = x1 + s - 1, y2 = y1 + s - 1;
    return sat[y2, x2] - sat[y1 - 1, x2] - sat[y2, x1 - 1] + sat[y1 - 1, x1 - 1];
}

int best3Val = int.MinValue, b3X = 0, b3Y = 0;
int bestAnyVal = int.MinValue, baX = 0, baY = 0, baS = 0;

for (int s = 1; s <= size; s++)
{
    for (int y = 1; y <= size - s + 1; y++)
    {
        for (int x = 1; x <= size - s + 1; x++)
        {
            int val = RectSum(x, y, s);
            if (s == 3 && val > best3Val) { best3Val = val; b3X = x; b3Y = y; }
            if (val > bestAnyVal) { bestAnyVal = val; baX = x; baY = y; baS = s; }
        }
    }
}

Console.WriteLine($"Day 11 -> Part 1: {b3X},{b3Y} | Part 2: {baX},{baY},{baS}");
