using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    /// <summary>
    /// 多语言
    /// </summary>
    public class Multilingual
    {
        public static string FormatStr(string targetStr, params string[] paras)
        {
            if (targetStr == null || targetStr == "")
            {
                return "";
            }

            string r = targetStr;
            if (paras != null)
            {
                try
                {
                    r = string.Format(targetStr, paras);
                }
                catch (System.Exception ex)
                {
                    DebugExt.LogE(targetStr + " format failed");
                }
            }


            return r;
        }
    }
}