using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class ExtCompImageSequenceCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        var imageSequenceChilds = new List<ImageSequence>();
        obj.GetComponentsInChildren<ImageSequence>(true, imageSequenceChilds);

        for (int i = 0; i < imageSequenceChilds.Count; i++)
        {
            ImageSequence imageSequence = imageSequenceChilds[i];

            if (imageSequence.sprites == null || imageSequence.sprites.Length == 0)//异常情况。                                                              
            {
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < imageSequence.sprites.Length; j++)
            {
                //compInfos add element
                string atlasName = imageSequence.sprites[j].texture.name;
                string texName = imageSequence.sprites[j].name;
                SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
                spriteInfos.Add(spriteInfo);
            }

            CompInfo buildInCompImageInfo = new ExtCompImageSequenceInfo(
                    imageSequence.transform,
                    spriteInfos);
            compInfos.Add(buildInCompImageInfo);

        }
        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

        var imageSequenceChilds = new List<ImageSequence>();
        obj.GetComponentsInChildren<ImageSequence>(true, imageSequenceChilds);
        Debug.Log(obj.name + "'s ImageSequence count:" + imageSequenceChilds.Count);


        for (int i = 0; i < imageSequenceChilds.Count; i++)
        {
            ImageSequence imageSequence = imageSequenceChilds[i];

            if (imageSequence.sprites == null || imageSequence.sprites.Length == 0)//异常情况。                                                              
            {
                Debug.LogWarning("warning,imageSequence:" + imageSequence.transform.GetHierarchy() + "  sprites not set");
                continue;
            }

            List<SpriteInfo> spriteInfos = new List<SpriteInfo>();
            for (int j = 0; j < imageSequence.sprites.Length; j++)
            {

                string atlasName = imageSequence.sprites[j].texture.name;
                //buildMap add element
                string texPath = AssetDatabase.GetAssetPath(imageSequence.sprites[j].texture);
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
