using System;
using System.Text;

string current = "1113222113";

Console.WriteLine($"Término 0: {current}");

for (int i = 1; i <= 50; i++)
{
    current = GetNextLookAndSay(current);
    Console.WriteLine($"Término {i}: (Longitud: {current.Length})");
}

static string GetNextLookAndSay(string s)
{
    StringBuilder sb = new StringBuilder();
    int length = s.Length;

    for (int i = 0; i < length; i++)
    {
        int count = 1;

        while (i + 1 < length && s[i] == s[i + 1])
        {
            count++;
            i++;
        }

        sb.Append(count);
        sb.Append(s[i]);
    }

    return sb.ToString();
}        
