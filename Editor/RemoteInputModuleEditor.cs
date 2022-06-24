using UnityEditor;
using UnityEngine;

namespace Futurus.RemoteInput.Editor
{
    [CustomEditor(typeof(RemoteInputModule))]
    public class RemoteInputModuleEditor : UnityEditor.Editor
    {
        RemoteInputModule _targetObject;

        int _raycastersCount = -1;
        string _raycasterNames = "";

        private void OnEnable()
        {
            _targetObject = serializedObject.targetObject as RemoteInputModule;
            _raycastersCount = -1;
        }
        public override void OnInspectorGUI()
        {
            if (!_targetObject)
                return;

            UpdateRaycasters(RemoteInputRaycaster.AllRaycasters.Count);

            DrawDefaultInspector();
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            GUILayout.Label($"Providers: {_targetObject.ProviderCount}");
            GUILayout.Label($"Raycasters: {RemoteInputRaycaster.AllRaycasters.Count}");
            GUILayout.Label(_raycasterNames);
            EditorGUI.EndDisabledGroup();
        }

        void UpdateRaycasters(int newCount)
        {
            if (_raycastersCount == newCount) return;
            _raycastersCount = newCount;
            _raycasterNames = "Active raycasters: ";
            foreach (var raycaster in RemoteInput.RemoteInputRaycaster.AllRaycasters)
            {
                _raycasterNames += "\n  " + raycaster.name;
            }
        }
    }
}

