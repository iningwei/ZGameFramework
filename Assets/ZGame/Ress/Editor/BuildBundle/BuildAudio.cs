using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    public class BuildAudio : BuildBase
    {
        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.Audio);
            if (obj.name.ContainChinese())
            {
                Debug.LogError("资源名不能有中文字符，" + obj.name);
                return false;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
            buildMap[0].assetNames = new string[] { path };
            BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, buildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return true;
        }
    }
}