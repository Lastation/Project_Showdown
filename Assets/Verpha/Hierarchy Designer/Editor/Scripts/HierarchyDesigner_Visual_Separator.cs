#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Verpha.HierarchyDesigner
{
    [InitializeOnLoad]
    public class HierarchyDesigner_Visual_Separator
    {
        #region Default Values
        private static readonly Color defaultBackgroundColor = Color.gray;
        private static readonly Color defaultTextColor = Color.white;
        private static readonly FontStyle defaultFontStyle = FontStyle.Normal;
        private static readonly int defaultFontSize = 12;
        private static readonly TextAnchor defaultTextAlignment = TextAnchor.MiddleCenter;
        private static readonly HierarchyDesigner_Info_Separator.BackgroundImageType defaultImageType = HierarchyDesigner_Info_Separator.BackgroundImageType.Classic;
        #endregion
        #region Text Style Struct
        struct TextStyleKey
        {
            public Color TextColor;
            public FontStyle FontStyle;
            public int FontSize;
            public TextAnchor TextAlignment;

            public TextStyleKey(Color textColor, FontStyle fontStyle, int fontSize, TextAnchor textAlignment)
            {
                TextColor = textColor;
                FontStyle = fontStyle;
                FontSize = fontSize;
                TextAlignment = textAlignment;
            }

            public override bool Equals(object obj)
            {
                return obj is TextStyleKey key &&
                       TextColor.Equals(key.TextColor) &&
                       FontStyle == key.FontStyle &&
                       FontSize == key.FontSize &&
                       TextAlignment == key.TextAlignment;
            }

            public override int GetHashCode()
            {
                return System.HashCode.Combine(TextColor, FontStyle, FontSize, TextAlignment);
            }
        }
        #endregion      
        private static readonly HashSet<int> separatorInstanceIDs = new HashSet<int>();
        private static readonly HierarchyDesigner_Info_Separator defaultSeparatorInfo = new HierarchyDesigner_Info_Separator("Separator", defaultTextColor, defaultBackgroundColor, defaultFontStyle, defaultFontSize, defaultTextAlignment, defaultImageType);
        private static readonly Dictionary<TextStyleKey, GUIStyle> textStyleCache = new Dictionary<TextStyleKey, GUIStyle>();

        static HierarchyDesigner_Visual_Separator()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            HierarchySeparatorWindow.LoadSeparators();
            EditorApplication.delayCall += UpdateSeparatorVisuals;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += (scene, mode) => { UpdateSeparatorVisuals(); };
        }

        public static void UpdateSeparatorVisuals()
        {
            CacheAllSeparators();
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void CacheAllSeparators()
        {
            separatorInstanceIDs.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                foreach (GameObject rootObj in scene.GetRootGameObjects())
                {
                    CacheSeparatorsInHierarchy(rootObj.transform);
                }
            }
        }

        private static void CacheSeparatorsInHierarchy(Transform root)
        {
            if (IsSeparator(root.gameObject))
            {
                separatorInstanceIDs.Add(root.gameObject.GetInstanceID());
            }
            foreach (Transform child in root)
            {
                CacheSeparatorsInHierarchy(child);
            }
        }

        private static bool IsSeparator(GameObject gameObject)
        {
            return gameObject.tag == "EditorOnly" && gameObject.name.StartsWith("//");
        }

        private static void HierarchyItemCB(int instanceID, Rect selectionRect)
        {
            if (HierarchyDesigner_Manager_Settings.DisableHierarchyDesignerDuringPlayMode && EditorApplication.isPlaying) return;
            if (!separatorInstanceIDs.Contains(instanceID)) return;

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != null && IsSeparator(gameObject))
            {
                DrawSeparator(gameObject, selectionRect);
            }
        }

        private static void DrawSeparator(GameObject gameObject, Rect selectionRect)
        {
            GUI.color = HierarchyDesigner_Shared_ColorUtility.GetEditorBackgroundColor();
            GUI.DrawTexture(new Rect(32, selectionRect.y, EditorGUIUtility.currentViewWidth, selectionRect.height), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;

            selectionRect.x = 32;
            selectionRect.width = EditorGUIUtility.currentViewWidth;

            string separatorKey = gameObject.name.Replace("//", "").Trim();
            HierarchyDesigner_Info_Separator separatorInfo;

            if (!HierarchySeparatorWindow.separators.TryGetValue(separatorKey, out separatorInfo))
            {
                separatorInfo = defaultSeparatorInfo;
            }

            GUIStyle textStyle = GetCachedTextStyle(separatorInfo.TextColor, separatorInfo.FontStyle, separatorInfo.FontSize, separatorInfo.TextAlignment);
            Texture2D backgroundTexture = HierarchyDesigner_Manager_Background.GetBackgroundImage(separatorInfo.ImageType);

            GUI.color = separatorInfo.BackgroundColor;
            GUI.DrawTexture(selectionRect, backgroundTexture);
            GUI.color = Color.white;

            Rect textRect = AdjustRect(selectionRect, separatorInfo.TextAlignment);
            GUI.Label(textRect, separatorInfo.Name, textStyle);
        }

        private static GUIStyle GetCachedTextStyle(Color textColor, FontStyle fontStyle, int fontSize, TextAnchor textAlignment)
        {
            TextStyleKey key = new TextStyleKey(textColor, fontStyle, fontSize, textAlignment);
            if (!textStyleCache.TryGetValue(key, out GUIStyle textStyle))
            {
                textStyle = new GUIStyle
                {
                    alignment = textAlignment,
                    fontSize = fontSize,
                    fontStyle = fontStyle,
                    normal = { textColor = textColor }
                };
                textStyleCache[key] = textStyle;
            }
            return textStyle;
        }

        private static Rect AdjustRect(Rect rect, TextAnchor textAlignment)
        {
            switch (textAlignment)
            {
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                case TextAnchor.LowerLeft:
                    rect.x += 3;
                    break;
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                case TextAnchor.LowerRight:
                    rect.x -= 36;
                    break;
            }
            return rect;
        }
    }
}
#endif