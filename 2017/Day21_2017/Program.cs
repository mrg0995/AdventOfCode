using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : Array.Empty<string>();

var rules = ParseRules(lines);

var part1LitPixels = CountLitAfterIterations(rules, iterations: 5);
var part2LitPixels = CountLitAfterIterations(rules, iterations: 18);

Console.WriteLine($"Parte 1 - Píxeles encendidos tras 5 iteraciones: {part1LitPixels}");
Console.WriteLine($"Parte 2 - Píxeles encendidos tras 18 iteraciones: {part2LitPixels}");

static char[,] CreateInitialGrid() => new[,]
{
    { '.', '#', '.' },
    { '.', '.', '#' },
    { '#', '#', '#' }
};

static Dictionary<string, char[,]> ParseRules(string[] lines)
{
    var map = new Dictionary<string, char[,]>();

    foreach (var rawLine in lines)
    {
        var parts = rawLine.Split("=>", StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            throw new FormatException($"Regla inválida: '{rawLine}'");

        var inputMatrix = ParsePattern(parts[0]);
        var outputMatrix = ParsePattern(parts[1]);

        var current = inputMatrix;
        for (var rot = 0; rot < 4; rot++)
        {
            map[SerializePattern(current)] = outputMatrix;
            map[SerializePattern(FlipHorizontal(current))] = outputMatrix;
            current = RotateClockwise(current);
        }
    }

    return map;
}

static char[,] ParsePattern(string pattern)
{
    var rows = pattern.Split('/', StringSplitOptions.TrimEntries);
    var size = rows.Length;
    var grid = new char[size, size];

    for (var r = 0; r < size; r++)
    {
        for (var c = 0; c < size; c++)
        {
            grid[r, c] = rows[r][c];
        }
    }

    return grid;
}

static string SerializePattern(char[,] grid)
{
    var size = grid.GetLength(0);
    var chars = new char[size * size];
    var idx = 0;

    for (var r = 0; r < size; r++)
    {
        for (var c = 0; c < size; c++)
        {
            chars[idx++] = grid[r, c];
        }
    }

    return new string(chars);
}

static char[,] RotateClockwise(char[,] grid)
{
    var size = grid.GetLength(0);
    var rotated = new char[size, size];

    for (var r = 0; r < size; r++)
    {
        for (var c = 0; c < size; c++)
        {
            rotated[c, size - 1 - r] = grid[r, c];
        }
    }

    return rotated;
}

static char[,] FlipHorizontal(char[,] grid)
{
    var size = grid.GetLength(0);
    var flipped = new char[size, size];

    for (var r = 0; r < size; r++)
    {
        for (var c = 0; c < size; c++)
        {
            flipped[r, size - 1 - c] = grid[r, c];
        }
    }

    return flipped;
}

static char[,] ExtractTile(char[,] grid, int startRow, int startCol, int tileSize)
{
    var tile = new char[tileSize, tileSize];
    for (var r = 0; r < tileSize; r++)
    {
        for (var c = 0; c < tileSize; c++)
        {
            tile[r, c] = grid[startRow + r, startCol + c];
        }
    }
    return tile;
}

static void PlaceTile(char[,] targetGrid, char[,] tile, int destRow, int destCol)
{
    var tileSize = tile.GetLength(0);
    for (var r = 0; r < tileSize; r++)
    {
        for (var c = 0; c < tileSize; c++)
        {
            targetGrid[destRow + r, destCol + c] = tile[r, c];
        }
    }
}

static char[,] StepGrid(char[,] grid, Dictionary<string, char[,]> rules)
{
    var currentSize = grid.GetLength(0);
    var inTileSize = (currentSize % 2 == 0) ? 2 : 3;
    var outTileSize = inTileSize + 1;
    var tilesPerSide = currentSize / inTileSize;
    var nextSize = tilesPerSide * outTileSize;

    var nextGrid = new char[nextSize, nextSize];

    for (var tr = 0; tr < tilesPerSide; tr++)
    {
        for (var tc = 0; tc < tilesPerSide; tc++)
        {
            var tile = ExtractTile(grid, tr * inTileSize, tc * inTileSize, inTileSize);
            var key = SerializePattern(tile);

            if (!rules.TryGetValue(key, out var expandedTile))
                throw new InvalidOperationException($"No se encontró ninguna regla para el patrón '{key}'.");

            PlaceTile(nextGrid, expandedTile, tr * outTileSize, tc * outTileSize);
        }
    }

    return nextGrid;
}

static int CountLitAfterIterations(Dictionary<string, char[,]> rules, int iterations)
{
    var grid = CreateInitialGrid();

    for (var i = 0; i < iterations; i++)
    {
        grid = StepGrid(grid, rules);
    }

    var size = grid.GetLength(0);
    var lit = 0;

    for (var r = 0; r < size; r++)
    {
        for (var c = 0; c < size; c++)
        {
            if (grid[r, c] == '#')
                lit++;
        }
    }

    return lit;
}
