// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace RealityCollective.Utilities.Extensions
{
    /// <summary>
    /// Extension methods and utility functions for file loading operations.
    /// </summary>
    public static class FileLoadingExtensions
    {
        /// <summary>
        /// Loads an image from a file path into a Texture2D.
        /// </summary>
        /// <param name="filePath">The path to the image file</param>
        /// <returns>A Texture2D containing the loaded image, or null if loading fails</returns>
        /// <exception cref="ArgumentNullException">Thrown when filePath is null or empty</exception>
        public static Texture2D LoadImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path is null or empty");
            }

            Texture2D tex = null;

            if (File.Exists(filePath))
            {
                var fileData = LoadFileBytes(filePath);
                if (fileData != null && fileData.Length > 0)
                {
                    tex = new Texture2D(2, 2);
                    ImageConversion.LoadImage(tex, fileData);
                }
            }
            return tex;
        }

        /// <summary>
        /// Loads the raw bytes from a file.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>A byte array containing the file data, or null if loading fails</returns>
        /// <exception cref="ArgumentNullException">Thrown when filePath is null or empty</exception>
        public static byte[] LoadFileBytes(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path is null or empty");
            }

            byte[] fileData = null;

            if (File.Exists(filePath))
            {
                try
                {
                    fileData = File.ReadAllBytes(filePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load file bytes from {filePath}: {ex.Message}");
                }
            }
            return fileData;
        }
    }
}
