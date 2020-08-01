using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TimeLineSupportScript))]
    public class TimeLineSupport : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TimeLineSupportScript timeLineSupportScript = (TimeLineSupportScript) target;
            if (GUILayout.Button("反序列化"))
            {
                timeLineSupportScript.Deserialize();
            }

            if (GUILayout.Button("导入 Timeline"))
            {
                timeLineSupportScript.ImportTimeline();
            }

            if (GUILayout.Button("导出 Timeline"))
            {
                timeLineSupportScript.ExportTimeline();
            }
        }
    }
}