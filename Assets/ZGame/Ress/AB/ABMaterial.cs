using SoftMasking.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABMaterial
    {

        public static void Load(string matName, Action<Material> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                Material mat = objs[0] as Material;

                MatRes res = new MatRes(matName, mat);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);

                if (callback != null)
                {
                    callback(mat);
                }
            };

            AB.Load(matName, ABType.Material, (objs) =>
            {
                loadFinishHandle(objs);
            }, sync);
        }
    }
}