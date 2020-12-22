using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XOR : MonoBehaviour
{
    public static string EncryptStr(string target, string secretKey)
    {
        char[] data = target.ToCharArray();
        char[] key = secretKey.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        return new string(data);
    }

    public static string DecryptStr(string target, string secretKey)
    {
        char[] data = target.ToCharArray();
        char[] key = secretKey.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        return new string(data);
    }
}
