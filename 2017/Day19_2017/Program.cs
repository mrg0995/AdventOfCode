using System;
using System.IO;
using System.Text;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var rawLines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath)
    : GetSampleDiagram();

var (letters, steps) = FollowTubes(rawLines);

Console.WriteLine($"Parte 1 - Letras encontradas: {letters}");
Console.WriteLine($"Parte 2 - Total de pasos: {steps}");

static (string Letters, int Steps) FollowTubes(string[] rawLines)
{
    var rows = rawLines.Length;
    var cols = 0;
    foreach (var line in rawLines)
    {
        if (line.Length > cols)
            cols = line.Length;
    }

    var grid = new char[rows, cols];
    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < cols; c++)
        {
            grid[r, c] = c < rawLines[r].Length ? rawLines[r][c] : ' ';
        }
    }

    var startCol = -1;
    for (var c = 0; c < cols; c++)
    {
        if (grid[0, c] == '|')
        {
            startCol = c;
            break;
        }
    }

    if (startCol == -1)
        throw new InvalidOperationException("No se encontró el punto de entrada '|' en la primera fila.");

    var row = 0;
    var col = startCol;
    var dr = 1;
    var dc = 0;

    var letters = new StringBuilder();
    var steps = 0;

    while (row >= 0 && row < rows && col >= 0 && col < cols && grid[row, col] != ' ')
    {
        steps++;
        var current = grid[row, col];

        if (char.IsAsciiLetter(current))
        {
            letters.Append(current);
        }
        else if (current == '+')
        {
            (int candDr, int candDc)[] turns = [(-dc, dr), (dc, -dr)];
            var turned = false;

            foreach (var (nextDr, nextDc) in turns)
            {
                var probeR = row + nextDr;
                var probeC = col + nextDc;

                if (probeR >= 0 && probeR < rows &&
                    probeC >= 0 && probeC < cols &&
                    grid[probeR, probeC] != ' ')
                {
                    dr = nextDr;
                    dc = nextDc;
                    turned = true;
                    break;
                }
            }

            if (!turned)
                break;
        }

        row += dr;
        col += dc;
    }

    return (letters.ToString(), steps);
}

static string[] GetSampleDiagram() =>
[
    "     |          ",
    "     |  +--+    ",
    "     A  |  C    ",
    " F---|----E|--+ ",
    "     |  |  |  D ",
    "     +B-+  +--+ "
];