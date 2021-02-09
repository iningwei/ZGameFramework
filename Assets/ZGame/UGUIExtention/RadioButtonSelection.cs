using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ZGame.UGUIExtention
{
    public class RadioButtonSelection : MonoBehaviour
    {
        public Image selectImage;
        public Image unselectImage;

        private void Start()
        {
            this.selectImage.raycastTarget = false;
            this.unselectImage.raycastTarget = false;
            this.selectImage.gameObject.SetActive(false);
            this.unselectImage.gameObject.SetActive(false);
        }
    }
}