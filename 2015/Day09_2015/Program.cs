using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

string filePath = "Your_Route";

var rawDistances = ParseInputFile(filePath);

int shortestDistance = FindShortestPath(rawDistances);
int longestDistance = FindLongestPath(rawDistances);

Console.WriteLine($"La distancia más corta es: {shortestDistance}");
Console.WriteLine($"La distancia más larga es: {longestDistance}");

static List<(string From, string To, int Dist)> ParseInputFile(string filePath)
{
    var distances = new List<(string, string, int)>();
    foreach (var line in File.ReadLines(filePath))
    {
        var match = Regex.Match(line, @"^(\w+) to (\w+) = (\d+)$");
        if (match.Success)
        {
            distances.Add((match.Groups[1].Value, match.Groups[2].Value, int.Parse(match.Groups[3].Value)));
        }
    }
    return distances;
}

static int FindShortestPath(List<(string From, string To, int Dist)> rawDistances)
{
    var locationIds = new Dictionary<string, int>();
    int indexCounter = 0;

    foreach (var (from, to, _) in rawDistances)
    {
        if (!locationIds.ContainsKey(from)) locationIds[from] = indexCounter++;
        if (!locationIds.ContainsKey(to)) locationIds[to] = indexCounter++;
    }

    int n = locationIds.Count;
    int[,] dist = new int[n, n];
    int INF = 1_000_000_000;

    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            dist[i, j] = INF;

    foreach (var (from, to, d) in rawDistances)
    {
        int u = locationIds[from];
        int v = locationIds[to];
        dist[u, v] = d;
        dist[v, u] = d;
    }

    int totalStates = 1 << n;
    int[,] dp = new int[totalStates, n];

    for (int mask = 0; mask < totalStates; mask++)
        for (int u = 0; u < n; u++)
            dp[mask, u] = INF;

    for (int i = 0; i < n; i++) dp[1 << i, i] = 0;

    for (int mask = 1; mask < totalStates; mask++)
    {
        for (int last = 0; last < n; last++)
        {
            if ((mask & (1 << last)) == 0 || dp[mask, last] == INF) continue;

            for (int next = 0; next < n; next++)
            {
                if ((mask & (1 << next)) != 0) continue;

                int nextMask = mask | (1 << next);
                int newDist = dp[mask, last] + dist[last, next];

                if (newDist < dp[nextMask, next])
                {
                    dp[nextMask, next] = newDist;
                }
            }
        }
    }

    int fullMask = (1 << n) - 1;
    int minDistance = INF;

    for (int last = 0; last < n; last++)
    {
        minDistance = Math.Min(minDistance, dp[fullMask, last]);
    }

    return minDistance;
}

static int FindLongestPath(List<(string From, string To, int Dist)> rawDistances)
{
    var locationIds = new Dictionary<string, int>();
    int indexCounter = 0;

    foreach (var (from, to, _) in rawDistances)
    {
        if (!locationIds.ContainsKey(from)) locationIds[from] = indexCounter++;
        if (!locationIds.ContainsKey(to)) locationIds[to] = indexCounter++;
    }

    int n = locationIds.Count;
    int[,] dist = new int[n, n];
    int UNREACHABLE = 0;

    foreach (var (from, to, d) in rawDistances)
    {
        int u = locationIds[from];
        int v = locationIds[to];
        dist[u, v] = d;
        dist[v, u] = d;
    }

    int totalStates = 1 << n;
    int[,] dp = new int[totalStates, n];

    for (int mask = 0; mask < totalStates; mask++)
        for (int u = 0; u < n; u++)
            dp[mask, u] = UNREACHABLE;

    for (int i = 0; i < n; i++) dp[1 << i, i] = 0;

    for (int mask = 1; mask < totalStates; mask++)
    {
        for (int last = 0; last < n; last++)
        {
            if ((mask & (1 << last)) == 0) continue;
            if (dp[mask, last] == UNREACHABLE && mask != (1 << last)) continue;

            for (int next = 0; next < n; next++)
            {
                if ((mask & (1 << next)) != 0 || dist[last, next] == 0) continue;

                int nextMask = mask | (1 << next);
                int newDist = dp[mask, last] + dist[last, next];

                if (newDist > dp[nextMask, next])
                {
                    dp[nextMask, next] = newDist;
                }
            }
        }
    }

    int fullMask = (1 << n) - 1;
    int maxDistance = 0;

    for (int last = 0; last < n; last++)
    {
        maxDistance = Math.Max(maxDistance, dp[fullMask, last]);
    }

    return maxDistance;
}