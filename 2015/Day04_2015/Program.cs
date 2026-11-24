using System;
using System.Security.Cryptography;
using System.Text;

string texto = "ckczppom";
long numero = 0;
string objetivo = "000000";
string hashActual = string.Empty;

using (MD5 md5 = MD5.Create())
{
    while (true)
    {
        string Contraseña = texto + numero;

        byte[] inputBytes = Encoding.UTF8.GetBytes(Contraseña);

        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hashBytes.Length; i++)
        {
           sb.Append(hashBytes[i].ToString("x2"));
        }

        hashActual = sb.ToString();

        if (hashActual.StartsWith(objetivo))
        {
            break;
        }

        numero++;

    }

}

Console.WriteLine($"Número utilizado: {numero}");
Console.WriteLine($"Hash resultante: {hashActual}");
