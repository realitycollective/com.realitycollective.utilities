// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using XRTK.Editor;
using XRTK.Editor.Utilities;
using XRTK.Extensions;

namespace RealityToolkit.UPMTEMPLATE.Editor
{
    [InitializeOnLoad]
    internal static class UPMTEMPLATEPackageInstaller
    {
        private static readonly string DefaultPath = $"{MixedRealityPreferences.ProfileGenerationPath}UPMTEMPLATE";
        private static readonly string HiddenPath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(UPMTEMPLATEPathFinder)).ForwardSlashes()}{Path.DirectorySeparatorChar}{MixedRealityPreferences.HIDDEN_PACKAGE_ASSETS_PATH}");

        static UPMTEMPLATEPackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        [MenuItem("Reality Toolkit/Packages/Install UPMTEMPLATE Package Assets...", true)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{DefaultPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem("Reality Toolkit/Packages/Install UPMTEMPLATE Package Assets...")]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(UPMTEMPLATEPackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(UPMTEMPLATEPackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(UPMTEMPLATEPackageInstaller)}.Assets", PackageInstaller.TryInstallAssets(HiddenPath, DefaultPath));
            }
        }
    }
}
