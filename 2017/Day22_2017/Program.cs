using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

(int dr, int dc)[] directions = [(-1, 0), (0, 1), (1, 0), (0, -1)];

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : GetSampleGrid();

var part1Infections = SimulatePart1(lines, bursts: 10_000, directions);
var part2Infections = SimulatePart2(lines, bursts: 10_000_000, directions);

Console.WriteLine($"Parte 1 - Ráfagas que causan infección (10,000): {part1Infections}");
Console.WriteLine($"Parte 2 - Ráfagas que causan infección (10,000,000): {part2Infections}");

static HashSet<(int Row, int Col)> ParseInitialInfected(string[] lines)
{
    var infected = new HashSet<(int Row, int Col)>();
    var rows = lines.Length;
    var cols = lines[0].Length;
    var centerR = rows / 2;
    var centerC = cols / 2;

    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < cols; c++)
        {
            if (lines[r][c] == '#')
            {
                infected.Add((r - centerR, c - centerC));
            }
        }
    }

    return infected;
}

static int SimulatePart1(string[] lines, int bursts, (int dr, int dc)[] dirs)
{
    var infected = ParseInitialInfected(lines);
    var row = 0;
    var col = 0;
    var dir = 0;
    var infectionsCaused = 0;

    for (var i = 0; i < bursts; i++)
    {
        var pos = (row, col);

        if (infected.Contains(pos))
        {
            dir = (dir + 1) % 4;
            infected.Remove(pos);
        }
        else
        {
            dir = (dir + 3) % 4;
            infected.Add(pos);
            infectionsCaused++;
        }

        row += dirs[dir].dr;
        col += dirs[dir].dc;
    }

    return infectionsCaused;
}

static int SimulatePart2(string[] lines, int bursts, (int dr, int dc)[] dirs)
{
    var states = new Dictionary<(int Row, int Col), NodeState>();
    var rows = lines.Length;
    var cols = lines[0].Length;
    var centerR = rows / 2;
    var centerC = cols / 2;

    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < cols; c++)
        {
            if (lines[r][c] == '#')
            {
                states[(r - centerR, c - centerC)] = NodeState.Infected;
            }
        }
    }

    var row = 0;
    var col = 0;
    var dir = 0;
    var infectionsCaused = 0;

    for (var i = 0; i < bursts; i++)
    {
        var pos = (row, col);
        var currentState = states.GetValueOrDefault(pos, NodeState.Clean);

        switch (currentState)
        {
            case NodeState.Clean:
                dir = (dir + 3) % 4;
                states[pos] = NodeState.Weakened;
                break;

            case NodeState.Weakened:
                states[pos] = NodeState.Infected;
                infectionsCaused++;
                break;

            case NodeState.Infected:
                dir = (dir + 1) % 4;
                states[pos] = NodeState.Flagged;
                break;

            case NodeState.Flagged:
                dir = (dir + 2) % 4;
                states.Remove(pos);
                break;
        }

        row += dirs[dir].dr;
        col += dirs[dir].dc;
    }

    return infectionsCaused;
}

static string[] GetSampleGrid() =>
[
    "..#",
    "#..",
    "..."
];

enum NodeState
{
    Clean = 0,
    Weakened = 1,
    Infected = 2,
    Flagged = 3
}