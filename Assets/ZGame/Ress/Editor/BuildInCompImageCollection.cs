using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;

public class BuildInCompImageCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();


        var imageChilds = new List<Image>();
        obj.GetComponentsInChildren<Image>(true, imageChilds);



        for (int i = 0; i < imageChilds.Count; i++)
        {
            Image image = imageChilds[i];

            if (image.sprite == null || image.material == null)//sprite和material有一个为null,则属于异常情况。                                                              
            {
                Debug.LogWarning("warning,image:" + image.transform.GetHierarchy() + "  sprite not set or  material not set");
                continue;
            }

            //compInfos add elements
            string atlasName = image.sprite.texture.name;
            string texName = image.sprite.name;
            SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
            List<SpriteInfo> spriteInfos = new List<SpriteInfo>() { spriteInfo };
            CompInfo buildInCompImageInfo = new BuildInCompImageInfo(
                image.transform,
                image.material,
                image.material.shader.name,
                spriteInfos);
            compInfos.Add(buildInCompImageInfo);

        }




        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();


        var imageChilds = new List<Image>();
        obj.GetComponentsInChildren<Image>(true, imageChilds);
        Debug.Log(obj.name + "'s Image count:" + imageChilds.Count);


        for (int i = 0; i < imageChilds.Count; i++)
        {
            Image image = imageChilds[i];

            if (image.sprite == null || image.material == null)//sprite和material有一个为null,则属于异常情况。                                                              
            {
                Debug.LogWarning("warning,image:" + image.transform.GetHierarchy() + "  sprite not set or  material not set");
                continue;
            }




            //buildMap add elements
            string atlasName = image.sprite.texture.name;
            string texPath = AssetDatabase.GetAssetPath(image.sprite.texture);
            AssetBundleBuild build = new AssetBundleBuild();
            string preFix = ABTypeUtil.GetPreFix(ABType.Sprite);
            build.assetBundleName = preFix + atlasName.ToLower() + IOTools.abSuffix;
            build.assetNames = new string[] { texPath };
            buildMap.Add(build);

        }

        return buildMap;
    }

}
