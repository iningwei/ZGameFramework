using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberExt
{
    public static string FormatKM(this float value)
    {
        return FormatKM((int)value);
    }
    public static string FormatKM(this int value, int precise = 1)
    {
        string preciseStr = "f" + precise;
        string formatStr;
        if (value < 1000)
        {
            formatStr = value.ToString();
        }
        else if (value >= 1000 && value < 1000000)
        {
            formatStr = (value / 1000f).ToString(preciseStr) + "K";
        }
        else
        {
            formatStr = (value / 1000000f).ToString(preciseStr) + "M";
        }

        return formatStr;
    }

    public static string FormatHHMMSS(this int seconds)
    {
        string hms = "00:00:00";
        hms = $"{seconds / 3600:00}:{seconds % 3600 / 60:00}:{seconds % 60:00}";
        return hms;
    }

    public static string FormatMMSS(this int seconds)
    {
        string hms = "00:00";
        hms = $"{seconds / 60:00}:{seconds % 60:00}";
        return hms;
    }
  
}
