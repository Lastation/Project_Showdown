
using System;
using UnityEditor;
using UnityEngine;

namespace Yamadev.YamaStream.Script
{
    public class LTCGIEditor : EditorWindow
    {
        static string _url = "https://github.com/PiMaker/ltcgi/releases/latest";
        static string _crtPath = "Assets/Yamadev/YamaStream/Assets/Textures/YamaPlayerCRT.asset";
        static string _crtGuid = "a9024879323f03444be1a5332baee58e";
        static string _ltcgiControllerPath = "Packages/at.pimaker.ltcgi/LTCGI Controller.prefab";
        static string _ltcgiControllerGuid = "4b1aac09caa0ea54ba902102643bb545";
        static bool _ltcgiImported = false;
        static YamaPlayer _player;

        [MenuItem("YamaPlayer/LTCGI")]
        public static void ShowLTCGIEditorWindow()
        {
            LTCGIEditor window = GetWindow<LTCGIEditor>(title: "LTCGI Editor");
            window.Show();
        }

        static void clearCurrentSettings()
        {
            Type ltcgiScreen = Utils.FindType("pi.LTCGI.LTCGI_Screen", true);
            UnityEngine.Object[] screens = Utils.FindComponentsInHierarthy(ltcgiScreen);
            foreach (UnityEngine.Object screen in screens) DestroyImmediate(screen);
            YamaPlayer[] players = Utils.FindComponentsInHierarthy<YamaPlayer>();
            foreach (YamaPlayer player in players)
            {
                Controller yamaplayerController = _player.GetComponentInChildren<Controller>();
                if (yamaplayerController != null) yamaplayerController.SetProgramVariable("_lod", null);
            }
        }

        static void autoApply()
        {
            clearCurrentSettings();
            CustomRenderTexture crt = AssetDatabase.LoadAssetAtPath<CustomRenderTexture>(_crtPath);
            if (crt == null)
            {
                string crtPath = AssetDatabase.GUIDToAssetPath(_crtGuid);
                crt = AssetDatabase.LoadAssetAtPath<CustomRenderTexture>(crtPath);
            }
            if (crt == null) return;
            Controller yamaplayerController = _player.GetComponentInChildren<Controller>();
            if (yamaplayerController != null)
            {
                SerializedObject serializedObject = new SerializedObject(yamaplayerController);
                serializedObject.FindProperty("_lod").objectReferenceValue = crt.material;
                serializedObject.ApplyModifiedProperties();
            }
            Renderer mainScreen = _player.MainScreen;
            if (mainScreen == null) mainScreen = _player.transform.Find("Screen")?.GetComponent<Renderer>();
            if (mainScreen != null)
            {
                Type ltcgiScreen = Utils.FindType("pi.LTCGI.LTCGI_Screen", true);
                Component screen = mainScreen.gameObject.GetComponent(ltcgiScreen);
                if (screen == null) screen = mainScreen.gameObject.AddComponent(ltcgiScreen);
                ((dynamic)screen).ColorMode = Enum.Parse(((dynamic)screen).ColorMode.GetType(), "Texture");
            }
            Type ltcgiController = Utils.FindType("pi.LTCGI.LTCGI_Controller", true);
            UnityEngine.Object[] controllers = Utils.FindComponentsInHierarthy(ltcgiController);
            UnityEngine.Object controller = controllers.Length > 0 ? controllers[0] : null;
            if (controller == null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_ltcgiControllerPath);
                if (prefab == null)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(_ltcgiControllerGuid);
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                }
                if (prefab != null)
                {
                    GameObject obj = Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    obj.transform.SetParent(null, true);
                    controller = obj.GetComponent(ltcgiController);
                }
            }
            if (controller != null) ((dynamic)controller).VideoTexture = crt;
        }

        void OnEnable()
        {
            if (Utils.FindType("pi.LTCGI.LTCGI_Controller", true) != null) _ltcgiImported = true;
        }

        void OnGUI()
        {
            if (!_ltcgiImported)
            {
                GUILayout.Label("LTCGI not imported.", EditorStyles.boldLabel);
                GUILayout.Label("LTCGIを先にインポートしてください。");
                if (GUILayout.Button("Open Github Page")) Application.OpenURL(_url);
                return;
            }
            GUILayout.Label("Target YamaPlayer");
            // if (Utils.FindComponentsInHierarthy<YamaPlayer>().Length != 1)
            _player = (YamaPlayer)EditorGUILayout.ObjectField(_player, typeof(YamaPlayer), true);
            if (_player == null) return;
            if (GUILayout.Button("LTCGI自動設定") && EditorUtility.DisplayDialog("LTCGI自動設定", "現在のLTCGI設定が上書きされます?", "Yes", "No"))
                autoApply();
        }
    }
}