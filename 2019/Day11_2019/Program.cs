using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

static class Day11
{
    sealed class IntcodeVm
    {
        private readonly Dictionary<long, long> _mem = new();
        private long _ip, _relBase;
        private readonly Queue<long> _inputs = new();
        public bool Halted { get; private set; }

        public IntcodeVm(long[] program)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
        }

        public void EnqueueInput(long v) => _inputs.Enqueue(v);

        private long Read(long addr) => _mem.TryGetValue(addr, out var v) ? v : 0L;
        private void Write(long addr, long v) => _mem[addr] = v;

        private static int Mode(int offset, long modes)
        {
            long div = offset switch { 1 => 1L, 2 => 10L, 3 => 100L, _ => 1L };
            return (int)((modes / div) % 10L);
        }

        private long WriteAddr(int offset, long modes)
        {
            int mode = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return mode switch
            {
                0 => raw,
                2 => _relBase + raw,
                _ => throw new InvalidOperationException($"Invalid write mode {mode}")
            };
        }

        private long Param(int offset, long modes)
        {
            int mode = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return mode switch
            {
                0 => Read(raw),
                1 => raw,
                2 => Read(_relBase + raw),
                _ => throw new InvalidOperationException($"Unknown mode {mode}")
            };
        }

        public long? RunUntilOutputOrHalt()
        {
            while (true)
            {
                long instr = Read(_ip);
                int op = (int)(instr % 100L);
                long modes = instr / 100L;

                if (op == 99) { Halted = true; return null; }

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
                            _ip += 4;
                            break;
                        }
                    case 3:
                        if (_inputs.Count == 0) return null;
                        Write(WriteAddr(1, modes), _inputs.Dequeue());
                        _ip += 2;
                        break;
                    case 4:
                        long outVal = Param(1, modes);
                        _ip += 2;
                        return outVal;
                    case 5:
                    case 6:
                        {
                            long a = Param(1, modes);
                            long b = Param(2, modes);
                            if ((op == 5 && a != 0L) || (op == 6 && a == 0L)) _ip = b;
                            else _ip += 3;
                            break;
                        }
                    case 9:
                        _relBase += Param(1, modes);
                        _ip += 2;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown opcode {op}");
                }
            }
        }
    }

    static Dictionary<(int x, int y), int> RunRobot(long[] program, int startColor)
    {
        var vm = new IntcodeVm(program);
        (int dx, int dy)[] dirs = { (0, -1), (1, 0), (0, 1), (-1, 0) };
        int dIdx = 0, x = 0, y = 0;
        var grid = new Dictionary<(int x, int y), int> { [(0, 0)] = startColor };

        while (!vm.Halted)
        {
            vm.EnqueueInput(grid.TryGetValue((x, y), out var cur) ? cur : 0);
            var color = vm.RunUntilOutputOrHalt();
            if (vm.Halted || color is null) break;
            var turn = vm.RunUntilOutputOrHalt();
            if (turn is null) break;

            grid[(x, y)] = (int)color.Value;
            dIdx = (dIdx + (turn.Value == 1L ? 1 : 3)) % 4;
            x += dirs[dIdx].dx;
            y += dirs[dIdx].dy;
        }

        return grid;
    }

    static string RenderGrid(Dictionary<(int x, int y), int> grid)
    {
        var whites = grid.Where(kv => kv.Value == 1).Select(kv => kv.Key).ToList();
        if (whites.Count == 0) return string.Empty;

        int minX = whites.Min(p => p.x), maxX = whites.Max(p => p.x);
        int minY = whites.Min(p => p.y), maxY = whites.Max(p => p.y);

        var sb = new StringBuilder();
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                sb.Append(grid.TryGetValue((x, y), out var c) && c == 1 ? '#' : ' ');
            }
            if (y < maxY) sb.AppendLine();
        }
        return sb.ToString();
    }

    public static (int Part1, string Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        int p1 = RunRobot(program, 0).Count;
        string p2 = RenderGrid(RunRobot(program, 1));
        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day11_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 11 - Part 1: {p1}\nDay 11 - Part 2:\n{p2}");
    }
}