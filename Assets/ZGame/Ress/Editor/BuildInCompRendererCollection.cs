using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
using ZGame.RessEditor;

public class BuildInCompRendererCollection : IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();


        var rendererChilds = new List<Renderer>();
        obj.GetComponentsInChildren<Renderer>(true, rendererChilds);

        for (int i = 0; i < rendererChilds.Count; i++)
        {
            Renderer renderer = rendererChilds[i];
            if (renderer is SpriteRenderer)
            {
                continue;
            }

            if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)//�쳣�����                                                              
            {

                continue;
            }

            for (int j = 0; j < renderer.sharedMaterials.Length; j++)
            {
                //compInfos add elements
                List<TextureInfo> textureInfos = new List<TextureInfo>();
                var mat = renderer.sharedMaterials[j];
                if (mat == null)
                {
                    Debug.LogError("error, mat is null:" + renderer.gameObject.GetHierarchy());
                }
                if (BuildConfig.ignoredMats.Contains(mat.name))
                {
                    Debug.Log("mat ignore check:" + mat.name);
                    continue;
                }

                for (int k = 0; k < ShaderUtil.GetPropertyCount(mat.shader); k++)
                {
                    var t = ShaderUtil.GetPropertyType(mat.shader, k);
                    if (t == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyName = ShaderUtil.GetPropertyName(mat.shader, k);
                        Texture tex = mat.GetTexture(propertyName);

                        if (tex == null)
                        {
                            continue;
                        }
                        //���ʹ�õ�ͼƬ�Ƿ��ڹ�����
                        string texTmpPath = AssetDatabase.GetAssetPath(tex);
                        if (!texTmpPath.Contains("Assets"))
                        {
                            Debug.LogError("error, texPath:" + texTmpPath + ", renderer:" + renderer.gameObject);
                            continue;
                        }

                        string texName = tex.name;
                        TextureInfo textureInfo = new TextureInfo(texName, propertyName);
                        textureInfos.Add(textureInfo);
                    }
                }

                CompInfo compInfo = new BuildInCompRendererInfo(
                            renderer.transform,
                           mat,
                            mat.shader.name,
                            textureInfos);
                compInfos.Add(compInfo);
            }
        }

        return compInfos;
    }

    public List<AssetBundleBuild> GetResMap(GameObject obj)
    {
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();


        var rendererChilds = new List<Renderer>();
        obj.GetComponentsInChildren<Renderer>(true, rendererChilds);
        Debug.Log(obj.name + "'s Renderer count:" + rendererChilds.Count);


        for (int i = 0; i < rendererChilds.Count; i++)
        {
            Renderer renderer = rendererChilds[i];
            if (renderer is SpriteRenderer)
            {
                continue;
            }

            if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)//�쳣�����                                                              
            {
                Debug.LogWarning("warning,renderer:" + renderer.transform.GetHierarchy() + " material not set");
                continue;
            }

            for (int j = 0; j < renderer.sharedMaterials.Length; j++)
            {
                //compInfos add elements
                List<TextureInfo> textureInfos = new List<TextureInfo>();
                var mat = renderer.sharedMaterials[j];
                for (int k = 0; k < ShaderUtil.GetPropertyCount(mat.shader); k++)
                {
                    var t = ShaderUtil.GetPropertyType(mat.shader, k);
                    if (t == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyName = ShaderUtil.GetPropertyName(mat.shader, k);
                        Texture tex = mat.GetTexture(propertyName);

                        if (tex == null)
                        {
                            continue;
                        }
                        //���ʹ�õ�ͼƬ�Ƿ��ڹ�����
                        string texTmpPath = AssetDatabase.GetAssetPath(tex);
                        if (!texTmpPath.Contains("Assets"))
                        {
                            Debug.LogError("error, texPath:" + texTmpPath + ", renderer:" + renderer.gameObject);
                            continue;
                        }

                        string texName = tex.name;

                        //buildMap add elements                        
                        AssetBundleBuild build = new AssetBundleBuild();
                        string preFix = ABTypeUtil.GetPreFix(ABType.Texture);
                        build.assetBundleName = preFix + texName.ToLower() + IOTools.abSuffix;
                        build.assetNames = new string[] { texTmpPath };
                        buildMap.Add(build);
                    }
                }
            }
        }
        return buildMap;
    }
}
