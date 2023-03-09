// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;

namespace RealityCollective.Extensions
{
    public static class TypeExtensions
    {
        internal static Dictionary<Guid, Type> BuildTypeCache(Dictionary<Guid, Type> typeCache)
        {
            if (typeCache == null)
            {
                throw new ArgumentNullException("typeCache", "No type cache dictionary supplied");
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().filterIgnoredDomains();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.BaseType != null && type.BaseType.Name.Contains("Delegate"))
                    {
                        continue;
                    }
                    if (type.IsClass && !type.IsAbstract && type.GUID != Guid.Empty)
                    {
                        try
                        {
                            var guid = type.GUID;
                            if (!typeCache.ContainsKey(guid))
                            {
                                typeCache.Add(guid, type);
                            }
                        }
                        catch (Exception ex)
                        {
                            // In some cases at runtime in a player build built using
                            // IL2CPP accessing Type.GUID throws an unsupported exception crashing the application.
                            // Tests have shown that catching the exception prevents the app from crashing
                            // without actually breaking functionality of the application.
                            // TODO: Why are some types causing these exceptions?
                            Debug.LogError($"Failed to add {type.Name} to type cache.");
                            Debug.LogException(ex);
                        }
                    }
                }
            }
            return typeCache;
        }

        private static string[] ignoredDomains = new string[]
        {
            "UnityEngine",
            "Unity",
            "System",
            "Mono",
            "NetStandard",
            "nunit",
            "log4net",
            "Bee",
            "NiceIO"
        };

        private static Assembly[] filterIgnoredDomains(this Assembly[] assemblies)
        {
            List<Assembly> returnAssemblies = new List<Assembly>();
            for (int i = assemblies.Length - 1; i > 0; i--)
            {
                bool ignoreAssembly = false;

                for (int j = ignoredDomains.Length - 1; j > 0; j--)
                {
                    if (assemblies[i].FullName.ToLower().Contains(ignoredDomains[j].ToLower()))
                    {
                        ignoreAssembly = true;
                    }
                }

                if (!ignoreAssembly)
                {
                    returnAssemblies.Add(assemblies[i]);
                }
            }
            return returnAssemblies.ToArray();
        }

        /// <summary>
        /// Attempts to resolve the type using the class <see cref="Guid"/>.
        /// </summary>
        /// <param name="guid">Class <see cref="Guid"/> reference.</param>
        /// <param name="resolvedType">The resolved <see cref="Type"/>.</param>
        /// <returns>True if the <see cref="resolvedType"/> was successfully obtained from or added to the <see cref="TypeCache"/>, otherwise false.</returns>
        public static bool TryResolveType(Guid guid, out Type resolvedType)
        {
            resolvedType = null;

            if (guid == Guid.Empty)
            {
                return false;
            }

            if (!TypeCache.Current.TryGetValue(guid, out resolvedType))
            {
                //Serious enough to put severe logging here as it will cause you many hours of hair pulling, only to find it is because Unity removed the class to be helpful with its Code Stripping functionality.
                //Add a Link.XML to the project and sleep better.
                var message = $"Configured Type Guid [{guid}] not found, either missing or lost to the curring room floor of Unity Code Stripping.\n If the class is in the project, consider including it in a Link.xml";
                Debug.LogError(message);
                System.Diagnostics.Debug.WriteLine(message);
                return false;
            }

            if (resolvedType != null && !resolvedType.IsAbstract)
            {
                if (!TypeCache.Current.ContainsKey(guid))
                {
                    TypeCache.Current.Add(guid, resolvedType);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to resolve the type using a the <see cref="System.Type.AssemblyQualifiedName"/> or <see cref="Type.GUID"/> as <see cref="string"/>.
        /// </summary>
        /// <param name="typeRef">The <see cref="Type.GUID"/> or <see cref="System.Type.AssemblyQualifiedName"/> as <see cref="string"/>.</param>
        /// <param name="resolvedType">The resolved <see cref="Type"/>.</param>
        /// <returns>True if the <see cref="resolvedType"/> was successfully obtained from or added to the <see cref="TypeCache"/>, otherwise false.</returns>
        public static bool TryResolveType(string typeRef, out Type resolvedType)
        {
            resolvedType = null;

            if (string.IsNullOrEmpty(typeRef)) { return false; }

            if (Guid.TryParse(typeRef, out var guid))
            {
                return TryResolveType(guid, out resolvedType);
            }

            resolvedType = Type.GetType(typeRef);

            if (resolvedType != null)
            {
                if (resolvedType.GUID != Guid.Empty)
                {
                    return TryResolveType(guid, out resolvedType);
                }

                if (!resolvedType.IsAbstract)
                {
                    Debug.LogWarning($"{resolvedType.Name} is missing a {nameof(GuidAttribute)}. This extension has been upgraded to use System.Type.GUID instead of System.Type.AssemblyQualifiedName");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recursively looks for generic type arguments in type hierarchy, starting with the
        /// root type provided. If no generic type arguments are found on a type, it's base
        /// type is checked.
        /// </summary>
        /// <param name="root">Root type to start looking for generic type arguments at.</param>
        /// <param name="maxRecursionDepth">The maximum recursion depth until execution gets canceled even if no results found.</param>
        /// <returns>Found generic type arguments array or null, if none found.</returns>
        public static Type[] FindTopmostGenericTypeArguments(this Type root, int maxRecursionDepth = 5)
        {
            var genericTypeArgs = root?.GenericTypeArguments;

            if (genericTypeArgs != null && genericTypeArgs.Length > 0)
            {
                return genericTypeArgs;
            }

            if (maxRecursionDepth > 0 && root != null)
            {
                return FindTopmostGenericTypeArguments(root.BaseType, --maxRecursionDepth);
            }

            Debug.LogError($"{nameof(FindTopmostGenericTypeArguments)} - Maximum recursion depth reached without finding generic type arguments.");
            return null;
        }
    }
}