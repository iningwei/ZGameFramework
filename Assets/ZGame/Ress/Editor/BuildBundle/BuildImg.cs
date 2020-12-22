﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ZGame.RessEditor
{
    public class BuildImg : BuildBase
    {
        public override bool Build(Object obj)
        {
            if (obj.name.ContainChinese())
            {
                Debug.LogError("resource name should not contail chinese charactor:" + obj.name);
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