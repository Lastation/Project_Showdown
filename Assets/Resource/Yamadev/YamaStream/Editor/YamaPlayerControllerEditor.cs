
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Components;

namespace Yamadev.YamaStream.Script
{
    [CustomEditor(typeof(YamaPlayerController))]
    public class YamaPlayerControllerEditor : EditorBase
    {
        UIController _uiController;
        SerializedObject _uiControllerSerializedObject;
        SerializedProperty _primaryColor;
        SerializedProperty _secondaryColor;
        SerializedProperty _yamaPlayer;
        YamaPlayerController _target;
        YamaPlayer[] _players;
        bool _uiOn;
        bool _pickup;

        private void OnEnable()
        {
            _target = target as YamaPlayerController;
            _uiController = _target.GetComponentInChildren<UIController>(true);
            if (_uiController != null)
            {
                _uiControllerSerializedObject = new SerializedObject(_uiController);
                _primaryColor = _uiControllerSerializedObject.FindProperty("_primaryColor");
                _secondaryColor = _uiControllerSerializedObject.FindProperty("_secondaryColor");
            }
            _yamaPlayer = serializedObject.FindProperty("YamaPlayer");
            _players = Utils.FindComponentsInHierarthy<YamaPlayer>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField(_target.name, _uiTitle);
            EditorGUILayout.Space();

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                if (_players.Length == 1) _yamaPlayer.objectReferenceValue = _players[0];
                EditorGUILayout.PropertyField(_yamaPlayer);
                VRCPickup vrcPickup = _target.GetComponentInChildren<VRCPickup>();
                if (vrcPickup != null)
                {
                    _pickup = vrcPickup.pickupable;
                    _pickup = EditorGUILayout.Toggle("Pickupable", _pickup);
                    vrcPickup.pickupable = _pickup;
                }
            }
            EditorGUILayout.Space();

            if (_uiController != null)
            {
                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("UI", _bold);
                    _uiOn = _uiController.gameObject.activeSelf;
                    _uiOn = EditorGUILayout.Toggle("UI ON", _uiOn);
                    _uiController.gameObject.SetActive(_uiOn);
                    if (_uiOn)
                    {
                        EditorGUILayout.PropertyField(_primaryColor);
                        EditorGUILayout.PropertyField(_secondaryColor);
                    }
                }
            }

            if (serializedObject.ApplyModifiedProperties() 
                || _uiControllerSerializedObject.ApplyModifiedProperties())
                ApplyModifiedProperties();
        }

        internal void ApplyModifiedProperties()
        {
            /*
            if (_yamaPlayer.objectReferenceValue == null)
            {
                _players = Utils.FindComponentsInHierarthy<YamaPlayer>();
                if (_players.Length > 0) _yamaPlayer.objectReferenceValue = _players[0];
            }
            */
            YamaPlayerHandle handle = (_yamaPlayer.objectReferenceValue as YamaPlayer)?.GetComponentInChildren<YamaPlayerHandle>();
            if (handle == null) return;
            UIController uiController = _target.GetComponentInChildren<UIController>();
            if (uiController != null)
            {
                SerializedObject serializedObject = new SerializedObject(uiController);
                serializedObject.FindProperty("_playerHandle").objectReferenceValue = handle;
                serializedObject.ApplyModifiedProperties();
            }
            YamaPlayerScreen screen = _target.gameObject.GetComponentInChildren<YamaPlayerScreen>();
            if (screen != null)
            {
                SerializedObject serializedObject = new SerializedObject(screen);
                serializedObject.FindProperty("_playerHandle").objectReferenceValue = handle;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}