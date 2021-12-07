using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class BuildInCompSpriteRendererCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {

        List<CompInfo> compInfos = new List<CompInfo>();


        var spriteRendererChilds = new List<SpriteRenderer>();
        obj.GetComponentsInChildren<SpriteRenderer>(true, spriteRendererChilds);


        for (int i = 0; i < spriteRendererChilds.Count; i++)
        {
            SpriteRenderer spriteRenderer = spriteRendererChilds[i];

            if (spriteRenderer.sprite == null || spriteRenderer.material == null)//sprite��material��һ��Ϊnull,�������쳣�����                                                              
            {
                continue;
            }

            //compInfos add element
            string atlasName = spriteRenderer.sprite.texture.name;
            string texName = spriteRenderer.sprite.name;
            SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
            List<SpriteInfo> spriteInfos = new List<SpriteInfo>() { spriteInfo };
            CompInfo buildInCompImageInfo = new BuildInCompImageInfo(
                spriteRenderer.transform,
                spriteRenderer.material,
                spriteRenderer.material.shader.name,
                spriteInfos);
            compInfos.Add(buildInCompImageInfo);
        }

        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
        var spriteRendererChilds = new List<SpriteRenderer>();
        obj.GetComponentsInChildren<SpriteRenderer>(true, spriteRendererChilds);
        Debug.Log(obj.name + "'s SpriteRenderer count:" + spriteRendererChilds.Count);


        for (int i = 0; i < spriteRendererChilds.Count; i++)
        {
            SpriteRenderer spriteRenderer = spriteRendererChilds[i];

            if (spriteRenderer.sprite == null || spriteRenderer.material == null)//sprite��material��һ��Ϊnull,�������쳣�����                                                              
            {
                Debug.LogWarning("warning,SpriteRenderer:" + spriteRenderer.transform.GetHierarchy() + "  sprite not set or  material not set");
                continue;
            }



            string atlasName = spriteRenderer.sprite.texture.name;
            //buildMap add element
            string texPath = AssetDatabase.GetAssetPath(spriteRenderer.sprite.texture);
            AssetBundleBuild build = new AssetBundleBuild();
            string preFix = ABTypeUtil.GetPreFix(ABType.Sprite);
            build.assetBundleName = preFix + atlasName.ToLower() + IOTools.abSuffix;
            build.assetNames = new string[] { texPath };
            buildMap.Add(build);

        }

        return buildMap;
    }
}
