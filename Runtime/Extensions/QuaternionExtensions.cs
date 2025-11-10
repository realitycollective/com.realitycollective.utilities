// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealityCollective.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Quaternion struct.
    /// </summary>
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Checks if a <see cref="Quaternion"/> instance is valid. A <see cref="Quaternion"/> is considered
        /// valid, if none of its components is <see cref="float.IsInfinity(float)"/> nor <see cref="float.IsNaN(float)"/>.
        /// </summary>
        /// <param name="rotation">The <see cref="Quaternion"/> to validate.</param>
        /// <returns>True, if valid rotation.</returns>
        public static bool IsValidRotation(this Quaternion rotation)
        {
            return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) &&
                   !float.IsInfinity(rotation.x) && !float.IsInfinity(rotation.y) && !float.IsInfinity(rotation.z) && !float.IsInfinity(rotation.w);
        }

        /// <summary>
        /// Checks the <see cref="Quaternion"/> can be approximately considered the same rotation when compared
        /// to another <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The original <see cref="Quaternion"/>.</param>
        /// <param name="other">The <see cref="Quaternion"/> to compare against.</param>
        /// <param name="threshold">Threshold in degrees to consider rotations the same.</param>
        /// <returns>True, if both rotations are approximately the same.</returns>
        public static bool Approximately(this Quaternion quaternion, Quaternion other, float threshold)
        {
            var qEuler = quaternion.eulerAngles;
            var otherEuler = other.eulerAngles;

            return
                Math.Abs(qEuler.x - otherEuler.x) <= threshold &&
                Math.Abs(qEuler.y - otherEuler.y) <= threshold &&
                Math.Abs(qEuler.z - otherEuler.z) <= threshold;
        }

        /// <summary>
        /// Slerps Quaternion source to goal, handles lerpTime of 0
        /// </summary>
        /// <param name="source"></param>
        /// <param name="goal"></param>
        /// <param name="deltaTime"></param>
        /// <param name="lerpTime"></param>
        /// <returns></returns>
        public static Quaternion SmoothTo(this Quaternion source, Quaternion goal, float deltaTime, float lerpTime)
        {
            return Quaternion.Slerp(source, goal, lerpTime.Equals(0.0f) ? 1f : deltaTime / lerpTime);
        }

        #region Precision-Preserving Methods (from Ethar Foundation)

        /// <summary>
        /// Rotates a Quaternion source by a Vector 3 angle
        /// </summary>
        /// <param name="source"><see cref="Quaternion"/> source value</param>
        /// <param name="rotation">Offset <see cref="Vector3"/> rotation axis</param>
        /// <returns>Rotated Quaternion</returns>
        public static Quaternion RotateBy(this Quaternion source, Vector3 rotation)
        {
            return source * Quaternion.Euler(rotation);
        }

        /// <summary>
        /// Converts a Quaternion to Euler angles with precision preservation.
        /// This method attempts to avoid precision loss due to gimbal lock and floating-point rounding
        /// by using rotation matrix decomposition and providing more stable angle extraction.
        /// </summary>
        /// <param name="quaternion">The quaternion to convert</param>
        /// <param name="previousEuler">Optional previous Euler angles for precision preservation</param>
        /// <returns>Precision-preserved Euler angles in [-180, 180] range</returns>
        public static Vector3 ToPrecisionEuler(this Quaternion quaternion, Vector3? previousEuler = null)
        {
            // First, check if the quaternion is effectively the same as the previous Euler angles
            // This helps preserve the original representation when no real change occurred
            if (previousEuler.HasValue && previousEuler.Value != Vector3.zero)
            {
                var previousQuaternion = previousEuler.Value.ToNormalizedQuaternion();
                float angleDifference = Quaternion.Angle(quaternion, previousQuaternion);
                
                // If the angle difference is very small (less than 0.01 degrees), preserve the previous Euler
                if (angleDifference < 0.01f)
                {
                    return previousEuler.Value;
                }
            }

            // Method 1: Try Unity's default conversion first
            var unityEuler = quaternion.eulerAngles;
            var normalizedUnityEuler = unityEuler.NormalizeEulerAngles();
            
            // Method 2: Try matrix-based extraction for gimbal lock scenarios
            Vector3? matrixBasedEuler = null;
            if (quaternion.IsGimbalLock())
            {
                matrixBasedEuler = quaternion.ExtractEulerFromRotationMatrix();
            }

            return matrixBasedEuler?.NormalizeEulerAngles() ?? normalizedUnityEuler;
        }

        /// <summary>
        /// Converts Euler angles to a Quaternion with normalization to ensure consistent conversion.
        /// </summary>
        /// <param name="euler">The Euler angles to convert</param>
        /// <returns>A normalized Quaternion representation of the rotation</returns>
        public static Quaternion ToNormalizedQuaternion(this Vector3 euler)
        {
            // Ensure the Euler angles are normalized before converting to Quaternion
            var normalizedEuler = euler.NormalizeEulerAngles();
            return Quaternion.Euler(normalizedEuler.x, normalizedEuler.y, normalizedEuler.z);
        }

        /// <summary>
        /// A precision-normalized identity quaternion that avoids floating-point errors.
        /// Use this instead of Quaternion.identity when rotation precision is critical.
        /// </summary>
        public static readonly Quaternion NormalizedIdentity = Vector3.zero.ToNormalizedQuaternion();

        /// <summary>
        /// Detects if a quaternion is in a gimbal lock scenario where Euler angle extraction is problematic.
        /// </summary>
        /// <param name="quaternion">The quaternion to check</param>
        /// <returns>True if the quaternion represents a gimbal lock scenario</returns>
        public static bool IsGimbalLock(this Quaternion quaternion)
        {
            // Check for Y-axis rotation near ±90 degrees (common gimbal lock case)
            // In gimbal lock, sin(pitch) ≈ ±1, which means |2(qw*qy + qx*qz)| ≈ 1
            float test = 2f * (quaternion.w * quaternion.y + quaternion.x * quaternion.z);
            return Mathf.Abs(test) > 0.998f; // Close to ±1 indicates gimbal lock
        }

        /// <summary>
        /// Extracts Euler angles from a quaternion using rotation matrix decomposition.
        /// This can be more stable than Unity's eulerAngles property in certain cases.
        /// </summary>
        /// <param name="quaternion">The quaternion to convert</param>
        /// <returns>Euler angles extracted from rotation matrix, or null if extraction fails</returns>
        public static Vector3? ExtractEulerFromRotationMatrix(this Quaternion quaternion)
        {
            try
            {
                // Convert quaternion to rotation matrix elements we need
                float qw = quaternion.w, qx = quaternion.x, qy = quaternion.y, qz = quaternion.z;
                
                // Calculate rotation matrix elements
                float m11 = 1 - 2 * (qy * qy + qz * qz);
                float m12 = 2 * (qx * qy - qw * qz);
                float m13 = 2 * (qx * qz + qw * qy);
                float m22 = 1 - 2 * (qx * qx + qz * qz);
                float m23 = 2 * (qy * qz - qw * qx);
                float m33 = 1 - 2 * (qx * qx + qy * qy);

                // Extract Euler angles using atan2 for better numerical stability
                float rotX, rotY, rotZ;

                // Check for gimbal lock
                if (Mathf.Abs(m13) >= 0.998f)
                {
                    // Gimbal lock case - set one rotation to 0 and solve for the other two
                    rotY = Mathf.Sign(m13) * 90f; // ±90 degrees
                    rotZ = 0f; // Set Z to 0
                    rotX = Mathf.Atan2(-m23, m22) * Mathf.Rad2Deg;
                }
                else
                {
                    // Normal case - extract all three angles
                    rotY = Mathf.Asin(Mathf.Clamp(m13, -1f, 1f)) * Mathf.Rad2Deg;
                    rotX = Mathf.Atan2(-m23, m33) * Mathf.Rad2Deg;
                    rotZ = Mathf.Atan2(-m12, m11) * Mathf.Rad2Deg;
                }

                return new Vector3(rotX, rotY, rotZ);
            }
            catch
            {
                // If matrix decomposition fails, return null
                return null;
            }
        }

        /// <summary>
        /// Compares two quaternions for near-equality using dot product.
        /// This is more reliable than direct component comparison due to quaternion dual-representation.
        /// </summary>
        /// <param name="quaternion">The first quaternion</param>
        /// <param name="other">The quaternion to compare with</param>
        /// <param name="tolerance">The tolerance for comparison (default: 0.99999f for high precision)</param>
        /// <returns>True if the quaternions represent nearly the same rotation</returns>
        public static bool IsNearlyEqual(this Quaternion quaternion, Quaternion other, float tolerance = 0.99999f)
        {
            return Mathf.Abs(Quaternion.Dot(quaternion, other)) >= tolerance;
        }

        /// <summary>
        /// Gets the angular difference between two quaternions in degrees.
        /// </summary>
        /// <param name="quaternion">The first quaternion</param>
        /// <param name="other">The quaternion to compare with</param>
        /// <returns>The angular difference in degrees</returns>
        public static float AngleTo(this Quaternion quaternion, Quaternion other)
        {
            return Quaternion.Angle(quaternion, other);
        }

        #endregion

    }
}