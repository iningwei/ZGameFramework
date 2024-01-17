using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class TableOutput : Editor
{
    [MenuItem("工具/导表")]
    static void Output()
    {
        string path = Application.dataPath + "/../table_tools/导表.bat";
        Process.Start(path);

    }
    
    [MenuItem("工具/打开Excel配置表文件夹")]
    private static void OpenExcelFolder()
    {
        string path = Application.dataPath + "/../table_tools/table";
        Process.Start(path);
    }
}
