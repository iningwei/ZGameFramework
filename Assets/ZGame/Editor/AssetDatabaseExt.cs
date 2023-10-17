using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetDatabaseExt
{
    public static TextureImporterType GetTextureImporterType(Texture tex)
    {
        string texPath = AssetDatabase.GetAssetPath(tex);
        var texType = (TextureImporterType)GetTextureImporterType(texPath);
        return texType;
    }
    public static int GetTextureImporterType(string path)
    {
        var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
        if (assetType.Name == typeof(UnityEngine.Texture2D).Name)
        {
            string metaFile = path + ".meta";
            var lines = IOTools.GetFileLines(metaFile);
            if (lines != null)
            {
                for (int j = 0; j < lines.Length; j++)
                {
                    var line = lines[j];
                    if (line.Contains("textureType:"))
                    {
                        int texTypeInt = int.Parse(line.Split(':')[1].Trim());
                        return texTypeInt;
                    }
                }
            }
        }
        Debug.LogError("get textureImporterType error," + path);
        return -1;
    }
}
