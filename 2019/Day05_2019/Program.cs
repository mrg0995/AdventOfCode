using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day05
{
    static List<int> RunIntcode(int[] program, int[] inputs)
    {
        int[] mem = (int[])program.Clone();
        int ip = 0, inIdx = 0;
        var outputs = new List<int>();

        int GetParam(int offset, int modes)
        {
            int divisor = offset switch { 1 => 1, 2 => 10, 3 => 100, _ => 1 };
            int mode = (modes / divisor) % 10;
            int val = mem[ip + offset];
            return mode == 0 ? mem[val] : val;
        }

        while (ip < mem.Length)
        {
            int instr = mem[ip];
            int op = instr % 100;
            int modes = instr / 100;

            if (op == 99) break;

            switch (op)
            {
                case 1:
                case 2:
                case 7:
                case 8:
                    {
                        int a = GetParam(1, modes);
                        int b = GetParam(2, modes);
                        int dest = mem[ip + 3];
                        mem[dest] = op switch
                        {
                            1 => a + b,
                            2 => a * b,
                            7 => a < b ? 1 : 0,
                            8 => a == b ? 1 : 0,
                            _ => 0
                        };
                        ip += 4;
                        break;
                    }
                case 3:
                    mem[mem[ip + 1]] = inputs[inIdx++];
                    ip += 2;
                    break;
                case 4:
                    outputs.Add(GetParam(1, modes));
                    ip += 2;
                    break;
                case 5:
                case 6:
                    {
                        int a = GetParam(1, modes);
                        int b = GetParam(2, modes);
                        if ((op == 5 && a != 0) || (op == 6 && a == 0)) ip = b;
                        else ip += 3;
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Unknown opcode {op} at {ip}");
            }
        }

        return outputs;
    }

    public static (int Part1, int Part2) Solve(string raw)
    {
        int[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();

        int p1 = RunIntcode(program, new[] { 1 })[^1];
        int p2 = RunIntcode(program, new[] { 5 })[^1];
        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day05_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 5 - Part 1: {p1}, Part 2: {p2}");
    }
}
