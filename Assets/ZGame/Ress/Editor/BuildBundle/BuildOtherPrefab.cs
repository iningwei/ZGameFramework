﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    public class BuildOtherPrefab : BuildPrefab
    {
        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.OtherPrefab);
            return base.Build(obj);
        }
    }
}