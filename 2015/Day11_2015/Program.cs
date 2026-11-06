using System;

string input = "hepxxyzz";
string nextPassword = FindNextPassword(input);

Console.WriteLine($"Contraseña actual: {input}");
Console.WriteLine($"Siguiente contraseña de Santa: {nextPassword}");

static string FindNextPassword(string password)
{
    password = IncrementString(password);

    while (!IsValidPassword(password))
    {
        password = IncrementString(password);
    }

    return password;
}

static string IncrementString(string s)
    {
        char[] chars = s.ToCharArray();
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            if (chars[i] == 'z')
            {
                chars[i] = 'a';
            }
            else
            {
                chars[i]++;

                if (chars[i] == 'i' || chars[i] == 'o' || chars[i] == 'l')
                {
                    chars[i]++;
                }
                break;
            }
        }
        return new string(chars);
    }

static bool IsValidPassword(string s)
{
    foreach (char c in s)
    {
        if (c == 'i' || c == 'o' || c == 'l')
            return false;
    }

    bool hasStraight = false;
    for (int i = 0; i < s.Length - 2; i++)
    {
        if (s[i + 1] == s[i] + 1 && s[i + 2] == s[i] + 2)
        {
            hasStraight = true;
            break;
        }
    }
    if (!hasStraight) return false;

    int pairCount = 0;
    for (int i = 0; i < s.Length - 1; i++)
    {
        if (s[i] == s[i + 1])
        {
            pairCount++;
            i++;
        }
    }

    return pairCount >= 2;
}
