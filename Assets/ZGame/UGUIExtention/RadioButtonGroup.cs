using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame.EventExt;
namespace ZGame.UGUIExtention
{
    public class RadioButtonGroup : MonoBehaviour
    {
        public int curIndex = 0;
        public Button[] btns;
        public UnityExtEvent onIndexChanged = new UnityExtEvent();

        public int GetCurIndex()
        {
            return this.curIndex;
        }
        private void Start()
        {

            if (btns != null && btns.Length != 0)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    int index = i;
                    btns[i].onClick.AddListener(() =>
                    {
                        this.onRadioBtnClicked(index);
                    });
                }
            }
        }

        void onRadioBtnClicked(int index)
        {
            this.SetSelection(index);
        }

        void selectIndex(int index)
        {
            if (btns == null || btns.Length == 0)
            {
                return;
            }

            Image targetImage;
            for (int i = 0; i < btns.Length; i++)
            {
                if (i == index)
                {
                    targetImage = btns[i].GetComponent<RadioButtonSelection>().selectImage;
                    btns[i].interactable = false;
                }
                else
                {
                    targetImage = btns[i].GetComponent<RadioButtonSelection>().unselectImage;
                    btns[i].interactable = true;
                }
                btns[i].GetComponent<Image>().sprite = targetImage.sprite;

            }

        }


        public void SetSelection(int index)
        {
            this.curIndex = index;
            this.selectIndex(index);
            onIndexChanged.Invoke(index);
        }
    }
}