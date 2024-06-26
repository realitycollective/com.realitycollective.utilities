﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace RealityCollective.Utilities.Editor
{
    internal class AssemblyDefinitionPostprocessor : AssetPostprocessor
    {
        [Serializable]
        private class PackageInfo
        {
            [SerializeField]
            private string name;

            public string Name => name;

            [SerializeField]
            private string version;

            public string Version => version;
        }

        private const string VersionRegexPattern = "\\[assembly: AssemblyVersion\\(\"(.*)\"\\)\\]";

        private void OnPreprocessAsset()
        {
            if (assetPath.Contains("package.json") && !Application.isBatchMode)
            {
                if (Path.GetFullPath(assetPath).Contains("PackageCache"))
                {
                    return;
                }

                var text = File.ReadAllText(assetPath);
                var packageJson = JsonUtility.FromJson<PackageInfo>(text);

                if (!packageJson.Name.Contains("com.realitycollective") &&
                    !packageJson.Name.Contains("com.realitytoolkit"))
                {
                    return;
                }

                var packageVersion = packageJson.Version;

                if (packageVersion.Contains("-pre."))
                {
#if UNITY_2021_1_OR_NEWER                    
                    packageVersion = packageVersion[..packageVersion.IndexOf("-", StringComparison.Ordinal)];
#else
                    packageVersion = packageVersion.Substring(0, packageVersion.IndexOf("-", StringComparison.Ordinal));
#endif
                }

                var newVersion = $"[assembly: AssemblyVersion(\"{packageVersion}\")]";
                var asmdefs = Directory.GetFiles(assetPath.Replace("package.json", string.Empty), "*.asmdef", SearchOption.AllDirectories);

                foreach (var assembly in asmdefs)
                {
                    var assemblyName = Path.GetFileNameWithoutExtension(assembly).ToLower();
                    var directory = Path.GetDirectoryName(assembly);
                    var assemblyInfoPath = $"{directory}/AssemblyInfo.cs";
                    var fileText = !File.Exists(assemblyInfoPath)
                        ? $@"// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Reflection;

[assembly: AssemblyVersion(""0.0.0"")]
[assembly: AssemblyTitle(""com.{assemblyName}"")]
[assembly: AssemblyCompany(""Reality Collective"")]
[assembly: AssemblyCopyright(""Copyright (c) Reality Collective. All rights reserved."")]
"
                        : File.ReadAllText(assemblyInfoPath);

                    if (!fileText.Contains("AssemblyVersion"))
                    {
                        fileText += "\nusing System.Reflection;\n\n[assembly: AssemblyVersion(\"0.0.0\")]\n";
                    }

                    if (!fileText.Contains("AssemblyTitle"))
                    {
                        fileText += $"[assembly: AssemblyTitle(\"com.{assemblyName}\")]\n";
                    }

                    File.WriteAllText(assemblyInfoPath, Regex.Replace(fileText, VersionRegexPattern, newVersion));
                }

                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }
    }
}
