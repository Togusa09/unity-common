using UnityEditor;
using UnityEngine;

namespace Anarksoft.SharedCore.Editor
{
    public class SharedCoreWindow : EditorWindow
    {
        [MenuItem("Tools/Anarksoft/Shared Core Dashboard")]
        public static void OpenWindow()
        {
            GetWindow<SharedCoreWindow>("Shared Core");
        }

        private void OnGUI()
        {
            GUILayout.Label("Shared Core Package Dashboard", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This package is linked as a local package via Git Submodule.", MessageType.Info);
        }
    }
}
