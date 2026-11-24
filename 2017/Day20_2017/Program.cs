using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : Array.Empty<string>();

var particles = ParseParticles(lines);

var closestParticleId = FindClosestLongTermParticle(particles);
var remainingParticles = SimulateCollisions(particles);

Console.WriteLine($"Parte 1 - Partícula más cercana a largo plazo: {closestParticleId}");
Console.WriteLine($"Parte 2 - Partículas restantes tras colisiones: {remainingParticles}");

static Particle[] ParseParticles(string[] lines)
{
    var regex = new Regex(
        @"p=<\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*>,\s*" +
        @"v=<\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*>,\s*" +
        @"a=<\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*>",
        RegexOptions.Compiled);

    var result = new Particle[lines.Length];

    for (var i = 0; i < lines.Length; i++)
    {
        var match = regex.Match(lines[i]);
        if (!match.Success)
            throw new FormatException($"Línea de partícula inválida en índice {i}: {lines[i]}");

        static long G(Match m, int idx) => long.Parse(m.Groups[idx].Value, CultureInfo.InvariantCulture);

        result[i] = new Particle(
            Id: i,
            Px: G(match, 1), Py: G(match, 2), Pz: G(match, 3),
            Vx: G(match, 4), Vy: G(match, 5), Vz: G(match, 6),
            Ax: G(match, 7), Ay: G(match, 8), Az: G(match, 9)
        );
    }

    return result;
}

static int FindClosestLongTermParticle(Particle[] particles)
{
    if (particles.Length == 0)
        return -1;

    var best = particles
        .OrderBy(p => Manhattan(p.Ax, p.Ay, p.Az))
        .ThenBy(p => Manhattan(p.Vx, p.Vy, p.Vz))
        .ThenBy(p => Manhattan(p.Px, p.Py, p.Pz))
        .First();

    return best.Id;
}

static int SimulateCollisions(Particle[] initialParticles)
{
    var active = initialParticles.ToList();
    var ticksWithoutCollision = 0;
    const int stabilityThreshold = 200;

    while (ticksWithoutCollision < stabilityThreshold && active.Count > 1)
    {
        for (var i = 0; i < active.Count; i++)
        {
            var p = active[i];
            var vx = p.Vx + p.Ax;
            var vy = p.Vy + p.Ay;
            var vz = p.Vz + p.Az;

            var px = p.Px + vx;
            var py = p.Py + vy;
            var pz = p.Pz + vz;

            active[i] = p with { Px = px, Py = py, Pz = pz, Vx = vx, Vy = vy, Vz = vz };
        }

        var groupsByPosition = active
            .GroupBy(p => (p.Px, p.Py, p.Pz))
            .ToList();

        var anyCollision = groupsByPosition.Any(g => g.Count() > 1);

        if (anyCollision)
        {
            active = groupsByPosition
                .Where(g => g.Count() == 1)
                .Select(g => g.First())
                .ToList();
            ticksWithoutCollision = 0;
        }
        else
        {
            ticksWithoutCollision++;
        }
    }

    return active.Count;
}

static long Manhattan(long x, long y, long z)
    => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);

readonly record struct Particle(
    int Id,
    long Px, long Py, long Pz,
    long Vx, long Vy, long Vz,
    long Ax, long Ay, long Az);