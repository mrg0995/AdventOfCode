using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

string[] lines = File.ReadAllLines(@"Your_Route");

var replacements = new List<(string From, string To)>();
string targetMolecule = "";

bool readingReplacements = true;
foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line))
    {
        readingReplacements = false;
        continue;
    }

    if (readingReplacements)
    {
        var parts = line.Split(new[] { " => " }, StringSplitOptions.None);
        replacements.Add((parts[0], parts[1]));
    }
    else
    {
        targetMolecule = line.Trim();
    }
}

var distinctMolecules = new HashSet<string>();

foreach (var (from, to) in replacements)
{
    int index = 0;
    while ((index = targetMolecule.IndexOf(from, index)) != -1)
    {
        string newMolecule = targetMolecule.Substring(0, index) + to + targetMolecule.Substring(index + from.Length);
        distinctMolecules.Add(newMolecule);

        index += from.Length;
    }
}

Console.WriteLine($"Parte 1 - Moléculas distintas generadas: {distinctMolecules.Count}");

int steps = 0;
var rand = new Random();
string current = "";

while (current != "e")
{
    current = targetMolecule;
    steps = 0;

    var shuffledReplacements = replacements.OrderBy(x => rand.Next()).ToList();

    bool progress = true;
    while (current != "e" && progress)
    {
        progress = false;
        foreach (var (from, to) in shuffledReplacements)
        {
            int index = current.IndexOf(to);
            if (index != -1)
            {
                current = current.Substring(0, index) + from + current.Substring(index + to.Length);
                steps++;
                progress = true;
                break;
            }
        }
    }
}

Console.WriteLine($"Parte 2 - Pasos mínimos para llegar a 'e': {steps}");
