using UnityEditor;
using UnityEngine;

namespace Futurus.RemoteInput.Editor
{
    [CustomEditor(typeof(RemoteInputModule))]
    public class RemoteInputModuleEditor : UnityEditor.Editor
    {
        RemoteInputModule _targetObject;

        private void OnEnable()
        {
            _targetObject = serializedObject.targetObject as RemoteInputModule;
        }
        public override void OnInspectorGUI()
        {
            if (!_targetObject)
                return;

            DrawDefaultInspector();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            GUILayout.Label($"Raycasters: {RemoteInputRaycaster.AllRaycasters.Count}");
            GUILayout.Label($"Providers: {_targetObject.ProviderCount}");
            EditorGUI.EndDisabledGroup();
        }
    }
}

