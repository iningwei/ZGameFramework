using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShaderCheckTool
{
    [MenuItem("工具/检测/项目中直接用到的shader")]
    public static void DoShaderCheck()
    {
        var shaders = new List<Shader>();
        var mats = Resources.FindObjectsOfTypeAll<Material>();
        foreach (var mat in mats)
        {
            if (!shaders.Contains(mat.shader) && mat.shader.name.IndexOf("Hidden/") != 0)
            {
                shaders.Add(mat.shader);
            }
        }
        foreach (var shader in shaders)
        {
            Debug.Log(shader.name);
        }
    }
}
