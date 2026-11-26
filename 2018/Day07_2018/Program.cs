using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var raw = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

(HashSet<char> steps, Dictionary<char, HashSet<char>> prereqs, Dictionary<char, List<char>> deps) BuildGraph()
{
    var steps = new HashSet<char>();
    var prereqs = new Dictionary<char, HashSet<char>>();
    var deps = new Dictionary<char, List<char>>();
    foreach (var l in raw)
    {
        char a = l[5], b = l[36];
        steps.Add(a); steps.Add(b);
        prereqs.TryAdd(b, new HashSet<char>());
        prereqs[b].Add(a);
        deps.TryAdd(a, new List<char>());
        deps[a].Add(b);
    }
    return (steps, prereqs, deps);
}

// Parte 1
var (s1, pre1, dep1) = BuildGraph();
var pq1 = new PriorityQueue<char, char>();
foreach (char c in s1.Where(c => !pre1.ContainsKey(c) || pre1[c].Count == 0)) pq1.Enqueue(c, c);

var order = new List<char>();
while (pq1.Count > 0)
{
    char curr = pq1.Dequeue();
    order.Add(curr);
    if (!dep1.TryGetValue(curr, out var nexts)) continue;
    foreach (char nxt in nexts)
    {
        pre1[nxt].Remove(curr);
        if (pre1[nxt].Count == 0) pq1.Enqueue(nxt, nxt);
    }
}

// Parte 2
var (s2, pre2, dep2) = BuildGraph();
var avail = new PriorityQueue<char, char>();
foreach (char c in s2.Where(c => !pre2.ContainsKey(c) || pre2[c].Count == 0)) avail.Enqueue(c, c);

var workers = new PriorityQueue<(int End, char Step), int>();
int time = 0, done = 0;

while (done < s2.Count)
{
    while (avail.Count > 0 && workers.Count < 5)
    {
        char step = avail.Dequeue();
        int end = time + 60 + (step - 'A' + 1);
        workers.Enqueue((end, step), end);
    }
    var (endT, finished) = workers.Dequeue();
    time = endT;
    done++;
    if (!dep2.TryGetValue(finished, out var nexts)) continue;
    foreach (char nxt in nexts)
    {
        pre2[nxt].Remove(finished);
        if (pre2[nxt].Count == 0) avail.Enqueue(nxt, nxt);
    }
}

Console.WriteLine($"Day 7 -> Part 1: {new string(order.ToArray())} | Part 2: {time}");
