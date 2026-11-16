using System;
using System.Collections.Generic;
using System.IO;

string map = File.ReadAllText(@"C:\Users\ProtoMkUser\Desktop\Proyectos\AdventOfCode\Day01_2016\Day01_2016.txt");
var (part1, part2) = Solve(map);

Console.WriteLine($"Parte 1 - Distancia final: {part1}");
Console.WriteLine($"Parte 2 - Primera intersección repetida: {part2}");

static (int part1, int part2) Solve(string map)
{
    int[] dx = { 0, 1, 0, -1 };
    int[] dy = { 1, 0, -1, 0 };

    int x = 0, y = 0;
    int facing = 0;

    var visited = new HashSet<(int, int)>();
    visited.Add((x, y));

    int? part2Distance = null;

    string[] instructions = map.Split(new[] { ',', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    foreach (var rawInstruction in instructions)
    {
        string instruction = rawInstruction.Trim();
        if (instruction.Length == 0) continue;

        char turn = instruction[0];
        int blocks = int.Parse(instruction.Substring(1));

        if (turn == 'R')
        {
            facing = (facing + 1) % 4;
        }
        else if (turn == 'L')
        {
            facing = (facing - 1 + 4) % 4;
        }

        for (int i = 0; i < blocks; i++)
        {
            x += dx[facing];
            y += dy[facing];

            if (part2Distance == null && !visited.Add((x, y)))
            {
                part2Distance = Math.Abs(x) + Math.Abs(y);
            }
        }
    }

    int part1Distance = Math.Abs(x) + Math.Abs(y);
    return (part1Distance, part2Distance ?? 0);
}