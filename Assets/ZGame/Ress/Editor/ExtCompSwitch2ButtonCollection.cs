using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
using ZGame.UGUIExtention;

public class ExtCompSwitch2ButtonCollection : IRefResCollection
{


    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        var switch2ButtonChilds = new List<Switch2Button>();
        obj.GetComponentsInChildren<Switch2Button>(true, switch2ButtonChilds);

        for (int i = 0; i < switch2ButtonChilds.Count; i++)
        {
            Switch2Button switch2Btn = switch2ButtonChilds[i];

            if (switch2Btn.switchSprites == null || switch2Btn.switchSprites.Length == 0)
            {
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < switch2Btn.switchSprites.Length; j++)
            {
                //compInfos add element
                string atlasName = switch2Btn.switchSprites[j].texture.name;
                string texName = switch2Btn.switchSprites[j].name;
                SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
                spriteInfos.Add(spriteInfo);
            }

            CompInfo extCompSwitch2BtnInfo = new ExtCompSwitch2ButtonInfo(switch2Btn.transform, spriteInfos);
            compInfos.Add(extCompSwitch2BtnInfo);
        }

        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject prefab)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();


        var switch2ButtonChilds = new List<Switch2Button>();
        prefab.GetComponentsInChildren<Switch2Button>(true, switch2ButtonChilds);
        Debug.Log(prefab.name + "'s Switch2Button count:" + switch2ButtonChilds.Count);


        for (int i = 0; i < switch2ButtonChilds.Count; i++)
        {
            Switch2Button switch2Btn = switch2ButtonChilds[i];

            if (switch2Btn.switchSprites == null || switch2Btn.switchSprites.Length == 0)//异常情况。                                                              
            {
                Debug.LogWarning("warning,SwapSprite:" + switch2Btn.transform.GetHierarchy() + "  sprites not set");
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < switch2Btn.switchSprites.Length; j++)
            {
                string atlasName = switch2Btn.switchSprites[j].texture.name;

                //buildMap add element
                string texPath = AssetDatabase.GetAssetPath(switch2Btn.switchSprites[j].texture);

                if (texPath.Contains("Assets/UI"))
                {
                    Debug.LogError("do not use origin tex under Assets/UI, path:" + switch2Btn.transform.GetHierarchy());
                    continue;
                }


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
