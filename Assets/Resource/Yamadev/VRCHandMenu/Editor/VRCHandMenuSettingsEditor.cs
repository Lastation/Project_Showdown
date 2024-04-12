
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;
using UnityEditor;
using UnityEditorInternal;
using Yamadev.VRCHandMenu.Script;
using UnityEngine.Rendering.PostProcessing;

namespace Yamadev.VRCHandMenu.Editor
{
    [CustomEditor(typeof(VRCHandMenuSettings))]
    public class VRCHandMenuSettingsEditor : EditorBase
    {
        SerializedProperty _mainColor;
        SerializedProperty _canvas;
        SerializedProperty _menuSize;
        SerializedProperty _menuDistance;

        SerializedProperty _ppBloom;
        SerializedProperty _ppMSVO;
        SerializedProperty _ppSAO;
        SerializedProperty _ppNight;

        SerializedProperty _lightDirectional;
        SerializedProperty _lightAvatar;

        SerializedProperty _lightDirectionalSlider;
        SerializedProperty _lightAvatarSlider;

        SerializedProperty _yamaPlayer;

        VRCHandMenuSettings _target;

        private void OnEnable()
        {
            _target = target as VRCHandMenuSettings;

            _mainColor = serializedObject.FindProperty("mainColor");
            _canvas = serializedObject.FindProperty("canvas");
            _menuSize = serializedObject.FindProperty("menuSize");
            _menuDistance = serializedObject.FindProperty("menuDistance");

            _ppBloom = serializedObject.FindProperty("PPBloom");
            _ppMSVO = serializedObject.FindProperty("PPMSVO");
            _ppSAO = serializedObject.FindProperty("PPSAO");
            _ppNight = serializedObject.FindProperty("PPNight");

            _lightDirectional = serializedObject.FindProperty("LightDirectional");
            _lightAvatar = serializedObject.FindProperty("LightAvatar");

            _lightDirectionalSlider = serializedObject.FindProperty("LightDirectionalSlider");
            _lightAvatarSlider = serializedObject.FindProperty("LightAvatarSlider");

            _yamaPlayer = serializedObject.FindProperty("yamaPlayer");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("VRC Hand Menu Settings", _uiTitle);

            EditorGUILayout.Space();

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("UI", _bold);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_mainColor);
                EditorGUILayout.PropertyField(_menuSize);
                EditorGUILayout.PropertyField(_menuDistance);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Post Processing", _bold);

                EditorGUILayout.LabelField("Default Value (0 is OFF)");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_ppBloom);
                EditorGUILayout.PropertyField(_ppMSVO);
                EditorGUILayout.PropertyField(_ppSAO);
                EditorGUILayout.PropertyField(_ppNight);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Light", _bold);

                EditorGUILayout.LabelField("Default Value (0 is OFF)");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_lightDirectional);
                EditorGUILayout.PropertyField(_lightAvatar);
                EditorGUI.indentLevel--;
            }

            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Video Player", _bold);

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_yamaPlayer);
                EditorGUI.indentLevel--;
            }

            if (serializedObject.ApplyModifiedProperties())
                ApplyModifiedProperties();
        }

        internal void ApplyModifiedProperties()
        {
            // UI
            if (_mainColor.colorValue != null)
            {
                Transform canvas = _canvas.objectReferenceValue as Transform;
                for (int i = 0; i < canvas.childCount; i++)
                {
                    Image ret = canvas.GetChild(i).GetComponent<Image>();
                    if (ret == null) continue;
                    ret.color = _mainColor.colorValue;
                }
            }
            _target.MenuHandle.SetVariable("menuSize", _menuSize.floatValue);
            _target.MenuHandle.SetVariable("distance", _menuDistance.floatValue);

            // PP
            PostProcessVolume ppBloom = _target.PPHandle.Find("BLOOM").GetComponent<PostProcessVolume>();
            ppBloom.weight = _target.PPBloom;
            PostProcessVolume ppMSVO = _target.PPHandle.Find("AO/MSVO").GetComponent<PostProcessVolume>();
            ppMSVO.weight = _target.PPMSVO;
            PostProcessVolume ppSao = _target.PPHandle.Find("AO/SAO").GetComponent<PostProcessVolume>();
            ppSao.weight = _target.PPSAO;
            PostProcessVolume ppNight = _target.PPHandle.Find("NIGHTMODE").GetComponent<PostProcessVolume>();
            ppNight.weight = _target.PPNight;

            // Light
            Light lightDirectional = _target.LightHandle.Find("DirectionalLight").GetComponent<Light>();
            lightDirectional.intensity = _target.LightDirectional;
            (_lightDirectionalSlider.objectReferenceValue as Slider).value = _target.LightDirectional;

            Light lightAvatar = _target.LightHandle.Find("AvatarLight").GetComponent<Light>();
            lightAvatar.intensity = _target.LightAvatar;
            (_lightAvatarSlider.objectReferenceValue as Slider).value = _target.LightAvatar;
        }
    }
}