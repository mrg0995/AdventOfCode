using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var lines = File.ReadAllLines("input.txt");
var grid = new Dictionary<(int X, int Y), char>();
var carts = new List<Cart>();

for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[y].Length; x++)
    {
        char ch = lines[y][x];
        switch (ch)
        {
            case '^': carts.Add(new Cart(x, y, 0, -1, 0, false)); grid[(x, y)] = '|'; break;
            case 'v': carts.Add(new Cart(x, y, 0, 1, 0, false)); grid[(x, y)] = '|'; break;
            case '<': carts.Add(new Cart(x, y, -1, 0, 0, false)); grid[(x, y)] = '-'; break;
            case '>': carts.Add(new Cart(x, y, 1, 0, 0, false)); grid[(x, y)] = '-'; break;
            case '/': case '\\': case '+': case '-': case '|': grid[(x, y)] = ch; break;
        }
    }
}

(int X, int Y)? firstCrash = null;
while (true)
{
    var ordered = carts.Where(c => !c.Crashed).OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
    foreach (var c in ordered)
    {
        if (c.Crashed) continue;
        c.X += c.Dx;
        c.Y += c.Dy;

        var hit = carts.FirstOrDefault(o => !o.Crashed && o != c && o.X == c.X && o.Y == c.Y);
        if (hit is not null)
        {
            firstCrash ??= (c.X, c.Y);
            c.Crashed = true;
            hit.Crashed = true;
            continue;
        }

        if (grid.TryGetValue((c.X, c.Y), out char tr))
        {
            if (tr == '/') { int t = c.Dx; c.Dx = -c.Dy; c.Dy = -t; }
            else if (tr == '\\') { int t = c.Dx; c.Dx = c.Dy; c.Dy = t; }
            else if (tr == '+')
            {
                int m = c.Turns % 3;
                if (m == 0) { int t = c.Dx; c.Dx = c.Dy; c.Dy = -t; }
                else if (m == 2) { int t = c.Dx; c.Dx = -c.Dy; c.Dy = t; }
                c.Turns++;
            }
        }
    }

    var alive = carts.Where(c => !c.Crashed).ToList();
    if (alive.Count == 1)
    {
        Console.WriteLine($"Day 13 -> Part 1: {firstCrash!.Value.X},{firstCrash.Value.Y} | Part 2: {alive[0].X},{alive[0].Y}");
        break;
    }
}

class Cart(int x, int y, int dx, int dy, int turns, bool crashed)
{
    public int X = x, Y = y, Dx = dx, Dy = dy, Turns = turns;
    public bool Crashed = crashed;
}
