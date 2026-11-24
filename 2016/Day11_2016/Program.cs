using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

record Pair(byte Gen, byte Chip) : IComparable<Pair>
{
    public int CompareTo(Pair? other)
    {
        if (other is null) return 1;
        int cmp = Gen.CompareTo(other.Gen);
        return cmp != 0 ? cmp : Chip.CompareTo(other.Chip);
    }
}

sealed record State(byte Elevator, Pair[] Pairs)
{
    public bool Equals(State? other)
    {
        if (other is null || Elevator != other.Elevator || Pairs.Length != other.Pairs.Length)
            return false;

        for (int i = 0; i < Pairs.Length; i++)
        {
            if (!Pairs[i].Equals(other.Pairs[i]))
                return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(Elevator);
        foreach (var p in Pairs)
        {
            hc.Add(p.Gen);
            hc.Add(p.Chip);
        }
        return hc.ToHashCode();
    }
}

static class Day11Solver
{
    private readonly record struct Item(char Kind, int PairIndex);

    public static Pair[] ParseInput(string input)
    {
        var genFloor = new Dictionary<string, byte>();
        var chipFloor = new Dictionary<string, byte>();
        var regex = new Regex(@"(\w+)(?:-compatible)? (generator|microchip)", RegexOptions.Compiled);

        string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (byte floor = 0; floor < lines.Length; floor++)
        {
            foreach (Match m in regex.Matches(lines[floor]))
            {
                string element = m.Groups[1].Value;
                string kind = m.Groups[2].Value;
                if (kind == "generator")
                    genFloor[element] = floor;
                else
                    chipFloor[element] = floor;
            }
        }

        var pairs = genFloor.Keys
            .Select(el => new Pair(genFloor[el], chipFloor[el]))
            .Order()
            .ToArray();

        return pairs;
    }

    private static bool IsValid(Pair[] pairs)
    {
        int genFloorMask = 0;
        foreach (var p in pairs)
            genFloorMask |= (1 << p.Gen);

        foreach (var p in pairs)
        {
            bool hasAnyGenOnChipFloor = (genFloorMask & (1 << p.Chip)) != 0;
            if (hasAnyGenOnChipFloor && p.Gen != p.Chip)
                return false;
        }
        return true;
    }

    private static bool IsGoal(Pair[] pairs, byte topFloor)
    {
        foreach (var p in pairs)
        {
            if (p.Gen != topFloor || p.Chip != topFloor)
                return false;
        }
        return true;
    }

    public static int Solve(Pair[] initialPairs, byte numFloors = 4)
    {
        byte topFloor = (byte)(numFloors - 1);
        var startPairs = initialPairs.Order().ToArray();

        if (IsGoal(startPairs, topFloor))
            return 0;

        var start = new State(0, startPairs);
        var visited = new HashSet<State> { start };
        var queue = new Queue<(State State, int Steps)>();
        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            var (current, steps) = queue.Dequeue();
            byte elev = current.Elevator;
            Pair[] pairs = current.Pairs;

            var itemsHere = new List<Item>();
            for (int i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].Gen == elev) itemsHere.Add(new Item('G', i));
                if (pairs[i].Chip == elev) itemsHere.Add(new Item('C', i));
            }

            var pairsOfTwo = new List<Item[]>();
            for (int i = 0; i < itemsHere.Count; i++)
                for (int j = i + 1; j < itemsHere.Count; j++)
                    pairsOfTwo.Add(new[] { itemsHere[i], itemsHere[j] });

            var singles = itemsHere.Select(it => new[] { it }).ToList();

            sbyte[] directions = { 1, -1 };
            foreach (sbyte dir in directions)
            {
                int nextElevInt = elev + dir;
                if (nextElevInt < 0 || nextElevInt > topFloor)
                    continue;

                byte nextElev = (byte)nextElevInt;

                if (dir == -1 && pairs.All(p => p.Gen > nextElev && p.Chip > nextElev))
                    continue;

                var orderedGroups = dir == 1
                    ? new[] { pairsOfTwo, singles }
                    : new[] { singles, pairsOfTwo };

                for (int gIdx = 0; gIdx < orderedGroups.Length; gIdx++)
                {
                    bool addedInGroup = false;
                    foreach (Item[] moving in orderedGroups[gIdx])
                    {
                        var nextPairs = new Pair[pairs.Length];
                        for (int i = 0; i < pairs.Length; i++)
                        {
                            byte ng = pairs[i].Gen;
                            byte nc = pairs[i].Chip;

                            foreach (var mv in moving)
                            {
                                if (mv.PairIndex == i)
                                {
                                    if (mv.Kind == 'G') ng = nextElev;
                                    else nc = nextElev;
                                }
                            }
                            nextPairs[i] = new Pair(ng, nc);
                        }

                        Array.Sort(nextPairs);
                        if (!IsValid(nextPairs))
                            continue;

                        if (IsGoal(nextPairs, topFloor))
                            return steps + 1;

                        var nextState = new State(nextElev, nextPairs);
                        if (visited.Add(nextState))
                        {
                            queue.Enqueue((nextState, steps + 1));
                            addedInGroup = true;
                        }
                    }

                    if (addedInGroup)
                        break;
                }
            }
        }

        throw new InvalidOperationException("No se encontró solución.");
    }

    public static int Part1(string input) => Solve(ParseInput(input));

    public static int Part2(string input)
    {
        var list = ParseInput(input).ToList();
        list.Add(new Pair(0, 0));
        list.Add(new Pair(0, 0));
        return Solve(list.ToArray());
    }
}

class Program
{
    static void Main()
    {
        string sample = """
            The first floor contains a strontium generator, a strontium-compatible microchip, a plutonium generator, and a plutonium-compatible microchip.
            The second floor contains a thulium generator, a ruthenium generator, a ruthenium-compatible microchip, a curium generator, and a curium-compatible microchip.
            The third floor contains a thulium-compatible microchip.
            The fourth floor contains nothing relevant.
            """;

        Console.WriteLine($"Ejemplo Parte 1: {Day11Solver.Part1(sample)}");
        Console.WriteLine($"Ejemplo Parte 2: {Day11Solver.Part2(sample)}");
    }
}