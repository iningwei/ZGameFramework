using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Window
{
    public class WindowRequest
    {
        public delegate void Completed(ZGame.Res.AsyncOperation ao);
        public event Completed onCompleted;
        public object asset;
        public WindowResSource source;
        public void OnComplete(ZGame.Res.AsyncOperation ao)
        {
            if (this.onCompleted != null)
            {
                this.onCompleted(ao);
            }
        }
    }
}