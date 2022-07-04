// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;

namespace RealityCollective.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Sprite class
    /// </summary>
    public static class SpriteExtensions
    {
        /// <summary>
        /// Converts a Unity <see cref="Sprite"/> into a <see cref="Texture2D"/>
        /// </summary>
        /// <param name="sprite">A Unity <see cref="Sprite"/> object</param>
        /// <returns>A <see cref="Texture2D"/> object</returns>
        public static Texture2D TextureFromSprite(this Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
            {
                return sprite.texture;
            }
        }
    }
}