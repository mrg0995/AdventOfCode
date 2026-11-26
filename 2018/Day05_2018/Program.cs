using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static int React(IEnumerable<char> polymer)
{
    var stack = new Stack<char>();
    foreach (char ch in polymer)
    {
        if (stack.Count > 0 && stack.Peek() != ch &&
            char.ToLowerInvariant(stack.Peek()) == char.ToLowerInvariant(ch))
            stack.Pop();
        else
            stack.Push(ch);
    }
    return stack.Count;
}

string input = File.ReadAllText("input.txt").Trim();
int part1 = React(input);
int part2 = Enumerable.Range('a', 26)
    .Select(c => (char)c)
    .Min(u => React(input.Where(ch => char.ToLowerInvariant(ch) != u)));

Console.WriteLine($"Day 5 -> Part 1: {part1} | Part 2: {part2}");
