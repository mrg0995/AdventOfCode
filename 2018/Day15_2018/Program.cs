using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var lines = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

(bool WonClean, int Outcome) Simulate(int elfAtk, bool stopOnElfDeath)
{
    var walls = new HashSet<(int X, int Y)>();
    var units = new List<Unit>();
    int id = 0;

    for (int y = 0; y < lines.Length; y++)
    {
        for (int x = 0; x < lines[y].Length; x++)
        {
            char c = lines[y][x];
            if (c == '#') walls.Add((x, y));
            else if (c == 'E' || c == 'G') units.Add(new Unit(id++, c, x, y, 200));
        }
    }

    (int X, int Y)[] Adj4(int x, int y) => [(x, y - 1), (x - 1, y), (x + 1, y), (x, y + 1)];

    int rounds = 0;
    while (true)
    {
        foreach (var u in units.Where(u => u.Hp > 0).OrderBy(u => u.Y).ThenBy(u => u.X).ToList())
        {
            if (u.Hp <= 0) continue;
            var enemies = units.Where(e => e.Hp > 0 && e.Kind != u.Kind).ToList();
            if (enemies.Count == 0)
            {
                int remHp = units.Where(e => e.Hp > 0).Sum(e => e.Hp);
                return (true, rounds * remHp);
            }

            var adjSet = Adj4(u.X, u.Y).ToHashSet();
            if (!enemies.Any(e => adjSet.Contains((e.X, e.Y))))
            {
                var occ = new HashSet<(int X, int Y)>(walls);
                foreach (var o in units.Where(o => o.Hp > 0 && o.Id != u.Id)) occ.Add((o.X, o.Y));

                var targets = new HashSet<(int X, int Y)>();
                foreach (var e in enemies)
                    foreach (var p in Adj4(e.X, e.Y))
                        if (!occ.Contains(p)) targets.Add(p);

                if (targets.Count > 0)
                {
                    var q = new Queue<(int X, int Y, int D)>();
                    var visited = new Dictionary<(int X, int Y), (int D, (int X, int Y)? Prev)>();
                    q.Enqueue((u.X, u.Y, 0));
                    visited[(u.X, u.Y)] = (0, null);

                    int? bestDist = null;
                    var reachable = new List<(int X, int Y)>();

                    while (q.Count > 0)
                    {
                        var (cx, cy, d) = q.Dequeue();
                        if (bestDist.HasValue && d > bestDist.Value) break;
                        if (targets.Contains((cx, cy)))
                        {
                            bestDist = d;
                            reachable.Add((cx, cy));
                            continue;
                        }
                        foreach (var nxt in Adj4(cx, cy))
                        {
                            if (!occ.Contains(nxt) && !visited.ContainsKey(nxt))
                            {
                                visited[nxt] = (d + 1, (cx, cy));
                                q.Enqueue((nxt.X, nxt.Y, d + 1));
                            }
                        }
                    }

                    if (reachable.Count > 0)
                    {
                        var goal = reachable.OrderBy(p => p.Y).ThenBy(p => p.X).First();
                        var cur = goal;
                        while (visited[cur].Prev.HasValue && visited[cur].Prev!.Value != (u.X, u.Y))
                            cur = visited[cur].Prev!.Value;
                        u.X = cur.X;
                        u.Y = cur.Y;
                    }
                }
            }

            var finalAdj = Adj4(u.X, u.Y).ToHashSet();
            var targetEnemy = units
                .Where(e => e.Hp > 0 && e.Kind != u.Kind && finalAdj.Contains((e.X, e.Y)))
                .OrderBy(e => e.Hp).ThenBy(e => e.Y).ThenBy(e => e.X)
                .FirstOrDefault();

            if (targetEnemy is not null)
            {
                targetEnemy.Hp -= u.Kind == 'E' ? elfAtk : 3;
                if (targetEnemy.Hp <= 0 && stopOnElfDeath && targetEnemy.Kind == 'E')
                    return (false, 0);
            }
        }

        units = units.Where(u => u.Hp > 0).ToList();
        rounds++;
    }
}

int part1 = Simulate(3, false).Outcome;
int atk = 4, part2 = 0;
while (true)
{
    var res = Simulate(atk, true);
    if (res.WonClean) { part2 = res.Outcome; break; }
    atk++;
}

Console.WriteLine($"Day 15 -> Part 1: {part1} | Part 2: {part2}");

class Unit(int id, char kind, int x, int y, int hp)
{
    public int Id = id, X = x, Y = y, Hp = hp;
    public char Kind = kind;
}
