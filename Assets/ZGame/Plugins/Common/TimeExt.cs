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
}
