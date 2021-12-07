using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShaderReplacement : ScriptableWizard
{
    //////public string oldShaderName;
    //////public string newShaderName;

    //////[MenuItem("ZGame/Shader/Shader替换")]
    //////static void ShaderReplacementWizard()
    //////{
    //////    ScriptableWizard.DisplayWizard("Shader替换", typeof(ShaderReplacement), "Confirm", "Cancel");
    //////}

    //////private void OnWizardCreate()
    //////{
    //////    this.doReplace();
    //////    Close();
    //////}
    //////private void OnWizardOtherButton()
    //////{
    //////    Close();
    //////}

    //////void doReplace()
    //////{
    //////    var mats = Resources.FindObjectsOfTypeAll<Material>();
    //////    foreach (var mat in mats)
    //////    {
    //////        if (mat.shader.name == oldShaderName)
    //////        {
    //////            mat.shader = Shader.Find(newShaderName);
    //////        }
    //////    }
    //////}
    ///

    static Dictionary<string, string> shaderDic;

    [MenuItem("ZGame/Shader/Shader替换")]
    static void BuildInShaderReplace()
    {

        shaderDic = new Dictionary<string, string>();
        shaderDic.Add("Mobile/Diffuse", "My/Mobile/Diffuse");
        shaderDic.Add("Mobile/Bumped Diffuse", "My/Mobile/Bumped Diffuse");

        shaderDic.Add("Legacy Shaders/Diffuse", "My/Legacy Shaders/Diffuse");
        shaderDic.Add("Legacy Shaders/Bumped Diffuse", "My/Legacy Shaders/Bumped Diffuse");

        shaderDic.Add("Legacy Shaders/Transparent/Diffuse", "My/Legacy Shaders/Transparent/Diffuse");
        shaderDic.Add("Legacy Shaders/Transparent/Bumped Diffuse", "My/Legacy Shaders/Transparent/Bumped Diffuse");


        shaderDic.Add("Standard", "My/Standard");
        shaderDic.Add("Standard (Specular setup)", "My/Standard (Specular setup)");

       
       



        var mats = Resources.FindObjectsOfTypeAll<Material>();
        foreach (var mat in mats)
        {
            if (shaderDic.ContainsKey(mat.shader.name))
            {
                Debug.LogError($"替换shader, mat:{mat.name}, oldShader:{mat.shader.name},newShader:{shaderDic[mat.shader.name]}");
                mat.shader = Shader.Find(shaderDic[mat.shader.name]);
            }
        }
        Debug.LogError("替换完成！");
    }
}
