using System;
using System.IO;
using System.Numerics;

static class Day22
{
    const long DeckP1 = 10_007L;
    static readonly BigInteger DeckP2 = BigInteger.Parse("119315717514047");
    static readonly BigInteger ShufflesP2 = BigInteger.Parse("101741582076661");

    static long Mod(long a, long m)
    {
        long r = a % m;
        return r < 0 ? r + m : r;
    }

    static BigInteger BigMod(BigInteger a, BigInteger m)
    {
        BigInteger r = a % m;
        return r < BigInteger.Zero ? r + m : r;
    }

    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        return BigInteger.ModPow(BigMod(a, m), m - 2, m);
    }

    static long TrackCard2019(string[] lines, long deckSize)
    {
        long pos = 2019L;
        foreach (var raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line == "deal into new stack")
            {
                pos = Mod(deckSize - 1L - pos, deckSize);
            }
            else if (line.StartsWith("cut "))
            {
                long n = long.Parse(line.Split(' ')[^1]);
                pos = Mod(pos - n, deckSize);
            }
            else if (line.StartsWith("deal with increment "))
            {
                long n = long.Parse(line.Split(' ')[^1]);
                pos = Mod((BigInteger)pos * n, deckSize);
            }
            else
            {
                throw new InvalidOperationException($"Unknown instruction: {line}");
            }
        }
        return pos;
    }

    static long Mod(BigInteger a, long m) => (long)BigMod(a, m);

    static (BigInteger a, BigInteger b) AffineForShuffle(string[] lines, BigInteger m)
    {
        BigInteger a = BigInteger.One;
        BigInteger b = BigInteger.Zero;

        foreach (var raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line == "deal into new stack")
            {
                a = BigMod(-a, m);
                b = BigMod(-b - BigInteger.One, m);
            }
            else if (line.StartsWith("cut "))
            {
                BigInteger n = BigInteger.Parse(line.Split(' ')[^1]);
                b = BigMod(b - n, m);
            }
            else if (line.StartsWith("deal with increment "))
            {
                BigInteger n = BigInteger.Parse(line.Split(' ')[^1]);
                a = BigMod(a * n, m);
                b = BigMod(b * n, m);
            }
            else
            {
                throw new InvalidOperationException($"Unknown instruction: {line}");
            }
        }
        return (a, b);
    }

    static (BigInteger A, BigInteger B) PolyPowMod(BigInteger a, BigInteger b, BigInteger k, BigInteger m)
    {
        BigInteger A = BigInteger.One;
        BigInteger B = BigInteger.Zero;
        BigInteger curA = BigMod(a, m);
        BigInteger curB = BigMod(b, m);
        BigInteger exp = k;

        while (exp > BigInteger.Zero)
        {
            if (!exp.IsEven)
            {
                A = BigMod(curA * A, m);
                B = BigMod(curA * B + curB, m);
            }
            curB = BigMod(curA * curB + curB, m);
            curA = BigMod(curA * curA, m);
            exp >>= 1;
        }
        return (A, B);
    }

    public static (long Part1, BigInteger Part2) Solve(string[] lines)
    {
        long p1 = TrackCard2019(lines, DeckP1);

        var (a, b) = AffineForShuffle(lines, DeckP2);
        var (A, B) = PolyPowMod(a, b, ShufflesP2, DeckP2);
        BigInteger invA = ModInverse(A, DeckP2);
        BigInteger p2 = BigMod(BigMod(new BigInteger(2020) - B, DeckP2) * invA, DeckP2);

        return (p1, p2);
    }

    static void Main()
    {
        string[] lines = File.ReadAllLines("Day22_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 22 - Part 1: {p1}, Part 2: {p2}");
    }
}