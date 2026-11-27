using System;
using System.IO;
using System.Linq;

static class Day02
{
    static int RunIntcode(int[] program, int noun, int verb)
    {
        int[] mem = (int[])program.Clone();
        mem[1] = noun;
        mem[2] = verb;
        int ip = 0;

        while (ip < mem.Length)
        {
            int op = mem[ip];
            if (op == 99) break;

            int a = mem[ip + 1];
            int b = mem[ip + 2];
            int dest = mem[ip + 3];

            if (op == 1) mem[dest] = mem[a] + mem[b];
            else if (op == 2) mem[dest] = mem[a] * mem[b];
            else throw new InvalidOperationException($"Unknown opcode {op} at {ip}");

            ip += 4;
        }

        return mem[0];
    }

    public static (int Part1, int Part2) Solve(string raw)
    {
        int[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();

        int p1 = RunIntcode(program, 12, 2);

        const int target = 19690720;
        int p2 = -1;
        for (int noun = 0; noun < 100 && p2 == -1; noun++)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                if (RunIntcode(program, noun, verb) == target)
                {
                    p2 = 100 * noun + verb;
                    break;
                }
            }
        }

        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day02_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 2 - Part 1: {p1}, Part 2: {p2}");
    }
}
