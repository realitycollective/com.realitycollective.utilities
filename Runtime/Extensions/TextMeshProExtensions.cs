// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using TMPro;
using UnityEngine;

namespace RealityCollective.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for TextMeshPro components.
    /// </summary>
    public static class TextMeshProExtensions
    {
        /// <summary>
        /// Fades text in, waits for a duration, then fades out.
        /// </summary>
        /// <param name="textComponent">The TextMeshProUGUI component to animate</param>
        /// <param name="fadeDuration">Duration for fade in and fade out</param>
        /// <param name="pauseDuration">Duration to pause between fade in and fade out</param>
        /// <returns>Coroutine for the animation sequence</returns>
        public static IEnumerator FadeInOutText(this TextMeshProUGUI textComponent, float fadeDuration, float pauseDuration)
        {
            yield return textComponent.Fade(0, 1, fadeDuration);
            yield return new WaitForSeconds(pauseDuration);
            yield return textComponent.Fade(1, 0, fadeDuration);
        }

        /// <summary>
        /// Fades the alpha value of a TextMeshProUGUI component from start to end alpha over the specified duration.
        /// </summary>
        /// <param name="textComponent">The TextMeshProUGUI component to fade</param>
        /// <param name="startAlpha">Starting alpha value (0-1)</param>
        /// <param name="endAlpha">Ending alpha value (0-1)</param>
        /// <param name="duration">Duration of the fade in seconds</param>
        /// <returns>Coroutine for the fade animation</returns>
        public static IEnumerator Fade(this TextMeshProUGUI textComponent, float startAlpha, float endAlpha, float duration)
        {
            float elapsedTime = 0;
            Color color = textComponent.color;
            color.a = startAlpha;
            textComponent.color = color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                textComponent.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            // Ensure the final alpha is set
            textComponent.color = new Color(color.r, color.g, color.b, endAlpha);
        }
    }
}
