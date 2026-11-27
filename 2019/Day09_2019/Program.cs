using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day09
{
    sealed class IntcodeVm
    {
        private readonly Dictionary<long, long> _mem = new();
        private long _ip;
        private long _relBase;
        private readonly Queue<long> _inputs = new();
        public List<long> Outputs { get; } = new();

        public IntcodeVm(long[] program, IEnumerable<long>? inputs = null)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
            if (inputs != null)
            {
                foreach (var v in inputs) _inputs.Enqueue(v);
            }
        }

        private long Read(long addr) => _mem.TryGetValue(addr, out var v) ? v : 0L;
        private void Write(long addr, long val) => _mem[addr] = val;

        private static int Mode(int offset, long modes)
        {
            long div = offset switch { 1 => 1L, 2 => 10L, 3 => 100L, _ => 1L };
            return (int)((modes / div) % 10L);
        }

        private long ReadAddr(int offset, long modes)
        {
            int mode = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return mode switch
            {
                0 => raw,
                2 => _relBase + raw,
                _ => throw new InvalidOperationException($"Invalid write mode {mode} at ip={_ip}")
            };
        }

        private long GetParam(int offset, long modes)
        {
            int mode = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return mode switch
            {
                0 => Read(raw),
                1 => raw,
                2 => Read(_relBase + raw),
                _ => throw new InvalidOperationException($"Unknown mode {mode} at ip={_ip}")
            };
        }

        public List<long> Run()
        {
            while (true)
            {
                long instr = Read(_ip);
                int op = (int)(instr % 100L);
                long modes = instr / 100L;

                if (op == 99) return Outputs;

                switch (op)
                {
                    case 1:
                    case 2:
                    case 7:
                    case 8:
                        {
                            long a = GetParam(1, modes);
                            long b = GetParam(2, modes);
                            long dest = ReadAddr(3, modes);
                            long res = op switch
                            {
                                1 => a + b,
                                2 => a * b,
                                7 => a < b ? 1L : 0L,
                                8 => a == b ? 1L : 0L,
                                _ => 0L
                            };
                            Write(dest, res);
                            _ip += 4;
                            break;
                        }
                    case 3:
                        Write(ReadAddr(1, modes), _inputs.Dequeue());
                        _ip += 2;
                        break;
                    case 4:
                        Outputs.Add(GetParam(1, modes));
                        _ip += 2;
                        break;
                    case 5:
                    case 6:
                        {
                            long a = GetParam(1, modes);
                            long b = GetParam(2, modes);
                            if ((op == 5 && a != 0L) || (op == 6 && a == 0L)) _ip = b;
                            else _ip += 3;
                            break;
                        }
                    case 9:
                        _relBase += GetParam(1, modes);
                        _ip += 2;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown opcode {op} at {_ip}");
                }
            }
        }
    }

    public static (long Part1, long Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        long p1 = new IntcodeVm(program, new[] { 1L }).Run()[^1];
        long p2 = new IntcodeVm(program, new[] { 2L }).Run()[^1];
        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day09_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 9 - Part 1: {p1}, Part 2: {p2}");
    }
}