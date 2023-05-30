using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class ExtCompSpriteSequenceCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        var spriteSequenceChilds = new List<SpriteSequence>();
        obj.GetComponentsInChildren<SpriteSequence>(true, spriteSequenceChilds);

        for (int i = 0; i < spriteSequenceChilds.Count; i++)
        {
            SpriteSequence spriteSequence = spriteSequenceChilds[i];

            if (spriteSequence.sprites == null || spriteSequence.sprites.Length == 0)//异常情况。                                                              
            {
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < spriteSequence.sprites.Length; j++)
            {
                //compInfos add element
                string atlasName = spriteSequence.sprites[j].texture.name;
                string texName = spriteSequence.sprites[j].name;
                SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
                spriteInfos.Add(spriteInfo);
            }

            CompInfo extCompSpriteSequenceInfo = new ExtCompSpriteSequenceInfo(
                    spriteSequence.transform,
                    spriteInfos);
            compInfos.Add(extCompSpriteSequenceInfo);

        }

        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

        var spriteSequenceChilds = new List<SpriteSequence>();
        obj.GetComponentsInChildren<SpriteSequence>(true, spriteSequenceChilds);
        Debug.Log(obj.name + "'s SpriteSequence count:" + spriteSequenceChilds.Count);


        for (int i = 0; i < spriteSequenceChilds.Count; i++)
        {
            SpriteSequence spriteSequence = spriteSequenceChilds[i];

            if (spriteSequence.sprites == null || spriteSequence.sprites.Length == 0)//异常情况。                                                              
            {
                Debug.LogWarning("warning,SpriteSequence:" + spriteSequence.transform.GetHierarchy() + "  sprites not set");
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < spriteSequence.sprites.Length; j++)
            {

                string atlasName = spriteSequence.sprites[j].texture.name;

                //buildMap add element
                string texPath = AssetDatabase.GetAssetPath(spriteSequence.sprites[j].texture);
                if (texPath.Contains("Assets/UI"))
                {
                    Debug.LogError("do not use tex in Assets/UI, path:" + spriteSequence.transform.GetHierarchy());
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