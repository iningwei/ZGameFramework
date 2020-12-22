using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ConvertExt
{
    public static string ToBase64(this string str)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    public static string Base64ToUTF8(this string base64Str)
    {
        var bytes = Convert.FromBase64String(base64Str);
        var utf8Str = Encoding.UTF8.GetString(bytes);
        return utf8Str;
    }
    
}
