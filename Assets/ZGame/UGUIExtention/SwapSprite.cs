using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.UGUIExtention
{
    public class SwapSprite : MonoBehaviour
    {
        public Sprite[] sprites;
        Image image;
        private void Awake()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError("error, no Image component attached to:" + this.gameObject.GetHierarchy());
            }
            if (this.sprites == null || this.sprites.Length == 0)
            {
                Debug.LogError("error,no sprite set for sprites!");
            }

        }

        public void SetIndex(int i)
        {
            if (this.sprites == null || this.sprites.Length == 0)
            {
                Debug.LogError("can not set swapSprite for sprites have no contents.");
                return;
            }
            if (i > this.sprites.Length - 1)
            {
                Debug.LogError("index out of range:" + i);
                return;
            }
            var s = this.sprites[i];
            if (s == null)
            {
                Debug.LogError($"error sprite {i} is null");
            }
            image.sprite = s;
        }

        public void SetImageNativeSize()
        {
            image.SetNativeSize();
        }
    }
}