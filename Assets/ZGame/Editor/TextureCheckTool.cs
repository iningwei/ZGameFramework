using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class TextureCheckTool
{


    [MenuItem("ZGame/图片检测/mipmap")]
    public static void CheckMipMap()
    {
        string[] guidsTexture = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" });
        string[] guidsSprite = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
        List<string> guidList = new List<string>();
        guidList.AddRange(guidsTexture);
        guidList.AddRange(guidsSprite);

        for (int i = 0; i < guidList.Count; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guidList[i]);
            //Debug.Log("----->" + filePath);
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(filePath);//对于TextureType为Sprite(2D and UI)类型的图，获得的类型也是Texture2D

            if (t == typeof(UnityEngine.Texture2D))
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                //mipmapCount=1则未开启，mipmapCount>1则开启了Mip Maps
                if (tex.mipmapCount != 1)
                {
                    Debug.Log(filePath + "开启了Mip Maps，请关闭。tex.mipmapCount：" + tex.mipmapCount);
                }


            }
        }

        Debug.Log("检测完毕！");
    }


    [MenuItem("ZGame/图片检测/Size")]
    public static void CheckSize()
    {
        string[] guidsTexture = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" });
        string[] guidsSprite = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
        List<string> guidList = new List<string>();
        guidList.AddRange(guidsTexture);
        guidList.AddRange(guidsSprite);

        for (int i = 0; i < guidList.Count; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guidList[i]);
            //Debug.Log("----->" + filePath);
            System.Type t = AssetDatabase.GetMainAssetTypeAtPath(filePath);//对于TextureType为Sprite(2D and UI)类型的图，获得的类型也是Texture2D

            if (t == typeof(UnityEngine.Texture2D))
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);


                if (tex.width >= 1024 || tex.height >= 1024)
                {
                    if (tex.width >= 2048 || tex.height >= 2048)
                    {
                        Debug.LogError(filePath + "，尺寸太大，width:" + tex.width + ", height:" + tex.height);
                    }
                    else
                    {
                        Debug.LogWarning(filePath + "，尺寸过大，width:" + tex.width + ", height:" + tex.height);
                    }
                }
            }
        }

        Debug.Log("检测完毕！");
    }


    /// <summary>
    /// 检测图片资源
    /// 遍历目标目录下的预制件，查找预制件引用到的图片，检测图片
    /// 尺寸>=512或者开了mipmap的图片需要提示用户
    /// </summary>
    [MenuItem("GameTool/CheckTexture")]
    public static void CheckTexture()
    {
        List<string> mipMapErrorList = new List<string>();
        List<string> sizeErrorList = new List<string>();


        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int length = guids.Length;
        //Debug.Log("prefab count:" + length);
        for (int i = 0; i < length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (!filePath.Contains("Effects") || filePath.Contains("NOT_USED"))
            {
                continue;
            }

            //检查该prefab的依赖项
            string[] dependencies = AssetDatabase.GetDependencies(filePath);
            if (dependencies == null || dependencies.Length == 0)
            {
                continue;
            }

            for (int j = 0; j < dependencies.Length; j++)
            {
                string path = dependencies[j];
                //AssetDatabase.LoadAssetAtPath(dependencies[j],)
                //Debug.Log("----->" + path);
                System.Type t = AssetDatabase.GetMainAssetTypeAtPath(path);//对于TextureType为Sprite(2D and UI)类型的图，获得的类型也是Texture2D

                if (t == typeof(UnityEngine.Texture2D))
                {
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    //mipmapCount=1则未开启，mipmapCount>1则开启了Mip Maps
                    if (tex.mipmapCount != 0)
                    {
                        if (!mipMapErrorList.Contains(path))
                        {
                            Debug.Log(path + "开启了Mip Maps，请关闭");
                            mipMapErrorList.Add(path);
                        }
                    }

                    if (tex.width >= 512 || tex.height >= 512)
                    {
                        if (!sizeErrorList.Contains(path))
                        {
                            if (tex.width >= 1024 || tex.height >= 1024)
                            {
                                Debug.LogError(path + "，尺寸太大，width:" + tex.width + ", height:" + tex.height);
                            }
                            else
                            {
                                Debug.LogWarning(path + "，尺寸过大，width:" + tex.width + ", height:" + tex.height);
                            }

                            sizeErrorList.Add(path);
                        }
                    }

                }
            }
        }

        Debug.Log("检测完毕！");
    }

    [MenuItem("GameTool/CheckSameRes")]
    public static void CheckSameNameTexture()
    {
        string texturePath = Path.Combine(Application.dataPath, "ArtResources");
        Dictionary<string, bool> imgs = new Dictionary<string, bool>();
        DirectoryInfo textureDir = new DirectoryInfo(texturePath);

        //|*.png
        //获取全部图片文件
        List<FileInfo> files = new List<FileInfo>();
        files.AddRange(textureDir.GetFiles("*.png", SearchOption.AllDirectories));
        files.AddRange(textureDir.GetFiles("*.jpg", SearchOption.AllDirectories));
        files.AddRange(textureDir.GetFiles("*.prefab", SearchOption.AllDirectories));
        files.AddRange(textureDir.GetFiles("*.mat", SearchOption.AllDirectories));

        foreach (var item in files)
        {
            string filename = item.Name.Replace(item.Extension, "");

            if (imgs.ContainsKey(filename))
            {
                if (!imgs[filename])
                {
                    imgs[item.Name] = true;
                    Debug.LogError("重复文件：" + item.Name);
                }
            }
            else
            {
                imgs[filename] = false;
            }
        }

        Debug.LogError("检查完毕");
    }

}
