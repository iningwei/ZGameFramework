using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

public class HierarchyLayoutExtention : EditorWindow
{
    string sequenceChildNamePrefix = "";
    int sequenceChildStartIndex = 0;
    GameObject sequenceChildParent = null;

    GameObject arrangeChildParent = null;
    Axis targetAxis = Axis.X;
    float arrangeGap = 10f;

    [MenuItem("工具/界面排版布局", false, 13)]
    static void HierarchyLayout()
    {
        HierarchyLayoutExtention visualizeTool = EditorWindow.GetWindow(typeof(HierarchyLayoutExtention)) as HierarchyLayoutExtention;
    }


    private void OnGUI()
    {
        //有序重命名子物体
        GUILayout.Label("有序重命名子物体", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("父物体:");
        sequenceChildParent = EditorGUILayout.ObjectField(sequenceChildParent, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("子物体前缀:");
        sequenceChildNamePrefix = EditorGUILayout.TextField(sequenceChildNamePrefix);
        GUILayout.Label("起始索引编号:");
        sequenceChildStartIndex = EditorGUILayout.IntField(sequenceChildStartIndex);
        if (GUILayout.Button("重命名"))
        {
            var childs = (sequenceChildParent as GameObject).GetChilds(false, true);
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].name = sequenceChildNamePrefix + (i + sequenceChildStartIndex).ToString();
            }

            Debug.Log("重命名完毕！");
        }
        GUILayout.EndHorizontal();

        //子物体以第1个物体为依准，z方向上按照正反方向依次等间距排列
        GUILayout.Space(20f);
        GUILayout.Label("以第1个子物体为准，z方向上按照正反方向依次等间距排列子物体", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("父物体:");
        arrangeChildParent = EditorGUILayout.ObjectField(arrangeChildParent, typeof(GameObject), true) as GameObject;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("目标方向:");
        targetAxis = (UnityEngine.Animations.Axis)EditorGUILayout.EnumPopup(targetAxis);
        GUILayout.Label("间距：");
        arrangeGap = EditorGUILayout.FloatField(arrangeGap);

        if (GUILayout.Button("排列"))
        {
            var childs = (arrangeChildParent as GameObject).GetChilds(false, true);
            if (childs != null && childs.Count > 1)
            {
                Transform refTran = childs[0].transform;
                Vector3 originPos = refTran.localPosition;
                for (int i = 1; i < childs.Count; i++)
                {
                    if (targetAxis == Axis.Z)
                    {
                        if (i % 2 == 1)
                        {
                            childs[i].transform.SetPosZAccordRefTran(refTran, originPos.z + (i / 2 + 1) * arrangeGap, Space.Self);
                        }
                        else
                        {
                            childs[i].transform.SetPosZAccordRefTran(refTran, originPos.z - (i / 2) * arrangeGap, Space.Self);
                        }
                    }
                    else if (targetAxis == Axis.X)
                    {
                        if (i % 2 == 1)
                        {
                            childs[i].transform.SetPosXAccordRefTran(refTran, originPos.x + (i / 2 + 1) * arrangeGap, Space.Self);
                        }
                        else
                        {
                            childs[i].transform.SetPosXAccordRefTran(refTran, originPos.x - (i / 2) * arrangeGap, Space.Self);
                        }
                    }
                    else if (targetAxis == Axis.Y)
                    {
                        if (i % 2 == 1)
                        {
                            childs[i].transform.SetPosYAccordRefTran(refTran, originPos.y + (i / 2 + 1) * arrangeGap, Space.Self);
                        }
                        else
                        {
                            childs[i].transform.SetPosYAccordRefTran(refTran, originPos.y - (i / 2) * arrangeGap, Space.Self);
                        }
                    }

                }
            }

            Debug.Log("排列完毕！");
        }
        GUILayout.EndHorizontal();
    }
}
