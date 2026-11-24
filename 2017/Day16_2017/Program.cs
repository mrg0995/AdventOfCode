using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var rawInput = File.Exists(inputPath)
    ? File.ReadAllText(inputPath).Trim()
    : "s1,x3/4,pe/b";

var moves = ParseMoves(rawInput);

var part1Order = Dance(moves, iterations: 1);
var part2Order = Dance(moves, iterations: 1_000_000_000);

Console.WriteLine($"Parte 1 - Orden tras 1 baile: {part1Order}");
Console.WriteLine($"Parte 2 - Orden tras 1,000,000,000 bailes: {part2Order}");

static DanceMove[] ParseMoves(string rawInput)
{
    var tokens = rawInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var moves = new DanceMove[tokens.Length];

    for (var i = 0; i < tokens.Length; i++)
    {
        var token = tokens[i];
        var kind = token[0];
        var payload = token[1..];

        switch (kind)
        {
            case 's':
                moves[i] = new DanceMove(MoveKind.Spin, int.Parse(payload, CultureInfo.InvariantCulture), 0, '\0', '\0');
                break;

            case 'x':
                var posParts = payload.Split('/', 2);
                moves[i] = new DanceMove(
                    MoveKind.Exchange,
                    0,
                    0,
                    '\0',
                    '\0',
                    int.Parse(posParts[0], CultureInfo.InvariantCulture),
                    int.Parse(posParts[1], CultureInfo.InvariantCulture));
                break;

            case 'p':
                var nameParts = payload.Split('/', 2);
                moves[i] = new DanceMove(MoveKind.Partner, 0, 0, nameParts[0][0], nameParts[1][0]);
                break;

            default:
                throw new FormatException($"Movimiento desconocido: '{token}'");
        }
    }

    return moves;
}

static string Dance(DanceMove[] moves, int iterations)
{
    var line = "abcdefghijklmnop".ToCharArray();
    var seenStates = new Dictionary<string, int>();
    var history = new List<string>();

    for (var step = 0; step < iterations; step++)
    {
        var currentState = new string(line);

        if (seenStates.TryGetValue(currentState, out var cycleStart))
        {
            var cycleLength = step - cycleStart;
            var remaining = (iterations - cycleStart) % cycleLength;
            return history[cycleStart + remaining];
        }

        seenStates[currentState] = step;
        history.Add(currentState);

        ApplySingleDance(line, moves);
    }

    return new string(line);
}

static void ApplySingleDance(char[] line, DanceMove[] moves)
{
    var n = line.Length;

    foreach (var move in moves)
    {
        switch (move.Kind)
        {
            case MoveKind.Spin:
                Spin(line, move.SpinCount, n);
                break;

            case MoveKind.Exchange:
                (line[move.IndexA], line[move.IndexB]) = (line[move.IndexB], line[move.IndexA]);
                break;

            case MoveKind.Partner:
                var idxA = Array.IndexOf(line, move.ProgA);
                var idxB = Array.IndexOf(line, move.ProgB);
                (line[idxA], line[idxB]) = (line[idxB], line[idxA]);
                break;
        }
    }
}

static void Spin(char[] line, int count, int length)
{
    count %= length;
    if (count == 0)
        return;

    Array.Reverse(line, 0, length - count);
    Array.Reverse(line, length - count, count);
    Array.Reverse(line, 0, length);
}

enum MoveKind
{
    Spin,
    Exchange,
    Partner
}

readonly record struct DanceMove(
    MoveKind Kind,
    int SpinCount,
    int Dummy,
    char ProgA,
    char ProgB,
    int IndexA = 0,
    int IndexB = 0);
