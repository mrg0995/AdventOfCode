using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day15
{
    sealed class DroidVm
    {
        private Dictionary<long, long> _mem = new();
        private long _ip, _relBase;
        private readonly Queue<long> _inputs = new();

        public DroidVm(long[] program)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
        }

        private DroidVm() { }

        public DroidVm Clone() => new()
        {
            _mem = new Dictionary<long, long>(_mem),
            _ip = _ip,
            _relBase = _relBase
        };

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

        public int SendMove(int cmd)
        {
            _inputs.Enqueue(cmd);
            while (true)
            {
                long instr = Read(_ip);
                int op = (int)(instr % 100L);
                long modes = instr / 100L;

                if (op == 99) return 0;

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
                        Write(WriteAddr(1, modes), _inputs.Dequeue());
                        _ip += 2;
                        break;
                    case 4:
                        int status = (int)Param(1, modes);
                        _ip += 2;
                        return status;
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

    public static (int Part1, int Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        (int cmd, int dx, int dy)[] moves =
        {
            (1, 0, -1),
            (2, 0, 1),
            (3, -1, 0),
            (4, 1, 0)
        };

        var start = (x: 0, y: 0);
        var visited = new HashSet<(int x, int y)> { start };
        var openTiles = new HashSet<(int x, int y)> { start };
        (int x, int y)? oxygenPos = null;
        int distToOxygen = 0;

        var q = new Queue<(int x, int y, int dist, DroidVm vm)>();
        q.Enqueue((0, 0, 0, new DroidVm(program)));

        while (q.Count > 0)
        {
            var (x, y, dist, vm) = q.Dequeue();
            foreach (var (cmd, dx, dy) in moves)
            {
                var nxt = (x + dx, y + dy);
                if (!visited.Add(nxt)) continue;

                var branch = vm.Clone();
                int status = branch.SendMove(cmd);
                if (status == 0) continue;

                openTiles.Add(nxt);
                if (status == 2)
                {
                    oxygenPos = nxt;
                    distToOxygen = dist + 1;
                }
                q.Enqueue((nxt.Item1, nxt.Item2, dist + 1, branch));
            }
        }

        var oxy = oxygenPos ?? throw new InvalidOperationException("Oxygen system not found");

        var fillQ = new Queue<(int x, int y, int mins)>();
        var seenFill = new HashSet<(int x, int y)> { oxy };
        fillQ.Enqueue((oxy.x, oxy.y, 0));
        int maxMinutes = 0;

        while (fillQ.Count > 0)
        {
            var (x, y, mins) = fillQ.Dequeue();
            if (mins > maxMinutes) maxMinutes = mins;

            foreach (var (_, dx, dy) in moves)
            {
                var nxt = (x + dx, y + dy);
                if (openTiles.Contains(nxt) && seenFill.Add(nxt))
                {
                    fillQ.Enqueue((nxt.Item1, nxt.Item2, mins + 1));
                }
            }
        }

        return (distToOxygen, maxMinutes);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day15_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 15 - Part 1: {p1}, Part 2: {p2}");
    }
}