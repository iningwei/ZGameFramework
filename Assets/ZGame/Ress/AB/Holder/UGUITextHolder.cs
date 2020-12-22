using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.AB.Holder
{
    public class UGUITextHolder : MonoBehaviour
    {
        [System.Serializable]
        public class TextEntity
        {
            public Transform target;
            public string fontName;
            public TextEntity(Transform target, string fontName)
            {
                this.target = target;
                this.fontName = fontName;
            }

        }

        public List<TextEntity> entities;
    }
}