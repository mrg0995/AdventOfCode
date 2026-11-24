using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

var steps = 371;

var part1Value = SolvePart1(steps, insertions: 2017);
var part2Value = SolvePart2(steps, insertions: 50_000_000);

Console.WriteLine($"Parte 1 - Valor tras 2017: {part1Value}");
Console.WriteLine($"Parte 2 - Valor tras el 0 (50M inserciones): {part2Value}");

static int SolvePart1(int steps, int insertions)
{
    var buffer = new List<int>(insertions + 1) { 0 };
    var currentPos = 0;

    for (var value = 1; value <= insertions; value++)
    {
        currentPos = ((currentPos + steps) % buffer.Count) + 1;
        buffer.Insert(currentPos, value);
    }

    return buffer[(currentPos + 1) % buffer.Count];
}

static int SolvePart2(int steps, int insertions)
{
    var currentPos = 0;
    var valueAfterZero = 0;

    for (var value = 1; value <= insertions; value++)
    {
        currentPos = ((currentPos + steps) % value) + 1;

        if (currentPos == 1)
        {
            valueAfterZero = value;
        }
    }

    return valueAfterZero;
}