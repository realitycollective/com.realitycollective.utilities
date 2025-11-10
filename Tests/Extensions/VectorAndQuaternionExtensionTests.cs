// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using RealityCollective.Utilities.Extensions;

namespace RealityCollective.Utilities.Tests.Extensions
{
    [TestFixture]
    public class VectorAndQuaternionExtensionTests
    {
        #region Vector3 Euler Angle Tests

        [Test]
        public void NormalizeEulerAngles_HandlesStandardAngles()
        {
            // Test standard angles that should remain unchanged
            Vector3 input = new Vector3(45f, 90f, 135f);
            Vector3 result = input.NormalizeEulerAngles();
            
            Assert.AreEqual(45f, result.x, 0.001f);
            Assert.AreEqual(90f, result.y, 0.001f);
            Assert.AreEqual(135f, result.z, 0.001f);
        }

        [Test]
        public void NormalizeEulerAngles_ConvertsLargeAngles()
        {
            // Test angles > 360° get normalized to [-180, 180] range
            Vector3 input = new Vector3(450f, -450f, 720f);
            Vector3 result = input.NormalizeEulerAngles();
            
            // 450° should become 90° (450 - 360 = 90)
            Assert.AreEqual(90f, result.x, 0.001f);
            // -450° should become -90° (-450 + 360 = -90)
            Assert.AreEqual(-90f, result.y, 0.001f);
            // 720° should become 0° (720 - 720 = 0)
            Assert.AreEqual(0f, result.z, 0.001f);
        }

        [Test]
        public void NormalizeEulerAngles_HandlesEdgeCases()
        {
            // Test angles exactly at 180° and -180°
            Vector3 input = new Vector3(180f, -180f, 0f);
            Vector3 result = input.NormalizeEulerAngles();
            
            Assert.AreEqual(180f, result.x, 0.001f);
            Assert.AreEqual(-180f, result.y, 0.001f);
            Assert.AreEqual(0f, result.z, 0.001f);
        }

        [Test]
        public void IsNearlyEqualEuler_DetectsSimilarAngles()
        {
            Vector3 euler1 = new Vector3(45.001f, 90.0005f, 135.0001f);
            Vector3 euler2 = new Vector3(45.0005f, 90.001f, 135.0005f);
            
            // With default tolerance (0.001), these should be considered equal
            Assert.IsTrue(euler1.IsNearlyEqualEuler(euler2, 0.01f));
            
            // With very strict tolerance, they should not be equal
            Assert.IsFalse(euler1.IsNearlyEqualEuler(euler2, 0.0001f));
        }

        [Test]
        public void AngleDifference_CalculatesCorrectDifferences()
        {
            Vector3 euler1 = new Vector3(10f, 20f, 30f);
            Vector3 euler2 = new Vector3(15f, 25f, 35f);
            Vector3 difference = euler1.AngleDifference(euler2);
            
            Assert.AreEqual(-5f, difference.x, 0.001f);
            Assert.AreEqual(-5f, difference.y, 0.001f);
            Assert.AreEqual(-5f, difference.z, 0.001f);
        }

        [Test]
        public void MaxAngleDifference_ReturnsLargestDifference()
        {
            Vector3 euler1 = new Vector3(10f, 20f, 30f);
            Vector3 euler2 = new Vector3(15f, 35f, 32f);
            float maxDiff = euler1.MaxAngleDifference(euler2);
            
            // Differences are: 5°, 15°, 2° - max should be 15°
            Assert.AreEqual(15f, maxDiff, 0.001f);
        }

        #endregion

        #region Quaternion Extension Tests

        [Test]
        public void IsValidRotation_AcceptsValidQuaternions()
        {
            Quaternion validQuat = Quaternion.Euler(45f, 90f, 135f);
            Assert.IsTrue(validQuat.IsValidRotation());
            
            Quaternion identity = Quaternion.identity;
            Assert.IsTrue(identity.IsValidRotation());
        }

        [Test]
        public void IsValidRotation_RejectsInvalidQuaternions()
        {
            // Create invalid quaternions with NaN values
            Quaternion invalidNaN = new Quaternion(float.NaN, 0f, 0f, 1f);
            Assert.IsFalse(invalidNaN.IsValidRotation());
            
            // Create invalid quaternions with Infinity values
            Quaternion invalidInfinity = new Quaternion(float.PositiveInfinity, 0f, 0f, 1f);
            Assert.IsFalse(invalidInfinity.IsValidRotation());
        }

        [Test]
        public void Approximately_DetectsSimilarRotations()
        {
            Quaternion q1 = Quaternion.Euler(45f, 90f, 135f);
            Quaternion q2 = Quaternion.Euler(45.5f, 90.5f, 135.5f);
            
            // With 1 degree threshold, these should be approximately equal
            Assert.IsTrue(q1.Approximately(q2, 1f));
            
            // With 0.1 degree threshold, they should not be equal
            Assert.IsFalse(q1.Approximately(q2, 0.1f));
        }

        [Test]
        public void SmoothTo_HandlesZeroLerpTime()
        {
            Quaternion source = Quaternion.Euler(0f, 0f, 0f);
            Quaternion goal = Quaternion.Euler(90f, 0f, 0f);
            
            // With lerpTime = 0, should immediately return goal
            Quaternion result = source.SmoothTo(goal, 0.1f, 0f);
            Assert.IsTrue(Quaternion.Angle(result, goal) < 0.01f);
        }

        [Test]
        public void RotateBy_CombinesRotationsCorrectly()
        {
            Quaternion initial = Quaternion.Euler(45f, 0f, 0f);
            Vector3 additionalRotation = new Vector3(0f, 90f, 0f);
            
            Quaternion result = initial.RotateBy(additionalRotation);
            
            // Verify the rotation has been applied
            Vector3 resultEuler = result.eulerAngles;
            Assert.AreNotEqual(initial.eulerAngles, resultEuler);
        }

        [Test]
        public void ToNormalizedQuaternion_ProducesConsistentResults()
        {
            Vector3 euler = new Vector3(450f, -270f, 180f); // Non-normalized angles
            Quaternion quat1 = euler.ToNormalizedQuaternion();
            
            // Normalize the euler angles manually and convert
            Vector3 normalizedEuler = euler.NormalizeEulerAngles();
            Quaternion quat2 = Quaternion.Euler(normalizedEuler);
            
            // Results should be very similar
            Assert.IsTrue(Quaternion.Angle(quat1, quat2) < 0.01f);
        }

        [Test]
        public void ToPrecisionEuler_PreservesSimpleRotations()
        {
            Vector3 originalEuler = new Vector3(45f, 90f, 135f);
            Quaternion quat = Quaternion.Euler(originalEuler);
            Vector3 precisionEuler = quat.ToPrecisionEuler();
            
            // Should preserve the original angles (within reasonable tolerance)
            Assert.IsTrue(originalEuler.IsNearlyEqualEuler(precisionEuler, 1f));
        }

        [Test]
        public void IsGimbalLock_DetectsGimbalLockScenarios()
        {
            // Quaternion representing 90 degree Y rotation (gimbal lock case)
            Quaternion gimbalLockQuat = Quaternion.Euler(0f, 90f, 0f);
            Assert.IsTrue(gimbalLockQuat.IsGimbalLock());
            
            // Normal rotation should not be gimbal lock
            Quaternion normalQuat = Quaternion.Euler(45f, 45f, 45f);
            Assert.IsFalse(normalQuat.IsGimbalLock());
        }

        [Test]
        public void IsNearlyEqual_ComparesQuaternionsCorrectly()
        {
            Quaternion q1 = Quaternion.Euler(45f, 90f, 135f);
            Quaternion q2 = Quaternion.Euler(45.01f, 90.01f, 135.01f);
            
            // Very similar quaternions should be nearly equal
            Assert.IsTrue(q1.IsNearlyEqual(q2, 0.99f));
            
            // Very different quaternions should not be nearly equal
            Quaternion q3 = Quaternion.Euler(180f, 0f, 0f);
            Assert.IsFalse(q1.IsNearlyEqual(q3, 0.99f));
        }

        [Test]
        public void AngleTo_CalculatesAngularDifference()
        {
            Quaternion q1 = Quaternion.identity;
            Quaternion q2 = Quaternion.Euler(90f, 0f, 0f);
            
            float angle = q1.AngleTo(q2);
            Assert.AreEqual(90f, angle, 0.1f);
        }

        #endregion

        #region Vector2 Extension Tests

        [Test]
        public void Vector2_Mul_MultipliesComponentWise()
        {
            Vector2 v1 = new Vector2(2f, 3f);
            Vector2 v2 = new Vector2(4f, 5f);
            Vector2 result = v1.Mul(v2);
            
            Assert.AreEqual(8f, result.x, 0.001f);
            Assert.AreEqual(15f, result.y, 0.001f);
        }

        [Test]
        public void Vector2_Div_DividesComponentWise()
        {
            Vector2 v1 = new Vector2(8f, 15f);
            Vector2 v2 = new Vector2(4f, 5f);
            Vector2 result = v1.Div(v2);
            
            Assert.AreEqual(2f, result.x, 0.001f);
            Assert.AreEqual(3f, result.y, 0.001f);
        }

        [Test]
        public void Vector2_RotateAroundPoint_RotatesCorrectly()
        {
            Vector2 point = new Vector2(1f, 0f);
            Vector2 pivot = Vector2.zero;
            float angle = Mathf.PI / 2f; // 90 degrees in radians
            
            Vector2 rotated = point.RotateAroundPoint(pivot, angle);
            
            // Should rotate to approximately (0, 1)
            Assert.AreEqual(0f, rotated.x, 0.001f);
            Assert.AreEqual(1f, rotated.y, 0.001f);
        }

        [Test]
        public void Vector2_Average_CalculatesCorrectAverage()
        {
            Vector2[] vectors = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(2f, 2f),
                new Vector2(4f, 4f)
            };
            
            Vector2 average = vectors.Average();
            Assert.AreEqual(2f, average.x, 0.001f);
            Assert.AreEqual(2f, average.y, 0.001f);
        }

        [Test]
        public void Vector2_MidPoint_CalculatesCorrectMidpoint()
        {
            Vector2 v1 = new Vector2(0f, 0f);
            Vector2 v2 = new Vector2(4f, 6f);
            Vector2 midpoint = v1.MidPoint(v2);
            
            Assert.AreEqual(2f, midpoint.x, 0.001f);
            Assert.AreEqual(3f, midpoint.y, 0.001f);
        }

        #endregion

        #region Vector3 General Extension Tests

        [Test]
        public void Vector3_Average_CalculatesCorrectAverage()
        {
            Vector3[] vectors = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(3f, 6f, 9f),
                new Vector3(6f, 12f, 18f)
            };
            
            Vector3 average = vectors.Average();
            Assert.AreEqual(3f, average.x, 0.001f);
            Assert.AreEqual(6f, average.y, 0.001f);
            Assert.AreEqual(9f, average.z, 0.001f);
        }

        [Test]
        public void Vector3_RotateAroundPoint_RotatesCorrectly()
        {
            Vector3 point = new Vector3(1f, 0f, 0f);
            Vector3 pivot = Vector3.zero;
            Vector3 axis = Vector3.up;
            float angle = 90f;
            
            Vector3 rotated = point.RotateAroundPoint(pivot, angle, axis);
            
            // Should rotate to approximately (0, 0, 1) when rotated 90° around Y-axis
            Assert.AreEqual(0f, rotated.x, 0.001f);
            Assert.AreEqual(0f, rotated.y, 0.001f);
            Assert.AreEqual(-1f, rotated.z, 0.001f); // Unity's coordinate system
        }

        #endregion
    }
}
