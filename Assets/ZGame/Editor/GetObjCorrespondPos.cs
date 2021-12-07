using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



/// <summary>
/// �������A�����ĳ������B������
/// ��Ч�ڰ�����A�ƶ�������B�£����A�����B�ı�������
/// </summary>
public class GetObjCorrespondPos : EditorWindow
{
    private static GameObject targetObj;
    private static GameObject correspondObj;

    [MenuItem("GameObject/GetObjCorrespondPos", false, 14)]
    public static void Get()
    {
        Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as Object[];
        if (assets != null && assets.Length > 0 && assets[0] is GameObject)
        {
            targetObj = assets[0] as GameObject;
        }

        GetObjCorrespondPos window = (GetObjCorrespondPos)EditorWindow.GetWindow(typeof(GetObjCorrespondPos), false, "GetObjCorrespondPos", true);
        window.Show();
    }




    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("targetObj:");
        targetObj = EditorGUILayout.ObjectField(targetObj, typeof(GameObject), true, GUILayout.Width(200)) as GameObject;
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("correspondObj:");
        correspondObj = EditorGUILayout.ObjectField(correspondObj, typeof(GameObject), true, GUILayout.Width(200)) as GameObject;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);
        if (GUILayout.Button("Get Pos", GUILayout.Width(100)))
        {

            var worldPos = targetObj.transform.TransformPoint(Vector3.zero);
            Debug.LogError("targetObj worldPos:" + worldPos);

            var correspondPos = correspondObj.transform.InverseTransformPoint(worldPos);
            Debug.LogError("targetObj correspondPos:" + correspondPos);
        }
    }
}
