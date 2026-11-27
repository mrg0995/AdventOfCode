using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day23
{
    sealed class NicVm
    {
        private readonly Dictionary<long, long> _mem = new();
        private long _ip, _relBase;
        private readonly Queue<long> _inputs = new();
        private readonly List<long> _outBuf = new(3);

        public int IdleReads { get; set; }
        public bool HasPendingInput => _inputs.Count > 0;

        public NicVm(long[] program, long addr)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
            _inputs.Enqueue(addr);
        }

        public void EnqueuePacket(long x, long y)
        {
            _inputs.Enqueue(x);
            _inputs.Enqueue(y);
            IdleReads = 0;
        }

        private long Read(long addr) => _mem.TryGetValue(addr, out var v) ? v : 0L;
        private void Write(long addr, long val) => _mem[addr] = val;

        private static int Mode(int offset, long modes)
        {
            long div = offset switch { 1 => 1L, 2 => 10L, 3 => 100L, _ => 1L };
            return (int)((modes / div) % 10L);
        }

        private long WriteAddr(int offset, long modes)
        {
            int m = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return m switch
            {
                0 => raw,
                2 => _relBase + raw,
                _ => throw new InvalidOperationException($"Invalid write mode {m}")
            };
        }

        private long Param(int offset, long modes)
        {
            int m = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return m switch
            {
                0 => Read(raw),
                1 => raw,
                2 => Read(_relBase + raw),
                _ => throw new InvalidOperationException($"Unknown mode {m}")
            };
        }

        public (long dest, long x, long y)? StepOnce()
        {
            long instr = Read(_ip);
            int op = (int)(instr % 100L);
            long modes = instr / 100L;

            if (op == 99) return null;

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
                    {
                        long val;
                        if (_inputs.Count > 0)
                        {
                            val = _inputs.Dequeue();
                            IdleReads = 0;
                        }
                        else
                        {
                            val = -1L;
                            IdleReads++;
                        }
                        Write(WriteAddr(1, modes), val);
                        _ip += 2;
                        break;
                    }
                case 4:
                    {
                        long outVal = Param(1, modes);
                        _ip += 2;
                        _outBuf.Add(outVal);
                        if (_outBuf.Count == 3)
                        {
                            var pkt = (_outBuf[0], _outBuf[1], _outBuf[2]);
                            _outBuf.Clear();
                            return pkt;
                        }
                        break;
                    }
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

            return null;
        }
    }

    public static (long Part1, long Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        var nics = new NicVm[50];
        for (int i = 0; i < 50; i++) nics[i] = new NicVm(program, i);

        long? p1 = null;
        (long x, long y)? natPkt = null;
        long? lastNatY = null;
        long? p2 = null;

        while (p1 is null || p2 is null)
        {
            bool progressed = false;
            for (int i = 0; i < 50; i++)
            {
                var pkt = nics[i].StepOnce();
                if (pkt is not null)
                {
                    progressed = true;
                    var (dest, x, y) = pkt.Value;
                    if (dest == 255L)
                    {
                        p1 ??= y;
                        natPkt = (x, y);
                    }
                    else if (dest is >= 0 and < 50)
                    {
                        nics[(int)dest].EnqueuePacket(x, y);
                    }
                }
                else if (nics[i].HasPendingInput || nics[i].IdleReads == 0)
                {
                    progressed = true;
                }
            }

            if (!progressed && nics.All(n => !n.HasPendingInput && n.IdleReads >= 2) && natPkt is not null)
            {
                var (nx, ny) = natPkt.Value;
                if (lastNatY is not null && ny == lastNatY.Value && p2 is null)
                {
                    p2 = ny;
                    break;
                }
                lastNatY = ny;
                nics[0].EnqueuePacket(nx, ny);
            }
        }

        return (p1!.Value, p2!.Value);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day23_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 23 - Part 1: {p1}, Part 2: {p2}");
    }
}
