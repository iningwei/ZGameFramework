using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
using ZGame.UGUIExtention;

public class ExtCompSwapSpriteCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        var swapSpriteChilds = new List<SwapSprite>();
        obj.GetComponentsInChildren<SwapSprite>(true, swapSpriteChilds);

        for (int i = 0; i < swapSpriteChilds.Count; i++)
        {
            SwapSprite swapSprite = swapSpriteChilds[i];

            if (swapSprite.sprites == null || swapSprite.sprites.Length == 0)//异常情况。                                                              
            {

                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < swapSprite.sprites.Length; j++)
            {
                //compInfos add element
                string atlasName = swapSprite.sprites[j].texture.name;
                string texName = swapSprite.sprites[j].name;
                SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
                spriteInfos.Add(spriteInfo);

            }

            CompInfo extCompSwapSpriteInfo = new ExtCompSwapSpriteInfo(
                    swapSprite.transform,
                    spriteInfos);
            compInfos.Add(extCompSwapSpriteInfo);

        }


        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject prefab)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();


        var swapSpriteChilds = new List<SwapSprite>();
        prefab.GetComponentsInChildren<SwapSprite>(true, swapSpriteChilds);
        Debug.Log(prefab.name + "'s SwapSprite count:" + swapSpriteChilds.Count);


        for (int i = 0; i < swapSpriteChilds.Count; i++)
        {
            SwapSprite swapSprite = swapSpriteChilds[i];

            if (swapSprite.sprites == null || swapSprite.sprites.Length == 0)//异常情况。                                                              
            {
                Debug.LogWarning("warning,SwapSprite:" + swapSprite.transform.GetHierarchy() + "  sprites not set");
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < swapSprite.sprites.Length; j++)
            {
                string atlasName = swapSprite.sprites[j].texture.name;

                //buildMap add element
                string texPath = AssetDatabase.GetAssetPath(swapSprite.sprites[j].texture);
                AssetBundleBuild build = new AssetBundleBuild();
                string preFix = ABTypeUtil.GetPreFix(ABType.Sprite);
                build.assetBundleName = preFix + atlasName.ToLower() + IOTools.abSuffix;
                build.assetNames = new string[] { texPath };
                buildMap.Add(build);
            }



        }


        return buildMap;
    }
}
