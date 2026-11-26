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

int haltIp = -1, cmpReg = 0;
for (int i = 0; i < insts.Length; i++)
{
    var ins = insts[i];
    if (ins.Op == "eqrr" && (ins.A == 0 || ins.B == 0))
    {
        haltIp = i;
        cmpReg = ins.A == 0 ? ins.B : ins.A;
        break;
    }
}

void Exec((string Op, int A, int B, int C) ins, int[] r)
{
    r[ins.C] = ins.Op switch
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
}

var r = new int[6];
int ip = 0;
var seen = new HashSet<int>();
int? part1 = null;
int lastUnique = 0;

while (ip >= 0 && ip < insts.Length)
{
    var ins = insts[ip];
    r[ipReg] = ip;
    Exec(ins, r);
    ip = r[ipReg] + 1;

    if (ip == haltIp)
    {
        int val = r[cmpReg];
        part1 ??= val;
        if (!seen.Add(val)) break;
        lastUnique = val;
    }
}

Console.WriteLine($"Day 21 -> Part 1: {part1} | Part 2: {lastUnique}");
