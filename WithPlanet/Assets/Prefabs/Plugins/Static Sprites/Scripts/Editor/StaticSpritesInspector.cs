using UnityEditor;
using UnityEngine;

namespace StaticSprites {
    
    [CustomEditor(typeof(StaticSprites))]
    public class StaticSpritesInspector : Editor {
        public override void OnInspectorGUI() {
            var ss = serializedObject.targetObject as StaticSprites;
            base.OnInspectorGUI();
            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Export combined sprites", 
                tooltip: "Optional button which saves out the combined sprite meshes and materials."), 
                GUILayout.Width(240)))
                ss.Export();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
