using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var pts = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Select(l => Regex.Matches(l, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray())
    .Where(a => a.Length == 4)
    .ToArray();

int Dist4(int[] a, int[] b) =>
    Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]) + Math.Abs(a[2] - b[2]) + Math.Abs(a[3] - b[3]);

int n = pts.Length;
var visited = new bool[n];
int constellations = 0;

for (int i = 0; i < n; i++)
{
    if (visited[i]) continue;
    constellations++;
    var q = new Queue<int>();
    visited[i] = true;
    q.Enqueue(i);

    while (q.Count > 0)
    {
        int curr = q.Dequeue();
        for (int nxt = 0; nxt < n; nxt++)
        {
            if (!visited[nxt] && Dist4(pts[curr], pts[nxt]) <= 3)
            {
                visited[nxt] = true;
                q.Enqueue(nxt);
            }
        }
    }
}

Console.WriteLine($"Day 25 -> Constellations: {constellations}");
