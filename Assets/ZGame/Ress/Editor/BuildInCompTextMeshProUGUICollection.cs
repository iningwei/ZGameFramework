using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.Info;

public class BuildInCompTextMeshProUGUICollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();
        var textChilds = new List<TextMeshProUGUI>();
        obj.GetComponentsInChildren<TextMeshProUGUI>(true, textChilds);

        for (int i = 0; i < textChilds.Count; i++)
        {
            TextMeshProUGUI text = textChilds[i];
            
            //be careful not use text.material,otherwise it will return old ugui text's default material.
            CompInfo buildInCompTextMeshProInfo = new BuildInCompTextMeshProUGUIInfo(text.transform, text.font.material, text.font.material.shader.name);
            compInfos.Add(buildInCompTextMeshProInfo);

            Debug.Log("---->shader name:" + text.font.material.shader.name + ", transform:" +text.transform.name);
        }
        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

        var textChilds = new List<TextMeshProUGUI>();
        obj.GetComponentsInChildren<TextMeshProUGUI>(true, textChilds);
        Debug.Log(obj.name + "'s TextMeshProUGUI count:" + textChilds.Count);


        return buildMap;
    }


}
