using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

string[] lines = File.ReadAllLines(@"Your_Route");

var happiness = new Dictionary<(string, string), int>();
var peopleSet = new HashSet<string>();

foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line)) continue;

    var parts = line.TrimEnd('.').Split(' ');
    string person = parts[0];
    string neighbor = parts[parts.Length - 1];
    int amount = int.Parse(parts[3]);

    if (parts[2] == "lose")
    {
        amount = -amount;
    }

    happiness[(person, neighbor)] = amount;
    peopleSet.Add(person);
}

List<string> people = peopleSet.ToList();

int maxHappinessPart1 = FindOptimalHappiness(people, happiness);
Console.WriteLine($"Máxima felicidad: {maxHappinessPart1}");

foreach (string person in people)
{
    happiness[("Me", person)] = 0;
    happiness[(person, "Me")] = 0;
}
people.Add("Me");

int maxHappinessPart2 = FindOptimalHappiness(people, happiness);
Console.WriteLine($"Máxima felicidad 2: {maxHappinessPart2}");

static int FindOptimalHappiness(List<string> people, Dictionary<(string, string), int> happiness)
{
    int maxTotalHappiness = int.MinValue;

    string fixedPerson = people[0];
    var others = people.Skip(1).ToList();

    foreach (var permutation in GetPermutations(others))
    {
        List<string> table = new List<string> { fixedPerson };
        table.AddRange(permutation);

        int currentHappiness = 0;
        int n = table.Count;

        for (int i = 0; i < n; i++)
        {
            string person = table[i];
            string leftNeighbor = table[(i - 1 + n) % n];
            string rightNeighbor = table[(i + 1) % n];

            currentHappiness += happiness[(person, leftNeighbor)];
            currentHappiness += happiness[(person, rightNeighbor)];
        }

        if (currentHappiness > maxTotalHappiness)
        {
            maxTotalHappiness = currentHappiness;
        }
    }

    return maxTotalHappiness;
}

static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list)
{
    if (!list.Any())
    {
        yield return Enumerable.Empty<T>();
        yield break;
    }

    var head = list.First();
    var tail = list.Skip(1);

    foreach (var perm in GetPermutations(tail))
    {
        for (int i = 0; i <= perm.Count(); i++)
        {
            yield return perm.Take(i).Concat(new[] { head }).Concat(perm.Skip(i));
        }
    }
}