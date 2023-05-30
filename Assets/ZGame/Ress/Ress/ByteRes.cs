using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

//һЩ�Զ�������� bytes �ļ�
namespace ZGame.Ress
{
    [Serializable]
    public class ByteRes : Res
    {
        string name;
        TextAsset asset;


        public ByteRes(string resName, TextAsset resObj) : base(resName, resObj)
        {
            this.name = resName;
            this.asset = resObj;
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(TextAsset))))
            {
                Debug.LogError("���Ͳ�ƥ�� TextAsset");
            }

            if (this.resName != name)
            {
                Debug.LogError("error,name��ƥ��");
            }

            result = (T)(object)asset;


            if (result == null)
            {
                Debug.LogError("error, get res fail, asset is null");
            }
            return result;
        }

        public override void Destroy()
        {
            base.Destroy();
            ABManager.Instance.RemoveRes(ABType.Byte, this);
        }
    }


    
}
