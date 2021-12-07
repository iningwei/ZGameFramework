using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CircleImage))]
public class CircleImageEditor : ImageEditor
{

    SerializedProperty fillPercent;
    SerializedProperty fillNum;

    protected override void OnEnable()
    {
        base.OnEnable();
        fillPercent = serializedObject.FindProperty("fillPercent");
        fillNum = serializedObject.FindProperty("fillNum");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        fillPercent.floatValue = EditorGUILayout.Slider("填充比例", fillPercent.floatValue, 0, 1);
        fillNum.intValue = EditorGUILayout.IntSlider("填充个数", fillNum.intValue, 1, 100);

        serializedObject.ApplyModifiedProperties();
    }
}