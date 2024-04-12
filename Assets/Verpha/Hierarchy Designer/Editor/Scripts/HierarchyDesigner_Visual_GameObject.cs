#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Verpha.HierarchyDesigner
{
    [InitializeOnLoad]
    public static class HierarchyDesigner_Visual_GameObject
    {
        #region Hierarchy Tree Icons
        private static readonly Texture2D branchIcon_I = HierarchyDesigner_Manager_Tree.BranchIcon_I;
        private static readonly Texture2D branchIcon_L = HierarchyDesigner_Manager_Tree.BranchIcon_L;
        private static readonly Texture2D branchIcon_T = HierarchyDesigner_Manager_Tree.BranchIcon_T;
        private static readonly Texture2D branchIcon_End = HierarchyDesigner_Manager_Tree.BranchIcon_End;
        #endregion
        #region TMP Availability Check
        private static bool? _isTMPAvailable;
        private static Type _tmpTextType;
        private static bool IsTMPAvailable()
        {
            if (!_isTMPAvailable.HasValue)
            {
                _isTMPAvailable = AssetDatabase.FindAssets("t:TMP_Settings").Length > 0;
            }
            return _isTMPAvailable.Value;
        }
        private static Type TMPTextType
        {
            get
            {
                if (_tmpTextType == null && IsTMPAvailable())
                {
                    _tmpTextType = Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro");
                }
                return _tmpTextType;
            }
        }
        #endregion
        #region UI Component Types Cached
        private static readonly Type ButtonType = typeof(Button);
        private static readonly Type ScrollbarType = typeof(Scrollbar);
        private static readonly Type TextType = typeof(Text);
        private static readonly Type RawImageType = typeof(RawImage);
        private static readonly Type ImageType = typeof(Image);
        #endregion

        private static readonly Texture2D backgroundImage = HierarchyDesigner_Manager_Background.BackgroundImage;
        private static Rect tagLabelRect, layerLabelRect;
        private static GUIStyle _tagLayerLabelStyle;
        private static GUIStyle tagLayerLabelStyle
        {
            get
            {
                if (_tagLayerLabelStyle == null)
                {
                    _tagLayerLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fontStyle = HierarchyDesigner_Manager_Settings.TagLayerFontStyle,
                        fontSize = HierarchyDesigner_Manager_Settings.TagLayerFontSize,
                        normal = { textColor = HierarchyDesigner_Manager_Settings.TagLayerTextColor }
                    };
                }
                else
                {
                    _tagLayerLabelStyle.fontStyle = HierarchyDesigner_Manager_Settings.TagLayerFontStyle;
                    _tagLayerLabelStyle.fontSize = HierarchyDesigner_Manager_Settings.TagLayerFontSize;
                    _tagLayerLabelStyle.normal.textColor = HierarchyDesigner_Manager_Settings.TagLayerTextColor;
                }
                return _tagLayerLabelStyle;
            }
        }
        private static Color treeColorCache;
        private static bool isTreeColorCacheInitialized = false;
        private static Color TreeColor
        {
            get
            {
                if (!isTreeColorCacheInitialized)
                {
                    treeColorCache = HierarchyDesigner_Manager_Settings.TreeColor;
                    isTreeColorCacheInitialized = true;
                }
                return treeColorCache;
            }
        }
        private static Dictionary<int, Texture2D> mainIconCache = new Dictionary<int, Texture2D>();
        private static Dictionary<WeakReference, Component[]> componentCache = new Dictionary<WeakReference, Component[]>();
        private static List<Component[]> componentArrayPool = new List<Component[]>();
        private static readonly Dictionary<int, (Texture2D icon, Color color)> branchIconCache = new Dictionary<int, (Texture2D icon, Color color)>();
        private static readonly Dictionary<(string, GUIStyle, int), Vector2> guiContentSizeCache = new Dictionary<(string, GUIStyle, int), Vector2>();
        private static HashSet<int> visibleGameObjects = new HashSet<int>();

        static HierarchyDesigner_Visual_GameObject()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            if (HierarchyDesigner_Manager_Settings.DisableHierarchyDesignerDuringPlayMode && EditorApplication.isPlaying) return;

            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) return;
            visibleGameObjects.Add(instanceID);

            if (HierarchyDesigner_Utility_Separator.IsSeparator(gameObject)) return;
            DrawMainGameObjectIcon(gameObject, selectionRect, instanceID);
            DrawHierarchyTree(gameObject, selectionRect);

            if (HierarchyDesigner_Utility_Tools.IsGameObjectLocked(gameObject)) return;
            DrawComponentIcons(gameObject, selectionRect);
            DrawTagAndLayerInfo(gameObject, selectionRect);

            HandleTagAndLayerShortcut(gameObject, selectionRect);
            HandleRenameShortcut();
        }

        private static void OnHierarchyChanged()
        {
            if (HierarchyDesigner_Manager_Settings.DisableHierarchyDesignerDuringPlayMode && EditorApplication.isPlaying) return;

            ValidateComponentCache();
            List<WeakReference> deadComponentReferences = new List<WeakReference>();
            foreach (WeakReference key in componentCache.Keys)
            {
                if (!key.IsAlive)
                {
                    deadComponentReferences.Add(key);
                }
            }
            foreach (WeakReference deadRef in deadComponentReferences)
            {
                if (componentCache.TryGetValue(deadRef, out Component[] components))
                {
                    ReturnComponentsToPool(components);
                }
                componentCache.Remove(deadRef);
            }

            List<int> keysToRemoveFromMainIconCache = new List<int>();
            foreach (var kvp in mainIconCache)
            {
                if (EditorUtility.InstanceIDToObject(kvp.Key) == null)
                {
                    keysToRemoveFromMainIconCache.Add(kvp.Key);
                }
            }
            foreach (int key in keysToRemoveFromMainIconCache)
            {
                mainIconCache.Remove(key);
            }

            branchIconCache.Clear();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ValidateComponentCache();
            visibleGameObjects.Clear();
            guiContentSizeCache.Clear();
            CleanupCache();
            CleanupComponentArrayPool();
        }

        private static void ValidateComponentCache()
        {
            List<WeakReference> deadReferences = new List<WeakReference>();
            foreach (WeakReference key in componentCache.Keys)
            {
                if (!key.IsAlive)
                {
                    deadReferences.Add(key);
                }
            }
            foreach (WeakReference deadRef in deadReferences)
            {
                componentCache.Remove(deadRef);
            }
        }

        private static void CleanupCache()
        {
            List<WeakReference> deadComponentReferences = new List<WeakReference>();
            foreach (WeakReference key in componentCache.Keys)
            {
                if (!key.IsAlive)
                {
                    deadComponentReferences.Add(key);
                }
            }
            foreach (WeakReference deadRef in deadComponentReferences)
            {
                componentCache.Remove(deadRef);
            }

            List<int> keysToRemove = new List<int>();
            foreach (int key in branchIconCache.Keys)
            {
                if (EditorUtility.InstanceIDToObject(key) == null)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (int key in keysToRemove)
            {
                branchIconCache.Remove(key);
            }
        }

        private static void DrawMainGameObjectIcon(GameObject gameObject, Rect selectionRect, int instanceID)
        {
            if (!HierarchyDesigner_Manager_Settings.ShowMainIconOfGameObject || ShouldSkipDrawing(gameObject) || HierarchyDesigner_Visual_Folder.IsFolder(gameObject)) return;

            bool isHovering = Event.current.type == EventType.Repaint && selectionRect.Contains(Event.current.mousePosition);
            GUI.color = HierarchyDesigner_Shared_ColorUtility.GetBackgroundColor(isHovering, instanceID);
            if (backgroundImage != null)
            {
                DrawBackgroundImage(selectionRect);
            }

            GUI.color = Color.white;
            Texture2D mainIcon = GetMainIconForGameObject(gameObject);
            if (mainIcon != null)
            {
                DrawMainIcon(mainIcon, selectionRect);
            }
        }

        private static void DrawBackgroundImage(Rect selectionRect)
        {
            float iconSize = selectionRect.height * 1.0f;
            float iconYPosition = selectionRect.y + (selectionRect.height - iconSize) / 2;
            Rect backgroundRect = new Rect(selectionRect.x, iconYPosition, iconSize, iconSize);
            GUI.DrawTexture(backgroundRect, backgroundImage);
        }

        private static Texture2D GetMainIconForGameObject(GameObject gameObject)
        {
            int instanceID = gameObject.GetInstanceID();
            if (!mainIconCache.TryGetValue(instanceID, out Texture2D mainIcon))
            {
                Component[] components = GetComponentsFromCacheOrObject(gameObject);
                UpdateMainIconCacheIfNeeded(gameObject, instanceID, components);
                mainIconCache.TryGetValue(instanceID, out mainIcon);
            }
            return mainIcon;
        }

        private static void DrawMainIcon(Texture2D icon, Rect selectionRect)
        {
            float iconSize = selectionRect.height * 1.0f;
            float iconYPosition = selectionRect.y + (selectionRect.height - iconSize) / 2;
            Rect iconRect = new Rect(selectionRect.x, iconYPosition, iconSize, iconSize);
            GUI.DrawTexture(iconRect, icon);
        }

        private static Component FindMainUIComponent(Component[] components)
        {
            if (components == null || components.Length == 0) return null;

            Component imageComponent = null;
            foreach (Component component in components)
            {
                if (component == null) continue;

                Type componentType = component.GetType();
                if (TMPTextType != null && componentType == TMPTextType)
                {
                    return component;
                }

                if (componentType == ButtonType || componentType == ScrollbarType || componentType == TextType || componentType == RawImageType)
                {
                    return component;
                }

                if (componentType == ImageType && imageComponent == null)
                {
                    imageComponent = component;
                }
            }
            return imageComponent;
        }

        private static void DrawComponentIcons(GameObject gameObject, Rect selectionRect)
        {
            if (!visibleGameObjects.Contains(gameObject.GetInstanceID()) || !HierarchyDesigner_Manager_Settings.ShowComponentIcons || ShouldSkipDrawing(gameObject)) { return; }

            Component[] components = GetComponentsFromCacheOrObject(gameObject);
            if (components.Length == 0) return;

            List<Component> filteredComponents = FilterComponents(components, HierarchyDesigner_Manager_Settings.ShowTransformComponent);

            float iconSize = selectionRect.height * 0.8f;
            float iconOffset = GetInitialIconOffset(filteredComponents.ToArray(), selectionRect);

            foreach (Component component in filteredComponents)
            {
                if (component == null)
                {
                    Texture2D warningIcon = EditorGUIUtility.FindTexture("console.warnicon");
                    DrawIcon(selectionRect, iconSize, ref iconOffset, warningIcon);
                    continue;
                }

                Texture2D componentIcon = GetComponentIcon(component);
                if (componentIcon != null)
                {
                    DrawIcon(selectionRect, iconSize, ref iconOffset, componentIcon);
                }
            }
        }

        private static List<Component> FilterComponents(Component[] components, bool showTransformComponent)
        {
            List<Component> filteredComponents = new List<Component>();
            foreach (Component component in components)
            {
                if (component is Transform && !showTransformComponent) continue;
                filteredComponents.Add(component);
            }
            return filteredComponents;
        }

        private static Texture2D GetComponentIcon(Component component)
        {
            if (component == null)
            {
                return null;
            }

            if (IsScriptMissing(component))
            {
                return EditorGUIUtility.FindTexture("console.warnicon");
            }

            return EditorGUIUtility.ObjectContent(component, component.GetType()).image as Texture2D;
        }

        private static void DrawIcon(Rect selectionRect, float iconSize, ref float iconOffset, Texture2D componentIcon)
        {
            float iconYPosition = selectionRect.y + (selectionRect.height - iconSize) / 2;
            Rect iconRect = new Rect(selectionRect.x + iconOffset, iconYPosition, iconSize, iconSize);
            GUI.DrawTexture(iconRect, componentIcon);
            iconOffset += iconSize + 2;
        }

        private static bool IsScriptMissing(Component component)
        {
            if (component is MonoBehaviour monoBehaviour)
            {
                SerializedObject serializedObject = new SerializedObject(component);
                SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
                return scriptProperty == null || scriptProperty.objectReferenceValue == null;
            }
            return false;
        }

        private static Component[] GetComponentsFromCacheOrObject(GameObject gameObject)
        {
            if (gameObject == null) { return new Component[0]; }

            int instanceID = gameObject.GetInstanceID();
            WeakReference weakRef = new WeakReference(gameObject);

            if (!componentCache.TryGetValue(weakRef, out Component[] components) || !weakRef.IsAlive)
            {
                components = GetPooledComponents(gameObject);
                componentCache[weakRef] = components;
            }

            UpdateMainIconCacheIfNeeded(gameObject, instanceID, components);
            return components;
        }

        private static Component[] GetPooledComponents(GameObject gameObject)
        {
            Component[] allComponents = gameObject.GetComponents<Component>();
            int componentCount = allComponents.Length;

            Component[] componentsFromPool = null;
            for (int i = 0; i < componentArrayPool.Count; i++)
            {
                if (componentArrayPool[i].Length >= componentCount && ValidateComponentsArray(componentArrayPool[i], componentCount))
                {
                    componentsFromPool = componentArrayPool[i];
                    componentArrayPool.RemoveAt(i);
                    break;
                }
            }

            if (componentsFromPool == null)
            {
                componentsFromPool = new Component[componentCount];
            }
            else
            {
                for (int i = componentCount; i < componentsFromPool.Length; i++)
                {
                    componentsFromPool[i] = null;
                }
            }

            Array.Copy(allComponents, componentsFromPool, componentCount);
            return componentsFromPool;
        }

        private static bool ValidateComponentsArray(Component[] components, int expectedComponentCount)
        {
            if (components.Length < expectedComponentCount)
            {
                return false;
            }

            bool hasNonNullComponent = false;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    hasNonNullComponent = true;
                    break;
                }
            }

            if (!hasNonNullComponent)
            {
                return false;
            }

            if (Array.TrueForAll(components, component => component == null))
            {
                return false;
            }

            return true;
        }

        private static void ReturnComponentsToPool(Component[] components)
        {
            bool isValidForPool = false;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    isValidForPool = true;
                    break;
                }
            }

            if (isValidForPool)
            {
                componentArrayPool.Add(components);
            }
        }

        private static void CleanupComponentArrayPool()
        {
            List<Component[]> validatedPool = new List<Component[]>();
            foreach (Component[] componentsArray in componentArrayPool)
            {
                bool hasNonNullComponent = false;
                for (int i = 0; i < componentsArray.Length; i++)
                {
                    if (componentsArray[i] != null)
                    {
                        hasNonNullComponent = true;
                        break;
                    }
                }

                if (hasNonNullComponent)
                {
                    validatedPool.Add(componentsArray);
                }
            }
            componentArrayPool = validatedPool;
        }

        private static float GetInitialIconOffset(Component[] components, Rect selectionRect)
        {
            if (components == null || components.Length == 0 || components[0] == null || components[0].gameObject == null)
            {
                return 0f;
            }

            GUIStyle style = GUI.skin.label;
            GUIContent content = new GUIContent(components[0].gameObject.name);
            float textWidth = style.CalcSize(content).x;
            return textWidth + 20f;
        }

        private static void UpdateMainIconCacheIfNeeded(GameObject gameObject, int instanceID, Component[] components)
        {
            Texture2D newMainIcon = GetIconForMainComponent(gameObject, components);

            if (!mainIconCache.TryGetValue(instanceID, out Texture2D currentMainIcon) || currentMainIcon != newMainIcon)
            {
                mainIconCache[instanceID] = newMainIcon;
            }
        }

        private static Texture2D GetIconForMainComponent(GameObject gameObject, Component[] components)
        {
            Component mainComponent = FindMainUIComponent(components);

            if (mainComponent == null)
            {
                mainComponent = (components != null && components.Length > 1) ? components[1] : gameObject.transform;
            }

            if (mainComponent != null)
            {
                return EditorGUIUtility.ObjectContent(mainComponent, mainComponent.GetType()).image as Texture2D;
            }

            return null;
        }

        private static void DrawTagAndLayerInfo(GameObject gameObject, Rect selectionRect)
        {
            if (!HierarchyDesigner_Manager_Settings.ShowTagAndLayer || ShouldSkipDrawing(gameObject)) return;

            GUIStyle currentStyle = tagLayerLabelStyle;

            GUIContent tagContent = new GUIContent(gameObject.tag);
            Vector2 tagSize = GetCachedGUIContentSize(tagContent, currentStyle);
            float tagLabelWidth = tagSize.x;

            string layerName = LayerMask.LayerToName(gameObject.layer);
            GUIContent layerContent = new GUIContent(layerName);
            Vector2 layerSize = GetCachedGUIContentSize(layerContent, currentStyle);
            float layerLabelWidth = layerSize.x;

            float iconOffset = GetAdjustedIconOffset(gameObject, selectionRect.height * 0.65f, selectionRect) + 3f;
            const float spaceBetween = 3f;

            tagLabelRect = new Rect(selectionRect.x + iconOffset, selectionRect.y, tagLabelWidth, selectionRect.height);
            layerLabelRect = new Rect(tagLabelRect.xMax + spaceBetween, selectionRect.y, layerLabelWidth, selectionRect.height);

            GUI.Label(tagLabelRect, gameObject.tag, currentStyle);
            GUI.Label(layerLabelRect, layerName, currentStyle);
        }

        private static Vector2 GetCachedGUIContentSize(GUIContent content, GUIStyle style)
        {
            var key = (content.text, style, style.fontSize);
            if (!guiContentSizeCache.TryGetValue(key, out Vector2 size))
            {
                size = style.CalcSize(content);
                guiContentSizeCache[key] = size;
            }
            return size;
        }

        public static void RecalculateTagAndLayerSizes()
        {
            guiContentSizeCache.Clear();
            _tagLayerLabelStyle = null;
        }

        private static void HandleTagAndLayerShortcut(GameObject gameObject, Rect selectionRect)
        {
            if (HierarchyDesigner_Manager_Settings.ShowTagAndLayer)
            {
                if (HierarchyDesigner_Utility_Shortcut.IsShortcutPressed(HierarchyDesigner_Manager_Settings.ChangeTagAndLayerShortcut))
                {
                    Vector2 mousePosition = Event.current.mousePosition;
                    if (tagLabelRect.Contains(mousePosition))
                    {
                        HierarchyDesigner_Window_TagLayer.OpenWindow(gameObject, true, Event.current.mousePosition);
                        Event.current.Use();
                    }
                    else if (layerLabelRect.Contains(mousePosition))
                    {
                        HierarchyDesigner_Window_TagLayer.OpenWindow(gameObject, false, Event.current.mousePosition);
                        Event.current.Use();
                    }
                }
            }
        }

        private static void HandleRenameShortcut()
        {
            if (HierarchyDesigner_Utility_Shortcut.IsShortcutPressed(HierarchyDesigner_Manager_Settings.RenameGameObjectsShortcut))
            {
                GameObject[] selectedGameObjects = Selection.gameObjects;
                if (selectedGameObjects.Length > 0)
                {
                    HierarchyDesigner_Window_Renaming.OpenWindow(selectedGameObjects, Event.current.mousePosition);
                }
            }
        }

        private static float GetAdjustedIconOffset(GameObject gameObject, float iconSize, Rect selectionRect)
        {
            float offset = 0f;
            float spaceBetweenElements = 5f;

            float mainIconWidth = 8f;
            offset += mainIconWidth + spaceBetweenElements;

            GUIContent gameObjectNameContent = new GUIContent(gameObject.name);
            GUIStyle labelStyle = GUI.skin.label;
            float gameObjectNameWidth = labelStyle.CalcSize(gameObjectNameContent).x;
            offset += gameObjectNameWidth + spaceBetweenElements;

            if (HierarchyDesigner_Manager_Settings.ShowComponentIcons)
            {
                Component[] components = GetComponentsFromCacheOrObject(gameObject);

                int visibleComponentsCount = 0;
                foreach (var component in components)
                {
                    if (HierarchyDesigner_Manager_Settings.ShowTransformComponent || !(component is Transform))
                    {
                        visibleComponentsCount++;
                    }
                }

                offset += visibleComponentsCount * (iconSize + spaceBetweenElements);
            }

            return offset;
        }

        private static void DrawHierarchyTree(GameObject gameObject, Rect selectionRect)
        {
            if (!HierarchyDesigner_Manager_Settings.ShowHierarchyTree || Event.current.type != EventType.Repaint || gameObject.transform.parent == null) return;

            const float iconOffset = 14f;
            const float initialOffset = 22f;
            Transform transform = gameObject.transform;
            float startX = selectionRect.x - initialOffset;
            Texture2D branchIcon = GetOrCreateBranchIcon(transform);
            GUI.DrawTexture(new Rect(startX, selectionRect.y, selectionRect.height, selectionRect.height), branchIcon, ScaleMode.ScaleToFit, true, 0, TreeColor, 0, 0);
            DrawParentTreeLines(transform.parent, startX, iconOffset, selectionRect.height, selectionRect.y);
        }

        private static Texture2D GetOrCreateBranchIcon(Transform transform)
        {
            int instanceID = transform.GetInstanceID();
            Color currentTreeColor = GetCachedTreeColor();

            if (!branchIconCache.TryGetValue(instanceID, out var cached) || cached.color != currentTreeColor)
            {
                bool isLastChild = transform.GetSiblingIndex() == transform.parent.childCount - 1;
                Texture2D branchIcon = isLastChild ? (transform.childCount == 0 ? branchIcon_End : branchIcon_L) : branchIcon_T;
                branchIconCache[instanceID] = (branchIcon, currentTreeColor);
                return branchIcon;
            }
            return cached.icon;
        }

        private static void DrawParentTreeLines(Transform parentTransform, float startX, float iconOffset, float rectHeight, float yPos)
        {
            Color currentTreeColor = GetCachedTreeColor();

            while (parentTransform != null)
            {
                if (parentTransform.parent == null) break;

                startX -= iconOffset;
                Rect rect = new Rect(startX, yPos, rectHeight, rectHeight);

                if (parentTransform.GetSiblingIndex() != parentTransform.parent.childCount - 1)
                {
                    GUI.DrawTexture(rect, branchIcon_I, ScaleMode.ScaleToFit, true, 0, currentTreeColor, 0, 0);
                }

                parentTransform = parentTransform.parent;
            }
        }

        private static Color GetCachedTreeColor()
        {
            if (!isTreeColorCacheInitialized)
            {
                treeColorCache = HierarchyDesigner_Manager_Settings.TreeColor;
                isTreeColorCacheInitialized = true;
            }
            return treeColorCache;
        }

        public static void UpdateTreeColorCache()
        {
            isTreeColorCacheInitialized = false;
        }

        private static bool ShouldSkipDrawing(GameObject gameObject)
        {
            return HierarchyDesigner_Utility_Separator.IsSeparator(gameObject) || (HierarchyDesigner_Visual_Folder.IsFolder(gameObject) && !HierarchyDesigner_Manager_Settings.ShowComponentIconsForFolders);
        }
    }
}
#endif