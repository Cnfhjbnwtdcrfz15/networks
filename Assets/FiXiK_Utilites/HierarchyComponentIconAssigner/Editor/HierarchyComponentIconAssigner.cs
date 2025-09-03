#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Michsky.UI.Shift;
using MiktoGames;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FiXiK.HierarchyComponentIconAssigner
{
    public static class HierarchyComponentIconAssigner
    {
        private static readonly Dictionary<Type, string> s_imageParams = new()
        {
            { typeof(Camera), "Camera.png" },
            { typeof(PlayerController), "Player.png" },
            { typeof(TMP_Text), "TMP.png"  },
            { typeof(MainPanelButton), "TMP.png"  },
            { typeof(ChapterButton), "TMP.png"  }
        };

        private static readonly Dictionary<Type, Texture2D> s_icons = new();

        private static bool _isSubscribed;

        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            EditorApplication.delayCall += () =>
            {
                Subscribe();
                LoadIcons();
            };
        }

        private static void Subscribe()
        {
            if (_isSubscribed)
                return;

            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyItem;
            EditorApplication.quitting += Unsubscribe;
            AssemblyReloadEvents.beforeAssemblyReload += Unsubscribe;
            _isSubscribed = true;
        }

        private static void Unsubscribe()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyItem;
            EditorApplication.quitting -= Unsubscribe;
            AssemblyReloadEvents.beforeAssemblyReload -= Unsubscribe;
            _isSubscribed = false;
        }

        private static void LoadIcons()
        {
            s_icons.Clear();

            foreach (var imageParams in s_imageParams)
            {
                string fileName = imageParams.Value;
                string nameNoExt = Path.GetFileNameWithoutExtension(fileName);
                Texture2D icon = LoadIconByName(nameNoExt, fileName);

                if (icon != null)
                    s_icons[imageParams.Key] = icon;
            }
        }

        private static Texture2D LoadIconByName(string nameNoExt, string fileName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Texture2D {nameNoExt}");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            return null;
        }

        private static void HandleHierarchyItem(int instanceID, Rect rect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (gameObject == null)
                return;

            foreach (var imageParams in s_icons)
            {
                if (gameObject.GetComponent(imageParams.Key) != null)
                {
                    Rect iconRect = new(rect.x, rect.y, 16f, 16f);
                    GUI.DrawTexture(iconRect, imageParams.Value);

                    return;
                }
            }
        }
    }
}
#endif