using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using ZGame;

public static class StringExt
{

    /// <summary>
    /// 扩展 获取变量名称(字符串)
    /// //PS:C# 6.0可以使用 nameof(变量) 来获得变量的名字
    /// </summary>
    /// <param name="var_name"></param>
    /// <param name="exp"></param>
    /// <returns>return string</returns>
    public static string GetVarName<T>(this T var_name, System.Linq.Expressions.Expression<Func<T>> exp)
    {
        return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
    }

    /// <summary>
    /// 判断字符串是否包含汉字
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool ContainChinese(this string text)
    {
        bool res = false;
        foreach (char t in text)
        {
            if ((int)t > 127)
                res = true;
        }
        return res;
    }

    /// <summary>
    /// 判断资源名是否合法
    /// 规则：“字母、数字、空格符 、下划线_、中划线-”组成，且开头和结尾只能是字母或者数字
    /// </summary>
    /// <param name="resNameWithoutExt">不带后缀的文件名</param>
    /// <returns></returns>
    public static bool IsResNameValid(this string resNameWithoutExt)
    {

        if (Regex.Match(resNameWithoutExt, @"^[0-9a-zA-Z][a-zA-Z0-9 _\-]+[0-9a-zA-Z]$").Success)
        {
            return true;
        }

        return false;
    }


    //反斜杠\ backslash
    //斜杠/  slash
    /// <summary>
    /// 归一化路径中的斜杠或反斜杠为斜杠
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static string UniformSlash(this string inputStr)
    {
        return inputStr.Replace('\\', '/');
    }


    public static int[] ToIntArray(this string[] strArray)
    {
        if (strArray == null || strArray.Length == 0)
        {
            Debug.LogError("error, input stringArray is null or length=0");
        }
        int[] result = new int[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            try
            {
                result[i] = int.Parse(strArray[i]);
            }
            catch (Exception ex)
            {
                Debug.LogError("StringArrayToIntArray exception:" + ex.Message);
            }

        }
        return result;
    }

    public static long[] ToLongArray(this string[] strArray)
    {
        if (strArray == null || strArray.Length == 0)
        {
            Debug.LogError("error, input stringArray is null or length=0");
        }
        long[] result = new long[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            try
            {
                result[i] = long.Parse(strArray[i]);
            }
            catch (Exception ex)
            {

                Debug.LogError("StringArrayToLongArray exception:" + ex.Message);
            }

        }
        return result;
    }

    public static float[] ToFloatArray(this string[] strArray)
    {
        if (strArray == null || strArray.Length == 0)
        {
            Debug.LogError("error, input stringArray is null or length=0");
        }
        float[] result = new float[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            try
            {
                result[i] = float.Parse(strArray[i]);
            }
            catch (Exception ex)
            {
                Debug.LogError("StringArrayToFloatArray exception:" + ex.Message);
            }

        }
        return result;
    }


    public static Vector2 ToVector2(this string str, char splitChar)
    {
        try
        {
            var datas = str.Split(splitChar);
            return new Vector2(float.Parse(datas[0]), float.Parse(datas[1]));
        }
        catch (Exception ex)
        {
            Debug.LogError("ToVector2 Error, input str:" + str + ", splitChar:" + splitChar + ", ex:" + ex.ToString());

            return Vector2.zero;
        }

    }


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

    public static string SubWithPoints(this string str, int len = 5)
    {
        if (str.Length <= len)
        {
            return str;
        }
        else
        {
            return str.Substring(0, len) + "...";
        }
    }



    /// <summary>
    /// 首字母小写写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToLower(this string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        string str = input.First().ToString().ToLower() + input.Substring(1);
        return str;
    }

    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToUpper(this string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        string str = input.First().ToString().ToUpper() + input.Substring(1);
        return str;
    }

    /// <summary>
    /// 向下取整指定位数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimalLength"></param>
    /// <returns></returns>
    public static string FormatNumberToString(this double value, int decimalLength, bool removeZero)
    {
        double d = Math.Pow(10, decimalLength);
        double v = ((int)Math.Floor(value * d)) / d;
        if (removeZero)
        {
            return v.ToString();
        }
        else
        {
            return v.ToString($"F{decimalLength}");
        }
    }
    /// <summary>
    /// 向下取整指定位数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimalLength"></param>
    /// <returns></returns>
    public static string FormatNumberToString(this float value, int decimalLength, bool removeZero)
    {
        var d = (double)value;
        return d.FormatNumberToString(decimalLength, removeZero);
    }


    public static bool VerifyIsValidPhoneNum(this string phoneNumStr)
    {
        string pattern = @"^1[3456789]\d{9}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumStr, pattern);
    }

    public static bool VerifyIsValidIdentityCardNum(this string idCardStr)
    {
        string pattern = @"^[1-9]\d{5}(18|19|20|21|22)?\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}(\d|[Xx])$";
        return System.Text.RegularExpressions.Regex.IsMatch(idCardStr, pattern);
    }
}
