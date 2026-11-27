using System;
using System.Collections.Generic;
using System.IO;

static class Day14
{
    record Reaction(long OutQty, (string Chem, long Qty)[] Inputs);

    static (string Chem, long Qty) ParseTerm(string term)
    {
        var parts = term.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return (parts[1], long.Parse(parts[0]));
    }

    public static (long Part1, long Part2) Solve(string[] lines)
    {
        var reactions = new Dictionary<string, Reaction>();
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var sides = line.Split("=>");
            var (outChem, outQty) = ParseTerm(sides[1]);
            var inParts = sides[0].Split(',', StringSplitOptions.RemoveEmptyEntries);
            var inputs = new (string Chem, long Qty)[inParts.Length];
            for (int i = 0; i < inParts.Length; i++) inputs[i] = ParseTerm(inParts[i]);
            reactions[outChem] = new Reaction(outQty, inputs);
        }

        long OreForFuel(long fuelAmount)
        {
            long oreNeeded = 0;
            var surplus = new Dictionary<string, long>();
            var need = new Dictionary<string, long> { ["FUEL"] = fuelAmount };

            while (need.Count > 0)
            {
                string chem = "";
                long req = 0;
                foreach (var kv in need) { chem = kv.Key; req = kv.Value; break; }
                need.Remove(chem);

                if (chem == "ORE")
                {
                    oreNeeded += req;
                    continue;
                }

                surplus.TryGetValue(chem, out long surp);
                if (surp >= req)
                {
                    surplus[chem] = surp - req;
                    continue;
                }

                req -= surp;
                surplus[chem] = 0;

                var rx = reactions[chem];
                long batches = (req + rx.OutQty - 1) / rx.OutQty;
                long produced = batches * rx.OutQty;
                surplus[chem] = produced - req;

                foreach (var (inChem, inQty) in rx.Inputs)
                {
                    need[inChem] = need.GetValueOrDefault(inChem, 0) + batches * inQty;
                }
            }

            return oreNeeded;
        }

        long p1 = OreForFuel(1);

        const long budget = 1_000_000_000_000L;
        long lo = 1, hi = budget, best = 1;
        while (lo <= hi)
        {
            long mid = lo + (hi - lo) / 2;
            if (OreForFuel(mid) <= budget)
            {
                best = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }

        return (p1, best);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day14_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 14 - Part 1: {p1}, Part 2: {p2}");
    }
}