using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeExt
{
    /// <summary>
    /// 获得当前时间对应的时间戳（单位毫秒）
    /// </summary>
    /// <returns></returns>
    public static long GetNowStamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    /// <summary>
    /// 获得明天0点的时间戳(单位毫秒)
    /// </summary>
    /// <returns></returns>
    public static long GetTomorrowZeroTimeStamp()
    {
        //获得今天0点对应的DateTime
        var todayZeroTime = DateTime.Now.Date;
        //获得明天0点对应的DateTime
        var tomorrowZeroTime = todayZeroTime.AddDays(1);

        long tomorrowStamp = (tomorrowZeroTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        return tomorrowStamp;
    }

    /// <summary>
    /// 单位毫秒
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long GetStamp(DateTime date)
    {
        return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    /// <summary>
    /// 获得年月日字符串
    /// 格式：yyyy分隔符MM分隔符dd
    /// </summary>
    /// <param name="date"></param>
    /// <param name="splitStr"></param>
    /// <returns></returns>
    public static string GetyyMMdd(DateTime date, string splitStr)
    {
        return date.ToString($"yyyy{splitStr}MM{splitStr}dd");
    }


    /// <summary>
    /// 获得年月日数值存在int[]数组内
    /// [0]是年，[1]是月，[2]是日
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int[] GetYMD(DateTime date)
    {
        int[] ymd = new int[3];
        ymd[0] = date.Year;
        ymd[1] = date.Month;
        ymd[2] = date.Day;
        return ymd;
    }


    /// <summary>
    /// 比较 旧的年月日 和 新的年月日
    /// 满足新旧关系返回1
    /// 新旧相等返回0
    /// 不满足新旧关系返回-1
    /// </summary>
    /// <param name="oldYMD"></param>
    /// <param name="newYMD"></param>
    /// <returns></returns>
    public static int CompareTwoYMD(int[] oldYMD, int[] newYMD)
    {
        int oldYear = oldYMD[0];
        int oldMonth = oldYMD[1];
        int oldDay = oldYMD[2];
        int newYear = newYMD[0];
        int newMonth = newYMD[1];
        int newDay = newYMD[2];

        if (oldYear == newYear && oldMonth == newMonth && oldDay == newDay)
        {
            return 0;
        }

        if (oldYear > newYear)
        {
            return -1;
        }
        else
        {
            if (oldMonth > newMonth)
            {
                return -1;
            }
            else
            {
                if (oldDay > newDay)
                {
                    return -1;
                }
            }
        }

        return 1;
    }

}
