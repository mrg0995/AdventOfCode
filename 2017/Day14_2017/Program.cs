using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

var inputPath = args.Length > 0 ? args[0] : "uugsqrei";
var key = File.Exists(inputPath)
    ? File.ReadAllText(inputPath).Trim()
    : "flqrgnkx";

var grid = BuildDiskGrid(key);

var usedSquares = CountUsedSquares(grid);
var regionCount = CountRegions(grid);

Console.WriteLine($"Parte 1 - Cuadros usados: {usedSquares}");
Console.WriteLine($"Parte 2 - Número de regiones: {regionCount}");

static bool[,] BuildDiskGrid(string key)
{
    const int size = 128;
    var grid = new bool[size, size];

    for (var row = 0; row < size; row++)
    {
        var hash = ComputeKnotHash($"{key}-{row}");
        var col = 0;

        foreach (var hexChar in hash)
        {
            var value = Convert.ToInt32(hexChar.ToString(), 16);
            for (var bit = 3; bit >= 0; bit--)
            {
                grid[row, col++] = ((value >> bit) & 1) == 1;
            }
        }
    }

    return grid;
}

static int CountUsedSquares(bool[,] grid)
{
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);
    var used = 0;

    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < cols; c++)
        {
            if (grid[r, c])
                used++;
        }
    }

    return used;
}

static int CountRegions(bool[,] grid)
{
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);
    var visited = new bool[rows, cols];
    var regions = 0;

    (int dr, int dc)[] directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
    var queue = new Queue<(int Row, int Col)>();

    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < cols; c++)
        {
            if (!grid[r, c] || visited[r, c])
                continue;

            regions++;
            visited[r, c] = true;
            queue.Enqueue((r, c));

            while (queue.Count > 0)
            {
                var (currR, currC) = queue.Dequeue();

                foreach (var (dr, dc) in directions)
                {
                    var nextR = currR + dr;
                    var nextC = currC + dc;

                    if (nextR >= 0 && nextR < rows &&
                        nextC >= 0 && nextC < cols &&
                        grid[nextR, nextC] &&
                        !visited[nextR, nextC])
                    {
                        visited[nextR, nextC] = true;
                        queue.Enqueue((nextR, nextC));
                    }
                }
            }
        }
    }

    return regions;
}

static string ComputeKnotHash(string rawInput)
{
    var suffix = new[] { 17, 31, 73, 47, 23 };
    var lengths = Encoding.ASCII.GetBytes(rawInput)
        .Select(b => (int)b)
        .Concat(suffix)
        .ToArray();

    var list = Enumerable.Range(0, 256).ToArray();
    var position = 0;
    var skipSize = 0;

    for (var round = 0; round < 64; round++)
    {
        RunRound(list, lengths, ref position, ref skipSize);
    }

    var hexBuilder = new StringBuilder(32);
    for (var block = 0; block < 16; block++)
    {
        var denseByte = 0;
        var offset = block * 16;

        for (var i = 0; i < 16; i++)
        {
            denseByte ^= list[offset + i];
        }

        hexBuilder.Append(denseByte.ToString("x2", CultureInfo.InvariantCulture));
    }

    return hexBuilder.ToString();
}

static void RunRound(int[] list, ReadOnlySpan<int> lengths, ref int position, ref int skipSize)
{
    var size = list.Length;

    foreach (var length in lengths)
    {
        if (length > size)
            throw new ArgumentOutOfRangeException(nameof(length), $"La longitud {length} excede el tamaño {size}.");

        for (var i = 0; i < length / 2; i++)
        {
            var leftIndex = (position + i) % size;
            var rightIndex = (position + length - 1 - i) % size;
            (list[leftIndex], list[rightIndex]) = (list[rightIndex], list[leftIndex]);
        }

        position = (position + length + skipSize) % size;
        skipSize++;
    }
}