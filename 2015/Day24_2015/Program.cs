using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

List<long> weights = File.ReadAllLines(@"Your_Route")
                                 .Where(l => !string.IsNullOrWhiteSpace(l))
                                 .Select(long.Parse)
                                 .OrderByDescending(w => w)
                                 .ToList();

long totalWeight = weights.Sum();

long targetWeightPart1 = totalWeight / 3;
long qePart1 = Solve(weights, targetWeightPart1);
Console.WriteLine($"Parte 1 - Mínimo Quantum Entanglement (3 grupos): {qePart1}");

long targetWeightPart2 = totalWeight / 4;
long qePart2 = Solve(weights, targetWeightPart2);
Console.WriteLine($"Parte 2 - Mínimo Quantum Entanglement (4 grupos): {qePart2}");

static long Solve(List<long> weights, long target)
{
    for (int k = 1; k <= weights.Count; k++)
    {
        var validGroups = new List<List<long>>();
        FindCombinations(weights, target, 0, k, new List<long>(), validGroups);

        if (validGroups.Count > 0)
        {
            return validGroups.Select(g => g.Aggregate(1L, (acc, val) => acc * val))
                              .Min();
        }
    }

    return -1;
}

static void FindCombinations(List<long> weights, long target, int startIndex, int remainingCount, List<long> currentGroup, List<List<long>> results)
{
    if (target == 0 && remainingCount == 0)
    {
        results.Add(new List<long>(currentGroup));
        return;
    }

    if (target < 0 || remainingCount == 0 || startIndex >= weights.Count)
    {
        return;
    }

    for (int i = startIndex; i < weights.Count; i++)
    {
        long weight = weights[i];

        if (weight > target) continue;

        currentGroup.Add(weight);
        FindCombinations(weights, target - weight, i + 1, remainingCount - 1, currentGroup, results);
        currentGroup.RemoveAt(currentGroup.Count - 1);
    }
}