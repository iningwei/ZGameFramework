using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �������A�����ĳ������B������
/// ��Ч�ڰ�����A�ƶ�������B�£����A�����B�ı�������
/// </summary>
public class GetScreenPointCorrespondUGUIPos : EditorWindow
{
    public static Camera cam;
    public static RectTransform targetRect;




    [MenuItem("GameObject/GetScreenPointCorrespondUGUIPos", false, 15)]
    public static void Get()
    {
        Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as Object[];
        if (assets != null && assets.Length > 0 && assets[0] is RectTransform)
        {
            targetRect = assets[0] as RectTransform;
        }

        GetScreenPointCorrespondUGUIPos window = (GetScreenPointCorrespondUGUIPos)EditorWindow.GetWindow(typeof(GetScreenPointCorrespondUGUIPos), false, "GetScreenPointCorrespondUGUIPos", true);
        window.Show();


    }

    private void Update()
    {
        if (targetRect == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPos;
            Vector2 screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, screenPos, cam, out localPos);
            Debug.LogError($"screenPos:{screenPos}, correspond pos:" + localPos);
        }
    }



    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("targetRect:");
        targetRect = EditorGUILayout.ObjectField(targetRect, typeof(RectTransform), true, GUILayout.Width(200)) as RectTransform;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("cam:");
        cam = EditorGUILayout.ObjectField(cam, typeof(Camera), true, GUILayout.Width(200)) as Camera;
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space(20);

    }

}
