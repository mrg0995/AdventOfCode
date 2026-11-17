using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

string[] lines = File.ReadAllLines(@"Your_Route");

int tlsCount = 0;
int sslCount = 0;

foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line)) continue;

    string[] parts = Regex.Split(line.Trim(), @"\[|\]");
    List<string> supernets = new List<string>();
    List<string> hypernets = new List<string>();

    for (int i = 0; i < parts.Length; i++)
    {
        if (i % 2 == 0) supernets.Add(parts[i]);
        else hypernets.Add(parts[i]);
    }

    bool supernetHasAbba = supernets.Any(HasAbba);
    bool hypernetHasAbba = hypernets.Any(HasAbba);

    if (supernetHasAbba && !hypernetHasAbba)
    {
        tlsCount++;
    }

    HashSet<string> supernetAbas = new HashSet<string>();
    foreach (var s in supernets)
    {
        foreach (var aba in GetAbas(s))
        {
            supernetAbas.Add(aba);
        }
    }

    bool foundSsl = false;
    foreach (var aba in supernetAbas)
    {
        string bab = $"{aba[1]}{aba[0]}{aba[1]}";
        if (hypernets.Any(h => h.Contains(bab)))
        {
            foundSsl = true;
            break;
        }
    }

    if (foundSsl)
    {
        sslCount++;
    }
}

Console.WriteLine($"Resultado Parte 1 (TLS): {tlsCount}");
Console.WriteLine($"Resultado Parte 2 (SSL): {sslCount}");

static bool HasAbba(string s)
{
    for (int i = 0; i < s.Length - 3; i++)
    {
        if (s[i] == s[i + 3] && s[i + 1] == s[i + 2] && s[i] != s[i + 1])
        {
            return true;
        }
    }
    return false;
}

static HashSet<string> GetAbas(string s)
{
    var abas = new HashSet<string>();
    for (int i = 0; i < s.Length - 2; i++)
    {
        if (s[i] == s[i + 2] && s[i] != s[i + 1])
        {
            abas.Add(s.Substring(i, 3));
        }
    }
    return abas;
}