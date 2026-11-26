using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static long Play(int players, int lastMarble)
{
    var scores = new long[players];
    var circle = new LinkedList<int>();
    var current = circle.AddFirst(0);

    LinkedListNode<int> Next(LinkedListNode<int> n) => n.Next ?? circle.First!;
    LinkedListNode<int> Prev(LinkedListNode<int> n) => n.Previous ?? circle.Last!;

    for (int marble = 1; marble <= lastMarble; marble++)
    {
        if (marble % 23 == 0)
        {
            for (int i = 0; i < 7; i++) current = Prev(current);
            var nextCur = Next(current);
            scores[marble % players] += marble + current.Value;
            circle.Remove(current);
            current = nextCur;
        }
        else
        {
            current = circle.AddAfter(Next(current), marble);
        }
    }
    return scores.Max();
}

var parts = File.ReadAllText("input.txt").Split(' ', StringSplitOptions.RemoveEmptyEntries);
int players = int.Parse(parts[0]), lastMarble = int.Parse(parts[6]);

Console.WriteLine($"Day 9 -> Part 1: {Play(players, lastMarble)} | Part 2: {Play(players, lastMarble * 100)}");