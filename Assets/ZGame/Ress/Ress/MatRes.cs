using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress
{
    public class MatRes : Res
    {
         
        public MatRes(string resName, Material resObj) :base(resName,resObj)
        {

        }
        public override T GetRes<T>(string name)
        {
            return base.GetRes<T>(name);
        }
    }
}
