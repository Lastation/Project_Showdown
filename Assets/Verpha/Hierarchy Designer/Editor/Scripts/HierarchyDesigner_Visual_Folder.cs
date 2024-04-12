#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Verpha.HierarchyDesigner
{
    [InitializeOnLoad]
    public class HierarchyDesigner_Visual_Folder
    {
        private static readonly Texture2D backgroundImage = HierarchyDesigner_Manager_Background.BackgroundImage;
        private static Dictionary<int, WeakReference<GameObject>> gameObjectCache = new Dictionary<int, WeakReference<GameObject>>();
        private static Dictionary<string, (Color folderColor, HierarchyDesigner_Info_Folder.FolderImageType folderImageType)> folderInfoCache = new Dictionary<string, (Color, HierarchyDesigner_Info_Folder.FolderImageType)>();

        static HierarchyDesigner_Visual_Folder()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
            HierarchyFolderWindow.LoadFolders();
        }

        private static void HierarchyItemCB(int instanceID, Rect selectionRect)
        {
            if (HierarchyDesigner_Manager_Settings.DisableHierarchyDesignerDuringPlayMode && EditorApplication.isPlaying) return;
            if (!TryGetGameObject(instanceID, out GameObject gameObject)) return;

            if (IsFolder(gameObject))
            {
                var (folderColor, folderImageType) = GetFolderInfo(gameObject.name);
                DrawFolderIcon(selectionRect, folderColor, folderImageType, instanceID);
            }
        }

        private static bool TryGetGameObject(int instanceID, out GameObject gameObject)
        {
            if (!gameObjectCache.TryGetValue(instanceID, out WeakReference<GameObject> weakRef) || !weakRef.TryGetTarget(out gameObject))
            {
                gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (gameObject != null)
                {
                    gameObjectCache[instanceID] = new WeakReference<GameObject>(gameObject);
                }
                else
                {
                    gameObjectCache.Remove(instanceID);
                }
            }
            return gameObject != null;
        }

        public static bool IsFolder(GameObject gameObject)
        {
            if (gameObject == null) return false;
            return gameObject.GetComponent<HierarchyDesignerFolder>() != null;
        }

        private static void DrawFolderIcon(Rect selectionRect, Color folderColor, HierarchyDesigner_Info_Folder.FolderImageType folderImageType, int instanceID)
        {
            bool isHovering = Event.current.type == EventType.Repaint && selectionRect.Contains(Event.current.mousePosition);
            
            GUI.color = HierarchyDesigner_Shared_ColorUtility.GetBackgroundColor(isHovering, instanceID);
            if (backgroundImage != null)
            {
                float iconSize = selectionRect.height * 1.0f;
                float iconYPosition = selectionRect.y + (selectionRect.height - iconSize) / 2;
                Rect backgroundRect = new Rect(selectionRect.x, iconYPosition, iconSize, iconSize);
                GUI.DrawTexture(backgroundRect, backgroundImage);
            }
            GUI.color = Color.white;

            Texture2D folderIcon = HierarchyDesigner_Manager_Folder.GetFolderIcon(folderImageType);
            if (folderIcon != null)
            {
                GUI.color = folderColor;
                Rect iconRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
                GUI.DrawTexture(iconRect, folderIcon);
                GUI.color = Color.white;
            }
        }

        private static (Color folderColor, HierarchyDesigner_Info_Folder.FolderImageType folderImageType) GetFolderInfo(string folderName)
        {
            if (!folderInfoCache.TryGetValue(folderName, out var info))
            {
                if (HierarchyFolderWindow.folders.TryGetValue(folderName, out var folder))
                {
                    info = (folder.FolderColor, folder.ImageType);
                    folderInfoCache[folderName] = info;
                }
                else
                {
                    info = (Color.white, HierarchyDesigner_Info_Folder.FolderImageType.Default);
                }
            }
            return info;
        }

        public static void UpdateFolderVisuals()
        {
            folderInfoCache.Clear();
            foreach (var kvp in HierarchyFolderWindow.folders)
            {
                string folderName = kvp.Key;
                HierarchyDesigner_Info_Folder folder = kvp.Value;
                folderInfoCache[folderName] = (folder.FolderColor, folder.ImageType);
            }
            gameObjectCache.Clear();
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
#endif