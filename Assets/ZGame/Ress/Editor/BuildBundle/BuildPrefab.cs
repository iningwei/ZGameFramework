using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZGame.RessEditor
{
    public class BuildPrefab : BuildBase
    {
        public override bool Build(Object obj)
        {
            
            if (obj.name.ContainChinese())
            {
                Debug.LogError("resource name should not contail chinese charactor:" + obj.name);
                return false;
            }

            if (CheckPrefab(obj as GameObject) == false)
            {
                return false;
            }
            string path = AssetDatabase.GetAssetPath(obj);

            //创建临时预制体，用来操作
            string tmpPath = "Assets/temp_for_prefab/" + obj.name + ".prefab";
            AssetDatabase.DeleteAsset(tmpPath);
            AssetDatabase.CopyAsset(path, tmpPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var tmpPrefab = AssetDatabase.LoadAssetAtPath(tmpPath, typeof
                (GameObject)) as GameObject;
            if (tmpPrefab == null)
            {
                Debug.LogError("load temp prefab error, path:" + tmpPath);
                return false;
            }


            var texMap = GetMatTexMap(path, tmpPrefab);
      
            
            //处理真实目标,以及设置依赖关系
            EditorUtility.SetDirty(tmpPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetBundleBuild[] buildMap = new AssetBundleBuild[2 + texMap.Count];
            buildMap[0] = BuildCommand.GetCommonMap();
            for (int i = 0; i < texMap.Count; i++)
            {
                buildMap[i + 1] = texMap[i];
            }
            buildMap[buildMap.Length - 1].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
            buildMap[buildMap.Length - 1].assetNames = new string[] { tmpPath };

            BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, buildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);

            //删除临时资源
            //AssetDatabase.DeleteAsset(tmpPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return true;
        }
    }
}