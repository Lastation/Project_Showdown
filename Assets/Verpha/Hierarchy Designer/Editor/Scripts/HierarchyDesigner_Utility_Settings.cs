#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    public class HierarchyDesigner_Utility_Settings : EditorWindow
    {
        #region OnGUI Properties
        private Vector2 scrollPositionOuter;
        private Vector2 scrollPositionInner;
        private GUIStyle customSettingsStyle;
        private GUIStyle customSettingsStyleSecondary;
        private GUIStyle headerLabelStyle;
        private GUIStyle contentLabelStyle;
        #endregion
        #region Temporary Holder Properties
        private bool showMainIconOfGameObjectTemp;
        private bool showComponentIconsTemp;
        private bool showTransformComponentTemp;
        private bool showComponentIconsForFoldersTemp;
        private bool showHierarchyTreeTemp;
        private bool showTagAndLayerTemp;
        private bool disableHierarchyDesignerDuringPlayModeTemp;
        #endregion
        #region Styling Properties
        private Color newTagLayerTextColor = Color.gray;
        private FontStyle newTagLayerFontStyle = FontStyle.BoldAndItalic;
        private int newTagLayerFontSize = 9;
        private readonly int[] fontSizeOptions = new int[15];
        private Color newTreeColor = Color.white;
        #endregion
        #region Shortcuts Properties
        private KeyCode enableDisableShortcutTemp;
        private KeyCode lockUnlockShortcutTemp;
        private KeyCode changeTagAndLayerShortcutTemp;
        private KeyCode renameGameObjectsShortcutTemp;
        #endregion

        [MenuItem("Hierarchy Designer/Hierarchy General Settings", false, 100)]
        public static void ShowWindow()
        {
            HierarchyDesigner_Utility_Settings window = GetWindow<HierarchyDesigner_Utility_Settings>("Hierarchy Designer General Settings");
            window.minSize = new Vector2(400, 200);
        }

        private void OnEnable()
        {
            LoadSettings();
            InitFontSizeOptions();
        }

        private void LoadSettings()
        {
            showMainIconOfGameObjectTemp = HierarchyDesigner_Manager_Settings.ShowMainIconOfGameObject;
            showComponentIconsTemp = HierarchyDesigner_Manager_Settings.ShowComponentIcons;
            showTransformComponentTemp = HierarchyDesigner_Manager_Settings.ShowTransformComponent;
            showComponentIconsForFoldersTemp = HierarchyDesigner_Manager_Settings.ShowComponentIconsForFolders;
            showHierarchyTreeTemp = HierarchyDesigner_Manager_Settings.ShowHierarchyTree;
            showTagAndLayerTemp = HierarchyDesigner_Manager_Settings.ShowTagAndLayer;
            disableHierarchyDesignerDuringPlayModeTemp = HierarchyDesigner_Manager_Settings.DisableHierarchyDesignerDuringPlayMode;
            newTagLayerTextColor = HierarchyDesigner_Manager_Settings.TagLayerTextColor;
            newTagLayerFontStyle = HierarchyDesigner_Manager_Settings.TagLayerFontStyle;
            newTagLayerFontSize = HierarchyDesigner_Manager_Settings.TagLayerFontSize;
            newTreeColor = HierarchyDesigner_Manager_Settings.TreeColor;
            enableDisableShortcutTemp = HierarchyDesigner_Manager_Settings.EnableDisableShortcut;
            lockUnlockShortcutTemp = HierarchyDesigner_Manager_Settings.LockUnlockShortcut;
            changeTagAndLayerShortcutTemp = HierarchyDesigner_Manager_Settings.ChangeTagAndLayerShortcut;
            renameGameObjectsShortcutTemp = HierarchyDesigner_Manager_Settings.RenameGameObjectsShortcut;
        }

        private void InitFontSizeOptions()
        {
            for (int i = 0; i < fontSizeOptions.Length; i++)
            {
                fontSizeOptions[i] = 7 + i;
            }
        }

        private void InitializeStyles()
        {
            customSettingsStyle = HierarchyDesigner_Info_OnGUI.CreateCustomStyle();
            customSettingsStyleSecondary = HierarchyDesigner_Info_OnGUI.CreateCustomStyle(true);
            headerLabelStyle = HierarchyDesigner_Info_OnGUI.HeaderLabelStyle;
            contentLabelStyle = HierarchyDesigner_Info_OnGUI.ContentLabelStyle;
        }

        private void OnGUI()
        {
            InitializeStyles();
            EditorGUILayout.BeginVertical(customSettingsStyle);

            GUILayout.Space(4);
            EditorGUILayout.LabelField("General Settings", headerLabelStyle);
            GUILayout.Space(8);

            scrollPositionOuter = EditorGUILayout.BeginScrollView(scrollPositionOuter, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region General Settings Fields
            using (new HierarchyDesigner_Info_OnGUI.LabelWidth(100))
            {
                showMainIconOfGameObjectTemp = DrawSettingToggle("Show Main Icon", showMainIconOfGameObjectTemp);
                showComponentIconsTemp = DrawSettingToggle("Show Component Icons", showComponentIconsTemp);
                showTransformComponentTemp = DrawSettingToggle("Show Transform Component Icon", showTransformComponentTemp);
                showComponentIconsForFoldersTemp = DrawSettingToggle("Show Folder's Component Icons", showComponentIconsForFoldersTemp);
                showHierarchyTreeTemp = DrawSettingToggle("Show Hierarchy Tree", showHierarchyTreeTemp);
                showTagAndLayerTemp = DrawSettingToggle("Show Tag and Layer", showTagAndLayerTemp);
                disableHierarchyDesignerDuringPlayModeTemp = DrawSettingToggle("Disable Hierarchy Designer During Play Mode", disableHierarchyDesignerDuringPlayModeTemp);
            }
            #endregion

            #region General Settings Buttons
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All", GUILayout.Height(25)))
            {
                SetAllFeatures(true);
            }
            if (GUILayout.Button("Disable All", GUILayout.Height(25)))
            {
                SetAllFeatures(false);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            #endregion

            #region Styling Fields
            EditorGUILayout.BeginVertical(customSettingsStyleSecondary);
            EditorGUILayout.LabelField("Styling", contentLabelStyle);
            GUILayout.Space(5);

            scrollPositionInner = EditorGUILayout.BeginScrollView(scrollPositionInner, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tag and Layer", GUILayout.MaxWidth(100f));
            newTagLayerTextColor = EditorGUILayout.ColorField(newTagLayerTextColor, GUILayout.Height(20));
            newTagLayerFontStyle = (FontStyle)EditorGUILayout.EnumPopup(newTagLayerFontStyle, GUILayout.Height(20));
            string[] fontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int newFontSizeIndex = Array.IndexOf(fontSizeOptions, newTagLayerFontSize);
            newTagLayerFontSize = fontSizeOptions[EditorGUILayout.Popup(newFontSizeIndex, fontSizeOptionsStrings, GUILayout.Height(20))];
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tree Color", GUILayout.MaxWidth(100f));
            newTreeColor = EditorGUILayout.ColorField(newTreeColor, GUILayout.ExpandWidth(true), GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Shortcuts Fields
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Shortcuts", contentLabelStyle);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enable/Disable GameObject Shortcut", GUILayout.Width(225));
            enableDisableShortcutTemp = (KeyCode)EditorGUILayout.EnumPopup(enableDisableShortcutTemp);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Lock/Unlock GameObject Shortcut", GUILayout.Width(225));
            lockUnlockShortcutTemp = (KeyCode)EditorGUILayout.EnumPopup(lockUnlockShortcutTemp);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Change Tag and Layer Shortcut", GUILayout.Width(225));
            changeTagAndLayerShortcutTemp = (KeyCode)EditorGUILayout.EnumPopup(changeTagAndLayerShortcutTemp);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rename GameObjects Shortcut", GUILayout.Width(225));
            renameGameObjectsShortcutTemp = (KeyCode)EditorGUILayout.EnumPopup(renameGameObjectsShortcutTemp);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.Space(2);
            if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
            {
                SaveSettings();
            }
            GUILayout.Space(5);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private bool DrawSettingToggle(string label, bool currentValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(280));
            bool newValue = EditorGUILayout.Toggle(currentValue);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        private void SetAllFeatures(bool enable)
        {
            showMainIconOfGameObjectTemp = enable;
            showComponentIconsTemp = enable;
            showTransformComponentTemp = enable;
            showComponentIconsForFoldersTemp = enable;
            showHierarchyTreeTemp = enable;
            showTagAndLayerTemp = enable;
            disableHierarchyDesignerDuringPlayModeTemp = enable;
        }

        private void SaveSettings()
        {
            bool settingsChanged =
                HierarchyDesigner_Manager_Settings.ShowMainIconOfGameObject != showMainIconOfGameObjectTemp ||
                HierarchyDesigner_Manager_Settings.ShowComponentIcons != showComponentIconsTemp ||
                HierarchyDesigner_Manager_Settings.ShowTransformComponent != showTransformComponentTemp ||
                HierarchyDesigner_Manager_Settings.ShowComponentIconsForFolders != showComponentIconsForFoldersTemp ||
                HierarchyDesigner_Manager_Settings.ShowHierarchyTree != showHierarchyTreeTemp ||
                HierarchyDesigner_Manager_Settings.ShowTagAndLayer != showTagAndLayerTemp ||
                HierarchyDesigner_Manager_Settings.TagLayerTextColor != newTagLayerTextColor ||
                HierarchyDesigner_Manager_Settings.TagLayerFontStyle != newTagLayerFontStyle ||
                HierarchyDesigner_Manager_Settings.TagLayerFontSize != newTagLayerFontSize ||
                HierarchyDesigner_Manager_Settings.TreeColor != newTreeColor;

            HierarchyDesigner_Manager_Settings.ShowMainIconOfGameObject = showMainIconOfGameObjectTemp;
            HierarchyDesigner_Manager_Settings.ShowComponentIcons = showComponentIconsTemp;
            HierarchyDesigner_Manager_Settings.ShowTransformComponent = showTransformComponentTemp;
            HierarchyDesigner_Manager_Settings.ShowComponentIconsForFolders = showComponentIconsForFoldersTemp;
            HierarchyDesigner_Manager_Settings.ShowHierarchyTree = showHierarchyTreeTemp;
            HierarchyDesigner_Manager_Settings.ShowTagAndLayer = showTagAndLayerTemp;
            HierarchyDesigner_Manager_Settings.DisableHierarchyDesignerDuringPlayMode = disableHierarchyDesignerDuringPlayModeTemp;
            HierarchyDesigner_Manager_Settings.TagLayerTextColor = newTagLayerTextColor;
            HierarchyDesigner_Manager_Settings.TagLayerFontStyle = newTagLayerFontStyle;
            HierarchyDesigner_Manager_Settings.TagLayerFontSize = newTagLayerFontSize;
            HierarchyDesigner_Manager_Settings.EnableDisableShortcut = enableDisableShortcutTemp;
            HierarchyDesigner_Manager_Settings.LockUnlockShortcut = lockUnlockShortcutTemp;
            HierarchyDesigner_Manager_Settings.ChangeTagAndLayerShortcut = changeTagAndLayerShortcutTemp;
            HierarchyDesigner_Manager_Settings.RenameGameObjectsShortcut = renameGameObjectsShortcutTemp;

            if (settingsChanged)
            {
                if (HierarchyDesigner_Manager_Settings.TagLayerFontSize != newTagLayerFontSize)
                {
                    HierarchyDesigner_Visual_GameObject.RecalculateTagAndLayerSizes();
                }
                if(HierarchyDesigner_Manager_Settings.TreeColor != newTreeColor)
                {
                    HierarchyDesigner_Visual_GameObject.UpdateTreeColorCache();
                    HierarchyDesigner_Manager_Settings.TreeColor = newTreeColor;
                }

                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }
}
#endif