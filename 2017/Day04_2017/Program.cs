using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

string[] lines = File.ReadAllLines("input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .ToArray();

int part1 = lines.Count(IsValidNoDuplicates);
int part2 = lines.Count(IsValidNoAnagrams);

Console.WriteLine($"Parte 1: {part1}");
Console.WriteLine($"Parte 2: {part2}");

static bool IsValidNoDuplicates(string passphrase)
{
    string[] words = passphrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var seen = new HashSet<string>(words.Length);

    foreach (string word in words)
    {
        if (!seen.Add(word))
        {
            return false;
        }
    }

    return true;
}

static bool IsValidNoAnagrams(string passphrase)
{
    string[] words = passphrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var seenCanonical = new HashSet<string>(words.Length);

    foreach (string word in words)
    {
        char[] chars = word.ToCharArray();
        Array.Sort(chars);
        string canonical = new string(chars);

        if (!seenCanonical.Add(canonical))
        {
            return false;
        }
    }

    return true;
}