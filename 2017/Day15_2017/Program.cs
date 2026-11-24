using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

const long FactorA = 16807;
const long FactorB = 48271;
const long Modulus = 2147483647;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var (seedA, seedB) = File.Exists(inputPath)
    ? ParseSeeds(File.ReadAllLines(inputPath))
    : (65L, 8921L);

var part1Matches = CountPart1Matches(seedA, seedB, pairs: 40_000_000);
var part2Matches = CountPart2Matches(seedA, seedB, pairs: 5_000_000);

Console.WriteLine($"Parte 1 - Coincidencias (40M pares): {part1Matches}");
Console.WriteLine($"Parte 2 - Coincidencias (5M pares filtrados): {part2Matches}");

static (long SeedA, long SeedB) ParseSeeds(string[] lines)
{
    var numbers = lines
        .Select(line => Regex.Match(line, @"-?\d+"))
        .Where(m => m.Success)
        .Select(m => long.Parse(m.Value))
        .ToArray();

    if (numbers.Length < 2)
        throw new FormatException("No se encontraron las semillas iniciales de los generadores A y B.");

    return (numbers[0], numbers[1]);
}

static long NextValue(long current, long factor)
    => (current * factor) % Modulus;

static int CountPart1Matches(long seedA, long seedB, int pairs)
{
    var a = seedA;
    var b = seedB;
    var matches = 0;

    for (var i = 0; i < pairs; i++)
    {
        a = NextValue(a, FactorA);
        b = NextValue(b, FactorB);

        if ((a & 0xFFFF) == (b & 0xFFFF))
            matches++;
    }

    return matches;
}

static int CountPart2Matches(long seedA, long seedB, int pairs)
{
    var a = seedA;
    var b = seedB;
    var matches = 0;

    for (var i = 0; i < pairs; i++)
    {
        do
        {
            a = NextValue(a, FactorA);
        } while ((a & 3) != 0);

        do
        {
            b = NextValue(b, FactorB);
        } while ((b & 7) != 0);

        if ((a & 0xFFFF) == (b & 0xFFFF))
            matches++;
    }

    return matches;
}