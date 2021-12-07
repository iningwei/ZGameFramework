using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.Info;

//Text虽然没有对图片的直接引用，但是其有Material，故需要记录Material和其Shader
//主要是避免在编辑器下通过AB加载导致的shader不生效
public class BuildInCompTextCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();
        var textChilds = new List<Text>();
        obj.GetComponentsInChildren<Text>(true, textChilds);

        for (int i = 0; i < textChilds.Count; i++)
        {
            Text text = textChilds[i];

            CompInfo buildInCompTextInfo = new BuildInCompTextInfo(text.transform, text.material, text.material.shader.name);
            compInfos.Add(buildInCompTextInfo);
        }
        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

        var textChilds = new List<Text>();
        obj.GetComponentsInChildren<Text>(true, textChilds);
        Debug.Log(obj.name + "'s Text count:" + textChilds.Count);


        return buildMap;
    }
}
