using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;

namespace ZGame.RessEditor
{
    /// <summary>
    /// AssetBundle打包配置类
    /// </summary>
    public class BuildConfig
    {
        public static BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;

        static string baseOutputPath = null;
        public static string outputPath
        {
            get
            {
                return baseOutputPath;
            }
        }



        public static void Init()
        {
            //generator ab output path
            if (baseOutputPath == null)
            {
                baseOutputPath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
            }
            if (!Directory.Exists(baseOutputPath))
            {
                Directory.CreateDirectory(baseOutputPath);
            }

            Debug.Log("baseOutputPath:" + baseOutputPath);

            //generator temp path for prefab
            string tmpPath = Application.dataPath + "/temp_for_prefab";
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
        }


        //资源配置项
        public class ResBuildConfig
        {
            public string rootPath;
            public Func<UnityEngine.Object, bool> buildFunc;
        }


        public static Dictionary<ABType, ResBuildConfig> allRes = new Dictionary<ABType, ResBuildConfig>()
    {
        //精灵
        {
            ABType.Sprite,
            new ResBuildConfig
            {
                rootPath="Assets/ArtResources/Sprite",
                buildFunc=new BuildSprite().Build
            }

        },
        //图片
        {
            ABType.Texture,
            new ResBuildConfig
            {
                rootPath="Assets/ArtResources/Texture",
                buildFunc=new BuildTexture().Build
            }
        },
                         
        //音效
        {
            ABType.Audio,
            new ResBuildConfig
            {
                rootPath="Assets/ArtResources/Audio",
                buildFunc=new BuildAudio().Build
            }
        }
    };

        /// <summary>
        /// 通过资源路径，获得其编译函数
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static Func<UnityEngine.Object, bool> getBuildFunc(string resPath, object asset)
        {
            //预制件要通过holder中的abType来确定
            if (resPath.EndsWith(".prefab"))
            {
                var holder = (asset as GameObject).GetComponent<MatTextureHolder>();
                if (holder == null)
                {
                    Debug.LogError("no MatTextureHolder attach to:" + resPath);
                    return null;
                }
                else
                {
                    if (holder.abType == ABType.Window)
                    {
                        return new BuildWindow().Build;
                    }
                    else if (holder.abType == ABType.Effect)
                    {
                        return new BuildEffect().Build;
                    }
                    else if (holder.abType == ABType.OtherPrefab)
                    {
                        return new BuildOtherPrefab().Build;
                    }
                }
            }



            foreach (var item in allRes)
            {
                if (resPath.Contains(item.Value.rootPath))
                {
                    return item.Value.buildFunc;
                }

                if (resPath.Contains("Sprite"))
                {
                    return new BuildSprite().Build;
                }
                if (resPath.Contains("Texture"))
                {
                    return new BuildTexture().Build;
                }
                if (resPath.Contains("Audio"))
                {
                    return new BuildAudio().Build;
                }
            }

            return new BuildBase().Build;
        }
    }
}