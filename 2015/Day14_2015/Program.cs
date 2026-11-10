using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

string rutaArchivo = @"Your_Route";
int raceDuration = 2503;
string pattern = @"^(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds\.$";

var reindeers = File.ReadAllLines(rutaArchivo)
    .Select(line => Regex.Match(line, pattern))
    .Where(m => m.Success)
    .Select(m => new Reindeer(
        Name: m.Groups[1].Value,
        Speed: int.Parse(m.Groups[2].Value),
        FlyTime: int.Parse(m.Groups[3].Value),
        RestTime: int.Parse(m.Groups[4].Value)
    ))
    .ToList();

int maxDistance = reindeers.Max(r => r.GetDistanceAt(raceDuration));
var winnerPart1 = reindeers.First(r => r.GetDistanceAt(raceDuration) == maxDistance);
Console.WriteLine($"[Parte 1] Ganador por distancia: {winnerPart1.Name} con {maxDistance} km.");


var points = reindeers.ToDictionary(r => r, r => 0);

for (int t = 1; t <= raceDuration; t++)
{
    var distances = reindeers.ToDictionary(r => r, r => r.GetDistanceAt(t));

    int currentMaxDist = distances.Values.Max();

    foreach (var reindeer in reindeers)
    {
        if (distances[reindeer] == currentMaxDist)
        {
            points[reindeer]++;
        }
    }
}

var winnerPart2 = points.OrderByDescending(p => p.Value).First();
Console.WriteLine($"[Parte 2] Ganador por puntos: {winnerPart2.Key.Name} con {winnerPart2.Value} puntos.");


record Reindeer(string Name, int Speed, int FlyTime, int RestTime)
{
    public int GetDistanceAt(int totalSeconds)
    {
        int cycleTime = FlyTime + RestTime;
        int fullCycles = totalSeconds / cycleTime;
        int remainingSeconds = totalSeconds % cycleTime;

        int activeFlyTime = (fullCycles * FlyTime) + Math.Min(remainingSeconds, FlyTime);
        return activeFlyTime * Speed;
    }
}