
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

public class CompResCollection
{
    public void AddBundleBuildData(string assetName, string assetPath, ABType abType, ref Dictionary<string, AssetBundleBuild> buildMap)
    {
        //buildMap若已包含assetPath则不可再加，否则打ab报错
        foreach (var item in buildMap)
        {
            for (int i = 0; i < item.Value.assetNames.Length; i++)
            {
                if (item.Value.assetNames[i] == assetPath)
                {
                    return;
                }
            }
        }


        string bundleName = ABTypeUtil.GetPreFix(abType) + assetName.ToLower() + IOTools.abSuffix;
        if (buildMap.ContainsKey(bundleName) == false)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = new string[] { assetPath };
            buildMap.Add(bundleName, build);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fbxFilePath">start with:Assets/</param>
    public void ExportMeshToTmpFolder(string fbxFilePath)
    {
        if (!fbxFilePath.EndsWith(".fbx") && !fbxFilePath.EndsWith(".FBX"))
        {
            Debug.LogError("it is not fbx/FBX:" + fbxFilePath);
            return;
        }
        string fbxFileName = Path.GetFileNameWithoutExtension(Path.Combine(Application.dataPath, fbxFilePath));

        string tmpFolder = Path.Combine(Application.dataPath, "temp_for_fbx/" + fbxFileName);
        if (!Directory.Exists(tmpFolder))
        {
            Directory.CreateDirectory(tmpFolder);
        }
        string outputFolder = "Assets/" + "/temp_for_fbx/" + fbxFileName;
        //create meshes
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(fbxFilePath);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is Mesh)
            {
                Mesh mesh = Object.Instantiate(objects[i]) as Mesh;

                string outputPath = outputFolder + "/" + objects[i].name + ".mesh";

                AssetDatabase.CreateAsset(mesh, outputPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public string GetMeshPathFromTmpFolder(string name)
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/temp_for_fbx", "*.mesh", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName == name)
                {
                    return filePath.Substring(filePath.IndexOf("Assets"));
                }
            }
        }
        return "";
    }


    public bool IsStripIgnoredFBX(GameObject[] stripIgnoredFBXs, string fbxPath)
    {
        if (stripIgnoredFBXs != null && stripIgnoredFBXs.Length > 0)
        {
            for (int i = 0; i < stripIgnoredFBXs.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(stripIgnoredFBXs[i]);
                if (path == fbxPath)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
