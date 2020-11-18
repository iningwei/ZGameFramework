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
        int h = seconds / 3600;
        seconds = seconds % 3600;
        int m = seconds / 60;
        seconds = seconds % 60;

        string hStr = "";
        if (h > 24)
        {
            hStr = h.ToString();
        }
        else
        {
            hStr = h.ToString().PadLeft(2, '0');
        }

        string mStr = m.ToString().PadLeft(2, '0');
        string sStr = seconds.ToString().PadLeft(2, '0');
        hms = hStr + ":" + mStr + ":" + sStr;
        return hms;
    }
}
