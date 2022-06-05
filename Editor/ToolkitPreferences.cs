// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Editor.Utilities;
using UnityEngine;

namespace RealityCollective.Utilities.Editor
{
    public static class ToolkitPreferences
    {
        public const string Editor_Menu_Keyword = "Reality Collective";

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
    }
}