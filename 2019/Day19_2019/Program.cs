using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day19
{
    static long BeamAt(long[] program, long x, long y)
    {
        if (x < 0 || y < 0) return 0L;

        var mem = new Dictionary<long, long>();
        for (int i = 0; i < program.Length; i++) mem[i] = program[i];

        long ip = 0, relBase = 0;
        var inputs = new Queue<long>(new[] { x, y });

        long Read(long addr) => mem.TryGetValue(addr, out var v) ? v : 0L;
        void Write(long addr, long val) => mem[addr] = val;

        int Mode(int offset, long modes)
        {
            long div = offset switch { 1 => 1L, 2 => 10L, 3 => 100L, _ => 1L };
            return (int)((modes / div) % 10L);
        }

        long WriteAddr(int offset, long modes)
        {
            int m = Mode(offset, modes);
            long raw = Read(ip + offset);
            return m switch
            {
                0 => raw,
                2 => relBase + raw,
                _ => throw new InvalidOperationException($"Invalid write mode {m}")
            };
        }

        long Param(int offset, long modes)
        {
            int m = Mode(offset, modes);
            long raw = Read(ip + offset);
            return m switch
            {
                0 => Read(raw),
                1 => raw,
                2 => Read(relBase + raw),
                _ => throw new InvalidOperationException($"Unknown mode {m}")
            };
        }

        while (true)
        {
            long instr = Read(ip);
            int op = (int)(instr % 100L);
            long modes = instr / 100L;

            if (op == 99) return 0L;

            switch (op)
            {
                case 1:
                case 2:
                case 7:
                case 8:
                    {
                        long a = Param(1, modes);
                        long b = Param(2, modes);
                        long dest = WriteAddr(3, modes);
                        Write(dest, op switch
                        {
                            1 => a + b,
                            2 => a * b,
                            7 => a < b ? 1L : 0L,
                            8 => a == b ? 1L : 0L,
                            _ => 0L
                        });
                        ip += 4;
                        break;
                    }
                case 3:
                    Write(WriteAddr(1, modes), inputs.Dequeue());
                    ip += 2;
                    break;
                case 4:
                    return Param(1, modes);
                case 5:
                case 6:
                    {
                        long a = Param(1, modes);
                        long b = Param(2, modes);
                        if ((op == 5 && a != 0L) || (op == 6 && a == 0L)) ip = b;
                        else ip += 3;
                        break;
                    }
                case 9:
                    relBase += Param(1, modes);
                    ip += 2;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown opcode {op}");
            }
        }
    }

    public static (int Part1, long Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        int p1 = 0;
        for (int gridY = 0; gridY < 50; gridY++)
        {
            for (int gridX = 0; gridX < 50; gridX++)
            {
                if (BeamAt(program, gridX, gridY) == 1L) p1++;
            }
        }

        const long size = 100L;
        long beamX = 0L;
        long beamY = 100L;
        long p2 = 0L;

        while (true)
        {
            while (BeamAt(program, beamX, beamY) == 0L) beamX++;

            long topRightX = beamX + size - 1L;
            long topRightY = beamY - (size - 1L);

            if (topRightY >= 0L && BeamAt(program, topRightX, topRightY) == 1L)
            {
                p2 = beamX * 10000L + topRightY;
                break;
            }

            beamY++;
        }

        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day19_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 19 - Part 1: {p1}, Part 2: {p2}");
    }
}