// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Editor.Extensions;
using RealityToolkit.Editor.Utilities;
using RealityToolkit.Editor.Utilities.SymbolicLinks;
using RealityToolkit.Extensions;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.Utilities.Editor
{
    public static class ToolkitPreferences
    {
        public const string Editor_Menu_Keyword = "Reality Toolkit";

        #region Debug Symbolic Links

        //private static readonly GUIContent DebugSymbolicContent = new GUIContent("Debug symbolic linking", "Enable or disable the debug information for symbolic linking.\n\nThis setting only applies to the currently running project.");
        private const string SYMBOLIC_DEBUG_KEY = "EnablePackageDebug";
        private static bool isSymbolicDebugPrefLoaded;
        private static bool debugSymbolicInfo;

        /// <summary>
        /// Enabled debugging info for the symbolic linking.
        /// </summary>
        public static bool DebugSymbolicInfo
        {
            get
            {
                if (!isSymbolicDebugPrefLoaded)
                {
                    debugSymbolicInfo = EditorPreferences.Get(SYMBOLIC_DEBUG_KEY, Application.isBatchMode);
                    isSymbolicDebugPrefLoaded = true;
                }

                return debugSymbolicInfo;
            }
            set => EditorPreferences.Set(SYMBOLIC_DEBUG_KEY, debugSymbolicInfo = value);
        }

        #endregion Debug Symbolic Links

        #region Symbolic Link Preferences

        private static bool isSymbolicLinkSettingsPathLoaded;
        private static string symbolicLinkSettingsPath = string.Empty;

        /// <summary>
        /// The path to the symbolic link settings found for this project.
        /// </summary>
        public static string SymbolicLinkSettingsPath
        {
            get
            {
                if (!isSymbolicLinkSettingsPathLoaded)
                {
                    symbolicLinkSettingsPath = EditorPreferences.Get("_SymbolicLinkSettingsPath", string.Empty);
                    isSymbolicLinkSettingsPathLoaded = true;
                }

                if (!EditorApplication.isUpdating &&
                    string.IsNullOrEmpty(symbolicLinkSettingsPath))
                {
                    symbolicLinkSettingsPath = AssetDatabase
                        .FindAssets($"t:{nameof(SymbolicLinkSettings)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .OrderBy(x => x)
                        .FirstOrDefault();
                }

                return symbolicLinkSettingsPath;
            }
            set => EditorPreferences.Set("_SymbolicLinkSettingsPath", symbolicLinkSettingsPath = value);
        }

        private static bool isAutoLoadSymbolicLinksLoaded;
        private static bool autoLoadSymbolicLinks = true;

        /// <summary>
        /// Should the project automatically load symbolic links?
        /// </summary>
        public static bool AutoLoadSymbolicLinks
        {
            get
            {
                if (!isAutoLoadSymbolicLinksLoaded)
                {
                    autoLoadSymbolicLinks = EditorPreferences.Get("_AutoLoadSymbolicLinks", true);
                    isAutoLoadSymbolicLinksLoaded = true;
                }

                return autoLoadSymbolicLinks;
            }
            set
            {
                EditorPreferences.Set("_AutoLoadSymbolicLinks", autoLoadSymbolicLinks = value);

                if (autoLoadSymbolicLinks && SymbolicLinker.Settings.IsNull())
                {
                    ScriptableObject.CreateInstance(nameof(SymbolicLinkSettings)).GetOrCreateAsset();
                }
            }
        }

        #endregion Symbolic Link Preferences
    }
}