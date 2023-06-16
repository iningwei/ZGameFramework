using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SetUIRaycastStatus : Editor
{
    [MenuItem("GameObject/SetUIRaycastStatus/opposite")]
    static void SetUIRaycastOpposite()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var rectTrans = obj.GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < rectTrans.Length; i++)
        {
            var rectTran = rectTrans[i];
            //text
            var text = rectTran.GetComponent<Text>();
            if (text)
            {
                text.raycastTarget = !text.raycastTarget;
            }

            //image
            var image = rectTran.GetComponent<Image>();
            if (image)
            {
                image.raycastTarget = !image.raycastTarget;
            }
            //rawImage
            var rawImage = rectTran.GetComponent<RawImage>();
            if (rawImage)
            {
                rawImage.raycastTarget = !rawImage.raycastTarget;
            }
            //RoundRawImage
            var rri = rectTran.GetComponent<RoundedRawImage>();
            if (rri)
            {
                rri.raycastTarget = !rri.raycastTarget;
            }
        }
        Debug.Log("finish SetUIRaycast opposite");
        EditorUtility.SetDirty(obj);
    }



    [MenuItem("GameObject/SetUIRaycastStatus/open")]
    static void SetUIRaycastOpen()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var rectTrans = obj.GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < rectTrans.Length; i++)
        {
            var rectTran = rectTrans[i];
            //text
            var text = rectTran.GetComponent<Text>();
            if (text)
            {
                text.raycastTarget = true;
            }

            //image
            var image = rectTran.GetComponent<Image>();
            if (image)
            {
                image.raycastTarget = true;
            }
            //rawImage
            var rawImage = rectTran.GetComponent<RawImage>();
            if (rawImage)
            {
                rawImage.raycastTarget = true;
            }
            //RoundRawImage
            var rri = rectTran.GetComponent<RoundedRawImage>();
            if (rri)
            {
                rri.raycastTarget = true;
            }
        }
        Debug.Log("finish SetUIRaycast open");
        EditorUtility.SetDirty(obj);
    }

    [MenuItem("GameObject/SetUIRaycastStatus/close")]
    static void SetUIRaycastClose()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var rectTrans = obj.GetComponentsInChildren<RectTransform>(true);
        for (int i = 0; i < rectTrans.Length; i++)
        {
            var rectTran = rectTrans[i];
            //text
            var text = rectTran.GetComponent<Text>();
            if (text)
            {
                text.raycastTarget = false;
            }

            //image
            var image = rectTran.GetComponent<Image>();
            if (image)
            {
                image.raycastTarget = false;
            }
            //rawImage
            var rawImage = rectTran.GetComponent<RawImage>();
            if (rawImage)
            {
                rawImage.raycastTarget = false;
            }
            //RoundRawImage
            var rri = rectTran.GetComponent<RoundedRawImage>();
            if (rri)
            {
                rri.raycastTarget = false;
            }
        }
        Debug.Log("finish SetUIRaycast close");
        EditorUtility.SetDirty(obj);
    }
}
