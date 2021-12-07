
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class MathTool
    {

        //https://www.cnblogs.com/lyh916/p/10633132.html
        /// <summary>
        /// 点是否在多边形范围内
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="vertexs">多边形顶点列表</param>
        /// <returns></returns>
        public static bool IsPointInPolygon(Vector2 p, List<Vector2> vertexs)
        {
            int crossNum = 0;
            int vertexCount = vertexs.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 v1 = vertexs[i];
                Vector2 v2 = vertexs[(i + 1) % vertexCount];

                if (((v1.y <= p.y) && (v2.y > p.y))
                    || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNum += 1;
                    }
                }
            }

            if (crossNum % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        /// <summary>
        /// 根据直角三角形两直角边，获得其斜边长度
        /// </summary>
        /// <param name="cathetus1"></param>
        /// <param name="cathetus2"></param>
        /// <returns></returns>
        public static float GetRightTriangleHypotenuseLength(float cathetus1, float cathetus2)
        {
            if (cathetus1 <= 0 || cathetus2 <= 0)
            {
                Debug.LogError("error input of cathethus");
            }
            return Mathf.Sqrt(cathetus1 * cathetus1 + cathetus2 * cathetus2);
        }
    }
}