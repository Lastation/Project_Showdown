#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    public static class HierarchyDesigner_Manager_Settings
    {
        #region Keys
        private const string ShowMainIconOfGameObjectKey = "HierarchyDesigner_ShowMainIconOfGameObject";
        private const string ShowComponentIconsKey = "HierarchyDesigner_ShowComponentIcons";
        private const string ShowTransformComponentKey = "HierarchyDesigner_ShowTransformComponent";
        private const string ShowComponentIconsForFoldersKey = "HierarchyDesigner_ShowComponentIconsForFolders";
        private const string ShowHierarchyTreeKey = "HierarchyDesigner_ShowHierarchyTree";
        private const string ShowTagAndLayerKey = "HierarchyDesigner_ShowTagAndLayer";
        private const string DisableHierarchyDesignerDuringPlayModeKey = "HierarchyDesigner_DisableDuringPlayMode";
        private const string TagLayerTextColorKey = "HierarchyDesigner_TagLayerTextColor";
        private const string TagLayerFontStyleKey = "HierarchyDesigner_TagLayerFontStyle";
        private const string TagLayerFontSizeKey = "HierarchyDesigner_TagLayerFontSize";
        private const string TreeColorKey = "HierarchyDesigner_TreeColor";
        private const string EnableDisableShortcutKey = "HierarchyDesigner_EnableDisableShortcut";
        private const string LockUnlockShortcutKey = "HierarchyDesigner_LockUnlockShortcut";
        private const string ChangeTagAndLayerShortcutKey = "HierarchyDesigner_ChangeTagAndLayerShortcut";
        private const string RenameGameObjectsShortcutKey = "HierarchyDesigner_RenameGameObjectsShortcut";
        #endregion

        private static bool showMainIconOfGameObject = EditorPrefs.GetBool(ShowMainIconOfGameObjectKey, true);
        private static bool showComponentIcons = EditorPrefs.GetBool(ShowComponentIconsKey, true);
        private static bool showTransformComponent = EditorPrefs.GetBool(ShowTransformComponentKey, true);
        private static bool showComponentIconsForFolders = EditorPrefs.GetBool(ShowComponentIconsForFoldersKey, true);
        private static bool showHierarchyTree = EditorPrefs.GetBool(ShowHierarchyTreeKey, true);
        private static bool showTagAndLayer = EditorPrefs.GetBool(ShowTagAndLayerKey, true);
        private static bool disableHierarchyDesignerDuringPlayMode = EditorPrefs.GetBool(DisableHierarchyDesignerDuringPlayModeKey, true);
        private static Color tagLayerTextColor = HierarchyDesigner_Shared_ColorUtility.ParseColor(EditorPrefs.GetString(TagLayerTextColorKey, HierarchyDesigner_Shared_ColorUtility.ColorToString(Color.gray)));
        private static FontStyle tagLayerFontStyle = (FontStyle)EditorPrefs.GetInt(TagLayerFontStyleKey, (int)FontStyle.Normal);
        private static int tagLayerFontSize = EditorPrefs.GetInt(TagLayerFontSizeKey, 12);
        private static Color treeColor = HierarchyDesigner_Shared_ColorUtility.ParseColor(EditorPrefs.GetString(TreeColorKey, HierarchyDesigner_Shared_ColorUtility.ColorToString(Color.white)));
        private static KeyCode enableDisableShortcut = (KeyCode)EditorPrefs.GetInt(EnableDisableShortcutKey, (int)KeyCode.Mouse2);
        private static KeyCode lockUnlockShortcutShortcut = (KeyCode)EditorPrefs.GetInt(LockUnlockShortcutKey, (int)KeyCode.F1);
        private static KeyCode changeTagAndLayerShortcut = (KeyCode)EditorPrefs.GetInt(ChangeTagAndLayerShortcutKey, (int)KeyCode.F2);
        private static KeyCode renameGameObjectsShortcut = (KeyCode)EditorPrefs.GetInt(RenameGameObjectsShortcutKey, (int)KeyCode.F3);

        public static bool ShowMainIconOfGameObject
        {
            get => showMainIconOfGameObject;
            set => SetAndSave(ref showMainIconOfGameObject, value, ShowMainIconOfGameObjectKey);
        }

        public static bool ShowComponentIcons
        {
            get => showComponentIcons;
            set => SetAndSave(ref showComponentIcons, value, ShowComponentIconsKey);
        }

        public static bool ShowTransformComponent
        {
            get => showTransformComponent;
            set => SetAndSave(ref showTransformComponent, value, ShowTransformComponentKey);
        }

        public static bool ShowComponentIconsForFolders
        {
            get => showComponentIconsForFolders;
            set => SetAndSave(ref showComponentIconsForFolders, value, ShowComponentIconsForFoldersKey);
        }

        public static bool ShowHierarchyTree
        {
            get => showHierarchyTree;
            set => SetAndSave(ref showHierarchyTree, value, ShowHierarchyTreeKey);
        }

        public static bool ShowTagAndLayer
        {
            get => showTagAndLayer;
            set => SetAndSave(ref showTagAndLayer, value, ShowTagAndLayerKey);
        }

        public static bool DisableHierarchyDesignerDuringPlayMode
        {
            get => disableHierarchyDesignerDuringPlayMode;
            set => SetAndSave(ref disableHierarchyDesignerDuringPlayMode, value, DisableHierarchyDesignerDuringPlayModeKey);
        }

        public static Color TagLayerTextColor
        {
            get => tagLayerTextColor;
            set
            {
                if (tagLayerTextColor != value)
                {
                    tagLayerTextColor = value;
                    EditorPrefs.SetString(TagLayerTextColorKey, HierarchyDesigner_Shared_ColorUtility.ColorToString(value));
                }
            }
        }

        public static FontStyle TagLayerFontStyle
        {
            get => tagLayerFontStyle;
            set
            {
                if (tagLayerFontStyle != value)
                {
                    tagLayerFontStyle = value;
                    EditorPrefs.SetInt(TagLayerFontStyleKey, (int)value);
                }
            }
        }

        public static int TagLayerFontSize
        {
            get => tagLayerFontSize;
            set
            {
                if (tagLayerFontSize != value)
                {
                    tagLayerFontSize = value;
                    EditorPrefs.SetInt(TagLayerFontSizeKey, value);
                }
            }
        }

        public static Color TreeColor
        {
            get => treeColor;
            set
            {
                if (treeColor != value)
                {
                    treeColor = value;
                    EditorPrefs.SetString(TreeColorKey, HierarchyDesigner_Shared_ColorUtility.ColorToString(value));
                }
            }
        }

        public static KeyCode EnableDisableShortcut
        {
            get => enableDisableShortcut;
            set => SetAndSave(ref enableDisableShortcut, value, EnableDisableShortcutKey);
        }

        public static KeyCode LockUnlockShortcut
        {
            get => lockUnlockShortcutShortcut;
            set => SetAndSave(ref lockUnlockShortcutShortcut, value, LockUnlockShortcutKey);
        }

        public static KeyCode ChangeTagAndLayerShortcut
        {
            get => changeTagAndLayerShortcut;
            set => SetAndSave(ref changeTagAndLayerShortcut, value, ChangeTagAndLayerShortcutKey);
        }

        public static KeyCode RenameGameObjectsShortcut
        {
            get => renameGameObjectsShortcut;
            set => SetAndSave(ref renameGameObjectsShortcut, value, RenameGameObjectsShortcutKey);
        }

        private static void SetAndSave(ref bool field, bool value, string key)
        {
            if (field != value)
            {
                field = value;
                EditorPrefs.SetBool(key, value);
            }
        }

        private static void SetAndSave<T>(ref T field, T value, string key)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                EditorPrefs.SetInt(key, Convert.ToInt32(value));
            }
        }
    }
}
#endif