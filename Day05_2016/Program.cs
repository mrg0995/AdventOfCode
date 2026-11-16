using System;
using System.Security.Cryptography;
using System.Text;

string doorId = "ugkcyxxp";

char[] passwordP2 = new char[8];
int filledP2 = 0;
StringBuilder passwordP1 = new StringBuilder();

using (MD5 md5 = MD5.Create())
{
    long index = 0;
    while (passwordP1.Length < 8 || filledP2 < 8)
    {
        string input = doorId + index;
        byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

        string hex = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        if (hex.StartsWith("00000"))
        {
            if (passwordP1.Length < 8)
            {
                passwordP1.Append(hex[5]);
            }

            char posChar = hex[5];
            if (posChar >= '0' && posChar <= '7')
            {
                int pos = posChar - '0';
                if (passwordP2[pos] == '\0')
                {
                    passwordP2[pos] = hex[6];
                    filledP2++;
                }
            }
        }
        index++;
    }
}

Console.WriteLine($"Parte 1: {passwordP1}");
Console.WriteLine($"Parte 2: {new string(passwordP2)}");