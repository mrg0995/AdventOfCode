using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var lines = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
int ipReg = int.Parse(lines[0].Split(' ')[1]);
var insts = lines.Skip(1).Select(l =>
{
    var p = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    return (Op: p[0], A: int.Parse(p[1]), B: int.Parse(p[2]), C: int.Parse(p[3]));
}).ToArray();

int Exec((string Op, int A, int B, int C) ins, int[] r) => ins.Op switch
{
    "addr" => r[ins.A] + r[ins.B],
    "addi" => r[ins.A] + ins.B,
    "mulr" => r[ins.A] * r[ins.B],
    "muli" => r[ins.A] * ins.B,
    "banr" => r[ins.A] & r[ins.B],
    "bani" => r[ins.A] & ins.B,
    "borr" => r[ins.A] | r[ins.B],
    "bori" => r[ins.A] | ins.B,
    "setr" => r[ins.A],
    "seti" => ins.A,
    "gtir" => ins.A > r[ins.B] ? 1 : 0,
    "gtri" => r[ins.A] > ins.B ? 1 : 0,
    "gtrr" => r[ins.A] > r[ins.B] ? 1 : 0,
    "eqir" => ins.A == r[ins.B] ? 1 : 0,
    "eqri" => r[ins.A] == ins.B ? 1 : 0,
    "eqrr" => r[ins.A] == r[ins.B] ? 1 : 0,
    _ => 0
};

int Run(int initR0, bool fastDivisors)
{
    var r = new int[6];
    r[0] = initR0;
    int ip = 0;

    while (ip >= 0 && ip < insts.Length)
    {
        if (fastDivisors && insIsLoopStart(insts[ip], r, ipReg))
        {
            int target = r.Max();
            int sum = 0;
            for (int d = 1; d <= target; d++)
                if (target % d == 0) sum += d;
            return sum;
        }

        r[ipReg] = ip;
        var ins = insts[ip];
        r[ins.C] = Exec(ins, r);
        ip = r[ipReg] + 1;
    }
    return r[0];
}

static bool insIsLoopStart((string Op, int A, int B, int C) ins, int[] r, int ipReg) =>
    ins.Op == "seti" && ins.A == 1 && ins.B == 4 && r.Max() > 1000;

Console.WriteLine($"Day 19 -> Part 1: {Run(0, false)} | Part 2: {Run(1, true)}");