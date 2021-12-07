using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;
using ZGame;

namespace ZGame.RessEditor
{
    public class BuildPrefab : BuildBase
    {
        public override bool Build(Object obj)
        {

            if (obj.name.ContainChinese())
            {
                Debug.LogError("resource name should not contain chinese charactor:" + obj.name);
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

            //填充 RootCompInfoHolder
            BuildCommand.FillRootCompInfoHolder(tmpPrefab);
            //填充 DynamicCompInfoHolder
            BuildCommand.FillDynamicCompInfoHolder(tmpPrefab);

            //保存Holder信息
            EditorUtility.SetDirty(tmpPrefab);
            AssetDatabase.SaveAssets();


            List<AssetBundleBuild> finalTexMap = BuildCommand.GetGameObjectAssetBundleBuildMap(tmpPrefab);

            Dictionary<string, bool> texDic = new Dictionary<string, bool>();

            //处理真实目标,以及设置依赖关系         

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetBundleBuild[] buildMap = new AssetBundleBuild[2 + finalTexMap.Count];
            buildMap[0] = BuildCommand.GetCommonMap();
            for (int i = 0; i < finalTexMap.Count; i++)
            {

                texDic[finalTexMap[i].assetBundleName] = true;
                buildMap[i + 1] = finalTexMap[i];
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