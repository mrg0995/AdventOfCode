using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

string[] lines = File.ReadAllLines("Your_Route");

Dictionary<string, List<int>> botChips = new Dictionary<string, List<int>>();
Dictionary<string, (string Low, string High)> botRules = new Dictionary<string, (string, string)>();
Dictionary<string, int> outputs = new Dictionary<string, int>();
Queue<string> queue = new Queue<string>();

foreach (var rawLine in lines)
{
    string line = rawLine.Trim();
    if (string.IsNullOrEmpty(line)) continue;

    if (line.StartsWith("value"))
    {
        var match = Regex.Match(line, @"value (\d+) goes to (bot \d+)");
        int val = int.Parse(match.Groups[1].Value);
        string bot = match.Groups[2].Value;

        if (!botChips.ContainsKey(bot)) botChips[bot] = new List<int>();
        botChips[bot].Add(val);

        if (botChips[bot].Count == 2)
        {
            queue.Enqueue(bot);
        }
    }
    else
    {
        var match = Regex.Match(line, @"(bot \d+) gives low to (.*?) and high to (.*)");
        string bot = match.Groups[1].Value;
        string lowDest = match.Groups[2].Value;
        string highDest = match.Groups[3].Value;

        botRules[bot] = (lowDest, highDest);
    }
}

int part1Ans = -1;

while (queue.Count > 0)
{
    string currentBot = queue.Dequeue();
    var chips = botChips[currentBot];
    chips.Sort();
    int lowVal = chips[0];
    int highVal = chips[1];

    if (lowVal == 17 && highVal == 61)
    {
        part1Ans = int.Parse(Regex.Match(currentBot, @"\d+").Value);
    }

    var (lowDest, highDest) = botRules[currentBot];

    if (lowDest.StartsWith("bot"))
    {
        if (!botChips.ContainsKey(lowDest)) botChips[lowDest] = new List<int>();
        botChips[lowDest].Add(lowVal);
        if (botChips[lowDest].Count == 2) queue.Enqueue(lowDest);
    }
    else
    {
        outputs[lowDest] = lowVal;
    }

    if (highDest.StartsWith("bot"))
    {
        if (!botChips.ContainsKey(highDest)) botChips[highDest] = new List<int>();
        botChips[highDest].Add(highVal);
        if (botChips[highDest].Count == 2) queue.Enqueue(highDest);
    }
    else
    {
        outputs[highDest] = highVal;
    }
}

int out0 = outputs.ContainsKey("output 0") ? outputs["output 0"] : 1;
int out1 = outputs.ContainsKey("output 1") ? outputs["output 1"] : 1;
int out2 = outputs.ContainsKey("output 2") ? outputs["output 2"] : 1;
int part2Ans = out0 * out1 * out2;

Console.WriteLine($"Resultado Parte 1 (Bot ID): {part1Ans}");
Console.WriteLine($"Resultado Parte 2 (Producto outputs 0, 1, 2): {part2Ans}");