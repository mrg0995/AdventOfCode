using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

string raw = File.ReadAllText("input.txt").Replace("\r\n", "\n").Trim();
var blocks = raw.Split("\n\n");
var rx = new Regex(@"(\d+) units each with (\d+) hit points (?:\(([^)]+)\) )?with an attack that does (\d+) (\w+) damage at initiative (\d+)");

List<Group> ParseGroups()
{
    var list = new List<Group>();
    int gid = 1;
    foreach (var (army, block) in new[] { ("immune", blocks[0]), ("infection", blocks[1]) })
    {
        foreach (var line in block.Split('\n').Skip(1).Select(l => l.Trim()).Where(l => l.Length > 0))
        {
            var m = rx.Match(line);
            var weak = new HashSet<string>();
            var immune = new HashSet<string>();
            string mods = m.Groups[3].Value;
            if (!string.IsNullOrEmpty(mods))
            {
                foreach (var part in mods.Split(';').Select(p => p.Trim()))
                {
                    if (part.StartsWith("weak to "))
                        foreach (var w in part[8..].Split(',')) weak.Add(w.Trim());
                    else if (part.StartsWith("immune to "))
                        foreach (var im in part[10..].Split(',')) immune.Add(im.Trim());
                }
            }
            list.Add(new Group
            {
                Id = gid++,
                Army = army,
                Units = int.Parse(m.Groups[1].Value),
                Hp = int.Parse(m.Groups[2].Value),
                Atk = int.Parse(m.Groups[4].Value),
                AtkType = m.Groups[5].Value,
                Init = int.Parse(m.Groups[6].Value),
                Weak = weak,
                Immune = immune
            });
        }
    }
    return list;
}

int DamageDealt(Group atk, Group def)
{
    if (def.Immune.Contains(atk.AtkType)) return 0;
    int dmg = atk.Power;
    if (def.Weak.Contains(atk.AtkType)) dmg *= 2;
    return dmg;
}

(string Winner, int Units) RunBattle(int boost)
{
    var groups = ParseGroups();
    foreach (var g in groups.Where(g => g.Army == "immune")) g.Atk += boost;

    while (true)
    {
        var alive = groups.Where(g => g.Units > 0).ToList();
        var armies = alive.Select(g => g.Army).Distinct().ToList();
        if (armies.Count == 1)
            return (armies[0], alive.Sum(g => g.Units));

        var orderSelect = alive.OrderByDescending(g => g.Power).ThenByDescending(g => g.Init).ToList();
        var chosen = new HashSet<int>();
        var targets = new Dictionary<int, Group>();

        foreach (var attacker in orderSelect)
        {
            var target = alive
                .Where(d => d.Army != attacker.Army && !chosen.Contains(d.Id) && DamageDealt(attacker, d) > 0)
                .OrderByDescending(d => DamageDealt(attacker, d))
                .ThenByDescending(d => d.Power)
                .ThenByDescending(d => d.Init)
                .FirstOrDefault();

            if (target is not null)
            {
                chosen.Add(target.Id);
                targets[attacker.Id] = target;
            }
        }

        int killsTotal = 0;
        foreach (var attacker in alive.OrderByDescending(g => g.Init))
        {
            if (attacker.Units <= 0 || !targets.TryGetValue(attacker.Id, out var def) || def.Units <= 0)
                continue;
            int dmg = DamageDealt(attacker, def);
            int kills = Math.Min(def.Units, dmg / def.Hp);
            def.Units -= kills;
            killsTotal += kills;
        }

        if (killsTotal == 0) return ("stalemate", 0);
    }
}

int part1 = RunBattle(0).Units;
int boostVal = 1, part2 = 0;
while (true)
{
    var (winner, rem) = RunBattle(boostVal);
    if (winner == "immune") { part2 = rem; break; }
    boostVal++;
}

Console.WriteLine($"Day 24 -> Part 1: {part1} | Part 2: {part2}");

class Group
{
    public int Id, Units, Hp, Atk, Init;
    public string Army = "", AtkType = "";
    public HashSet<string> Weak = [], Immune = [];
    public int Power => Units * Atk;
}
