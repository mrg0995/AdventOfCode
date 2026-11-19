using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static string Md5Hex(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = MD5.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    static char? FindFirstTripletChar(string hex)
    {
        for (int i = 0; i <= hex.Length - 3; i++)
        {
            if (hex[i] == hex[i + 1] && hex[i] == hex[i + 2])
                return hex[i];
        }
        return null;
    }

    static bool HasQuintuplet(string hex, char target)
    {
        int run = 0;
        for (int i = 0; i < hex.Length; i++)
        {
            run = (hex[i] == target) ? run + 1 : 0;
            if (run >= 5)
                return true;
        }
        return false;
    }

    static int Find64thKeyIndex(string salt, bool stretched)
    {
        var cache = new Dictionary<int, string>();

        string GetHash(int index)
        {
            if (cache.TryGetValue(index, out string? cached))
                return cached;

            string digest = Md5Hex($"{salt}{index}");
            if (stretched)
            {
                for (int r = 0; r < 2016; r++)
                    digest = Md5Hex(digest);
            }

            cache[index] = digest;
            return digest;
        }

        int keysFound = 0;
        int index = 0;

        while (true)
        {
            string h = GetHash(index);
            char? tripletChar = FindFirstTripletChar(h);
            if (tripletChar.HasValue)
            {
                char ch = tripletChar.Value;
                bool foundQuintuplet = false;

                for (int k = index + 1; k <= index + 1000; k++)
                {
                    if (HasQuintuplet(GetHash(k), ch))
                    {
                        foundQuintuplet = true;
                        break;
                    }
                }

                if (foundQuintuplet)
                {
                    keysFound++;
                    if (keysFound == 64)
                        return index;
                }
            }

            index++;
        }
    }

    static void Main()
    {
        string salt = File.ReadAllText("Your_Route").Trim();
        Console.WriteLine($"Parte 1: {Find64thKeyIndex(salt, stretched: false)}");
        Console.WriteLine($"Parte 2: {Find64thKeyIndex(salt, stretched: true)}");
    }
}
