using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class BezierTool
    {
        public static Vector3 InterpVector3(Vector3 startPos, Vector3 endPos, Vector3 controlPoint1, float ratio)
        {
            if (ratio < 0 || ratio > 1)
            {
                //DebugExt.LogE("error, ratio should between [0,1]");
            }
            Vector3 result = (1 - ratio) * (1 - ratio) * startPos + 2 * ratio * (1 - ratio) * controlPoint1 + ratio * ratio * endPos;
            return result;
        }
    }
}