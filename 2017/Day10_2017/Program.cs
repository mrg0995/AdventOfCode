using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var rawInput = File.Exists(inputPath)
    ? File.ReadAllText(inputPath).Trim()
    : string.Empty;

var part1Answer = SolvePart1(rawInput);
var part2Answer = ComputeKnotHash(rawInput);

Console.WriteLine($"Parte 1 - Producto de los dos primeros elementos: {part1Answer}");
Console.WriteLine($"Parte 2 - Knot Hash: {part2Answer}");

static int SolvePart1(string rawInput)
{
    var lengths = rawInput
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(int.Parse)
        .ToArray();

    var list = Enumerable.Range(0, 256).ToArray();
    var position = 0;
    var skipSize = 0;

    RunRound(list, lengths, ref position, ref skipSize);

    return list[0] * list[1];
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