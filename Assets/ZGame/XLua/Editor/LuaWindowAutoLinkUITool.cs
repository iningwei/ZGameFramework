using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LuaWindowAutoLinkUITool : Editor
{

    static Dictionary<string, GameObject> subPrefabDic;
    [MenuItem("Assets/生成选择窗体的Lua序列化脚本")]
    public static void LinkUI()
    {
        subPrefabDic = new Dictionary<string, GameObject>();

        UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];


        for (int i = 0; i < assets.Length; i++)
        {

            //判断自身是否是预制件
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(assets[i]))
            {
                continue;
            }

            string path = AssetDatabase.GetAssetPath(assets[i]);
            Debug.LogError($"{path}是预制件");
            handlePrefabAsset(assets[i], path);
        }


        //////System.Func<Task> func = async () =>
        //////{
        //////    await Task.Delay(System.TimeSpan.FromSeconds(3));

        //////    //需要延迟执行的方法体...
        //////    //处理依赖的子预制件
        //////    if (subPrefabDic.Count > 0)
        //////    {
        //////        foreach (var item in subPrefabDic)
        //////        {
        //////            handlePrefabAsset(item.Value, item.Key);
        //////        }
        //////    }
        //////    Debug.Log("output auto link ui finished!!");
        //////};
        //////func();

        ////////处理依赖的子预制件
        //////if (subPrefabDic.Count > 0)
        //////{
        //////    foreach (var item in subPrefabDic)
        //////    {
        //////        handlePrefabAsset(item.Value, item.Key);
        //////    }
        //////}
        Debug.Log("output auto link ui finished!!");

    }

    private static void handlePrefabAsset(UnityEngine.Object prefabAsset, string path)
    {
        string assetName = prefabAsset.name;


        //Debug.LogError("path:" + path);
        string packageName = IOTools.GetFileFolderName(path);

        string desFolderPath = Application.dataPath + "/../../client/LuaScript/Windows/AutoExport/" + packageName;
        string desFilePath = desFolderPath + "/" + packageName + "_" + assetName + "ByName" + ".lua";
        if (!Directory.Exists(desFolderPath))
        {
            Directory.CreateDirectory(desFolderPath);
        }
        if (!File.Exists(desFilePath))
        {
            File.Create(desFilePath).Dispose();
        }



        GameObject rootObj = PrefabUtility.LoadPrefabContents(path);
        genLuaFile(rootObj, packageName, desFilePath);
    }

    private static void genLuaFile(GameObject rootObj, string packageName, string desFilePath)
    {

        StringBuilder requireSB = new StringBuilder();
        StringBuilder descSB = new StringBuilder();
        StringBuilder contentSB = new StringBuilder();
        //Debug.LogError("obj.name:" + obj.name);
        descSB.AppendFormat("\t\t---@return {0}_{1}\r\n", packageName, rootObj.name);
        descSB.Append("\t\t---@param ui UnityEngine.Transform\r\n");
        descSB.AppendFormat("function Get{0}_{1}Uis(ui)\r\n", packageName, rootObj.name);
        descSB.AppendFormat("\t\t---@class {0}_{1}\r\n", packageName, rootObj.name);
        descSB.AppendFormat("\t\t---@field public root UnityEngine.Transform\r\n");
        contentSB.Append("\t\tlocal uis={}\r\n");

        handleChild(rootObj, rootObj, requireSB, descSB, contentSB);

        contentSB.Append("\t\tuis.root=ui\r\n");
        contentSB.Append("\t\treturn uis\r\n");
        contentSB.Append("end");
        string finalStr = requireSB.ToString();
        finalStr += descSB.ToString();
        finalStr += contentSB.ToString();

        IOTools.WriteString(desFilePath, finalStr);

    }

    private static void handleChild(GameObject rootObj, GameObject parentObj, StringBuilder requireSB, StringBuilder descSB, StringBuilder contentSB)
    {
        for (int j = 0; j < parentObj.transform.childCount; j++)
        {
            var child = parentObj.transform.GetChild(j);

            //该子物体是预制体中的预制体
            if (PrefabUtility.IsPartOfPrefabInstance(child.gameObject))
            {
                Debug.LogError(child.name + "是预制件");
                string p = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child.gameObject);


                string packageNameSub = IOTools.GetFileFolderName(p);
                string assetNameSub = Path.GetFileNameWithoutExtension(p);
                //Debug.LogError("原始路径：" + p);
                requireSB.AppendFormat("require \'{0}_{1}ByName\'\r\n", packageNameSub, assetNameSub);
                descSB.AppendFormat("\t\t---@field public {0} {1}_{2}\r\n", child.name, packageNameSub, assetNameSub);
                contentSB.AppendFormat("\t\tuis.{0}=Get{1}_{2}Uis(ui:Find(\"{3}\"))\r\n", child.name, packageNameSub, assetNameSub, retrivePath(child, rootObj.transform));

                var asset = PrefabUtility.LoadPrefabContents(p);
                //////if (!subPrefabDic.ContainsKey(p))
                //////{
                //////    subPrefabDic.Add(p, asset);
                //////}
                handlePrefabAsset(asset, p);
            }
            else
            {
                genLuaItem(descSB, contentSB, child, retrivePath(child, rootObj.transform));
                handleChild(rootObj, child.gameObject, requireSB, descSB, contentSB);
            }
        }
    }

    static string retrivePath(Transform child, Transform root)
    {
        string path = "";
        Transform tmpChild = child;
        while (tmpChild != null && tmpChild != root)
        {
            path = tmpChild.name + "/" + path;
            tmpChild = tmpChild.parent;
        }
        path = path.TrimEnd('/');
        return path;
    }

    private static void genLuaItem(StringBuilder descSB, StringBuilder contentSB, Transform trans, string path)
    {
        //Image x;
        //x.color=Color
        //////x.GetComponent<RectTransform>().anchoredPosition =
        //Slider x;

        if (trans.name.EndsWith("Tran"))
        {
            descSB.AppendFormat("\t\t---@field public {0} UnityEngine.Transform\r\n", trans.name);
            contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\")\r\n", trans.name, path);
        }
        else if (trans.name.EndsWith("Btn"))
        {
            if (trans.name.EndsWith("PressBtn"))
            {
                descSB.AppendFormat("\t\t---@field public {0} ZGame.UGUIExtention.PressButton\r\n", trans.name);
                contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.ZGame.UGUIExtention.PressButton))\r\n", trans.name, path);
            }
            else if (trans.name.EndsWith("SwitchBtn"))
            {
                descSB.AppendFormat("\t\t---@field public {0} ZGame.UGUIExtention.SwitchButton\r\n", trans.name);
                contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.ZGame.UGUIExtention.SwitchButton))\r\n", trans.name, path);
            }
            else
            {
                descSB.AppendFormat("\t\t---@field public {0} UnityEngine.UI.Button\r\n", trans.name);
                contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.UnityEngine.UI.Button))\r\n", trans.name, path);
            }
        }
        else if (trans.name.EndsWith("Txt"))
        {
            descSB.AppendFormat("\t\t---@field public {0} UnityEngine.UI.Text\r\n", trans.name);
            contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.UnityEngine.UI.Text))\r\n", trans.name, path);
        }
        else if (trans.name.EndsWith("Img"))
        {
            descSB.AppendFormat("\t\t---@field public {0} UnityEngine.UI.Image\r\n", trans.name);
            contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.UnityEngine.UI.Image))\r\n", trans.name, path);
        }
        else if (trans.name.EndsWith("Slider"))
        {
            descSB.AppendFormat("\t\t---@field public {0} UnityEngine.UI.Slider\r\n", trans.name);
            contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.UnityEngine.UI.Slider))\r\n", trans.name, path);
        }
        else if (trans.name.EndsWith("Holder"))
        {
            descSB.AppendFormat("\t\t---@field public {0} UnityEngine.Transform\r\n", trans.name);
            contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\")\r\n", trans.name, path);
        }
        else if (trans.name.EndsWith("RadioButtonGroup"))
        {
            descSB.AppendFormat("\t\t---@field public {0} ZGame.UGUIExtention.RadioButtonGroup\r\n", trans.name);
            contentSB.AppendFormat("\t\tuis.{0}=ui:Find(\"{1}\"):GetComponent(typeof(CS.ZGame.UGUIExtention.RadioButtonGroup))\r\n", trans.name, path);
        }
        else
        {

        }

    }
}
