// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using RealityCollective.Definitions.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace RealityCollective.Extensions
{
    /// <summary>
    /// Extension methods for Unity's <see cref="Bounds"/>
    /// </summary>
    public static class BoundsExtensions
    {
        // Corners
        public const int LBF = 0;
        public const int LBB = 1;
        public const int LTF = 2;
        public const int LTB = 3;
        public const int RBF = 4;
        public const int RBB = 5;
        public const int RTF = 6;
        public const int RTB = 7;

        // X axis
        public const int LTF_RTF = 8;
        public const int LBF_RBF = 9;
        public const int RTB_LTB = 10;
        public const int RBB_LBB = 11;

        // Y axis
        public const int LTF_LBF = 12;
        public const int RTB_RBB = 13;
        public const int LTB_LBB = 14;
        public const int RTF_RBF = 15;

        // Z axis
        public const int RBF_RBB = 16;
        public const int RTF_RTB = 17;
        public const int LBF_LBB = 18;
        public const int LTF_LTB = 19;

        // 2D corners
        public const int LT = 0;
        public const int LB = 1;
        public const int RT = 2;
        public const int RB = 3;

        // 2D midpoints
        public const int LT_RT = 4;
        public const int RT_RB = 5;
        public const int RB_LB = 6;
        public const int LB_LT = 7;

        // Face points
        public const int TOP = 0;
        public const int BOT = 1;
        public const int LFT = 2;
        public const int RHT = 3;
        public const int FWD = 4;
        public const int BCK = 5;

        private static Vector3[] corners = null;

        private static readonly Vector3[] rectTransformCorners = new Vector3[4];

        // Axis of the capsule’s lengthwise orientation in the object’s local space
        private const int CAPSULE_X_AXIS = 0;
        private const int CAPSULE_Y_AXIS = 1;
        private const int CAPSULE_Z_AXIS = 2;

        // Edges used to render the bounds.
        private static readonly int[] boundsEdges = new int[]
        {
             LBF, LBB,
             LBB, LTB,
             LTB, LTF,
             LTF, LBF,
             LBF, RTB,
             RTB, RTF,
             RTF, RBF,
             RBF, RBB,
             RBB, RTB,
             RTF, LBB,
             RBF, LTB,
             RBB, LTF
        };

        #region Public Static Functions

        /// <summary>
        /// Returns an instance of the <see cref="Bounds"/> class which is invalid. 
        /// An invalid <see cref="Bounds"/> instance is one which has its size vector set to 'float.MaxValue' for all 3 components.
        /// The center of an invalid bounds instance is the zero vector.
        /// </summary>
        /// <returns>Returns an instance of the <see cref="Bounds"/> class which is invalid.</returns>
        public static Bounds GetInvalidBoundsInstance()
        {
            return new Bounds(Vector3.zero, GetInvalidBoundsSize());
        }

        /// <summary>
        /// Checks if the specified bounds instance is valid. A valid 'Bounds' instance is
        /// one whose size vector does not have all 3 components set to 'float.MaxValue'.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        public static bool IsValid(this Bounds bounds)
        {
            return bounds.size != GetInvalidBoundsSize();
        }

        /// <summary>
        /// Gets all the corner points of the bounds in world space.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="transform">A relative transform point.</param>
        /// <param name="positions">A reference <see cref="Vector3"/> array of 8 points to populate.</param>
        public static void GetCornerPositionsWorldSpace(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            // Calculate the local points to transform.
            var center = bounds.center;
            var extents = bounds.extents;
            var leftEdge = center.x - extents.x;
            var rightEdge = center.x + extents.x;
            var bottomEdge = center.y - extents.y;
            var topEdge = center.y + extents.y;
            var frontEdge = center.z - extents.z;
            var backEdge = center.z + extents.z;

            // Allocate the array if needed.
            const int numPoints = 8;

            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            // Transform all the local points to world space.
            positions[LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
            positions[LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
            positions[LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
            positions[RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
            positions[RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
            positions[RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);
        }

        /// <summary>
        /// Gets all the corner points of the bounds in local space.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="positions">A reference <see cref="Vector3"/> array of 8 points to populate.</param>
        public static void GetCornerPositionsLocalSpace(this Bounds bounds, ref Vector3[] positions)
        {
            // Allocate the array if needed.
            const int numCorners = 8;

            if (positions == null || positions.Length != numCorners)
            {
                positions = new Vector3[numCorners];
            }

            // Permutate all axes using minCorner and maxCorner.
            var minCorner = bounds.center - bounds.extents;
            var maxCorner = bounds.center + bounds.extents;

            for (int cornerIndex = 0; cornerIndex < numCorners; cornerIndex++)
            {
                positions[cornerIndex] = new Vector3(
                    (cornerIndex & (1 << 0)) == 0 ? minCorner[0] : maxCorner[0],
                    (cornerIndex & (1 << 1)) == 0 ? minCorner[1] : maxCorner[1],
                    (cornerIndex & (1 << 2)) == 0 ? minCorner[2] : maxCorner[2]);
            }
        }

        /// <summary>
        /// Gets all the corner points of the bounds in world space by transforming input bounds using the given transform
        /// </summary>
        /// <param name="transform">Local to world transform</param>
        /// <param name="positions">Output corner positions</param>
        /// <param name="bounds">Input bounds, in local space</param>
        /// <remarks>
        /// <para>Use BoxColliderExtensions.{Left|Right}{Bottom|Top}{Front|Back} consts to index into the output
        /// corners array.</para>
        /// </remarks>
        public static void GetCornerPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            // Calculate the local points to transform.
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            // Allocate the array if needed.
            const int numPoints = 8;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            // Transform all the local points to world space.
            positions[LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
            positions[LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
            positions[LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
            positions[RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
            positions[RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
            positions[RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);
        }

        /// <summary>
        /// Gets all the corner points of the bounds.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="positions">A reference <see cref="Vector3"/> array of 8 points to populate.</param>
        /// <remarks>
        /// <see cref="Collider.bounds"/> is world space bounding volume.
        /// <see cref="Mesh.bounds"/> is local space bounding volume.
        /// <see cref="Renderer.bounds"/>  is the same as <see cref="Mesh.bounds"/> but in world space coords.
        /// </remarks>
        public static void GetCornerPositions(this Bounds bounds, ref Vector3[] positions)
        {
            var center = bounds.center;
            var extents = bounds.extents;
            var leftEdge = center.x - extents.x;
            var rightEdge = center.x + extents.x;
            var bottomEdge = center.y - extents.y;
            var topEdge = center.y + extents.y;
            var frontEdge = center.z - extents.z;
            var backEdge = center.z + extents.z;

            const int numPoints = 8;

            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            positions[LBF] = new Vector3(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = new Vector3(leftEdge, bottomEdge, backEdge);
            positions[LTF] = new Vector3(leftEdge, topEdge, frontEdge);
            positions[LTB] = new Vector3(leftEdge, topEdge, backEdge);
            positions[RBF] = new Vector3(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = new Vector3(rightEdge, bottomEdge, backEdge);
            positions[RTF] = new Vector3(rightEdge, topEdge, frontEdge);
            positions[RTB] = new Vector3(rightEdge, topEdge, backEdge);
        }

        /// <summary>
        /// Gets all the corner points from Renderer's Bounds
        /// </summary>
        public static void GetCornerPositionsFromRendererBounds(this Bounds bounds, ref Vector3[] positions)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            const int numPoints = 8;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            positions[LBF] = new Vector3(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = new Vector3(leftEdge, bottomEdge, backEdge);
            positions[LTF] = new Vector3(leftEdge, topEdge, frontEdge);
            positions[LTB] = new Vector3(leftEdge, topEdge, backEdge);
            positions[RBF] = new Vector3(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = new Vector3(rightEdge, bottomEdge, backEdge);
            positions[RTF] = new Vector3(rightEdge, topEdge, frontEdge);
            positions[RTB] = new Vector3(rightEdge, topEdge, backEdge);
        }

        public static void GetFacePositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            const int numPoints = 6;

            var center = bounds.center;
            var extents = bounds.extents;

            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            positions[TOP] = transform.TransformPoint(center + Vector3.up * extents.y);
            positions[BOT] = transform.TransformPoint(center + Vector3.down * extents.y);
            positions[LFT] = transform.TransformPoint(center + Vector3.left * extents.x);
            positions[RHT] = transform.TransformPoint(center + Vector3.right * extents.x);
            positions[FWD] = transform.TransformPoint(center + Vector3.forward * extents.z);
            positions[BCK] = transform.TransformPoint(center + Vector3.back * extents.z);
        }

        /// <summary>
        /// Gets all the corner points and mid points from Bounds
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="transform">A relative transform point.</param>
        /// <param name="positions">A reference <see cref="Vector3"/> array of 8 points to populate.</param>
        public static void GetCornerAndMidPointPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            // Calculate the local points to transform.
            var center = bounds.center;
            var extents = bounds.extents;
            var leftEdge = center.x - extents.x;
            var rightEdge = center.x + extents.x;
            var bottomEdge = center.y - extents.y;
            var topEdge = center.y + extents.y;
            var frontEdge = center.z - extents.z;
            var backEdge = center.z + extents.z;

            // Allocate the array if needed.
            const int numPoints = LTF_LTB + 1;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            // Transform all the local points to world space.
            positions[LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
            positions[LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
            positions[LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
            positions[RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
            positions[RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
            positions[RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);

            positions[LTF_RTF] = Vector3.Lerp(positions[LTF], positions[RTF], 0.5f);
            positions[LBF_RBF] = Vector3.Lerp(positions[LBF], positions[RBF], 0.5f);
            positions[RTB_LTB] = Vector3.Lerp(positions[RTB], positions[LTB], 0.5f);
            positions[RBB_LBB] = Vector3.Lerp(positions[RBB], positions[LBB], 0.5f);

            positions[LTF_LBF] = Vector3.Lerp(positions[LTF], positions[LBF], 0.5f);
            positions[RTB_RBB] = Vector3.Lerp(positions[RTB], positions[RBB], 0.5f);
            positions[LTB_LBB] = Vector3.Lerp(positions[LTB], positions[LBB], 0.5f);
            positions[RTF_RBF] = Vector3.Lerp(positions[RTF], positions[RBF], 0.5f);

            positions[RBF_RBB] = Vector3.Lerp(positions[RBF], positions[RBB], 0.5f);
            positions[RTF_RTB] = Vector3.Lerp(positions[RTF], positions[RTB], 0.5f);
            positions[LBF_LBB] = Vector3.Lerp(positions[LBF], positions[LBB], 0.5f);
            positions[LTF_LTB] = Vector3.Lerp(positions[LTF], positions[LTB], 0.5f);
        }

        /// <summary>
        /// Gets all the corner points and mid points from Bounds, ignoring the z axis
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="transform">A relative transform point.</param>
        /// <param name="positions">A reference <see cref="Vector3"/> array of 8 points to populate.</param>
        /// <param name="flattenAxis">The <see cref="CardinalAxis"/> to flatten the points against.</param>
        public static void GetCornerAndMidPointPositions2D(this Bounds bounds, Transform transform, ref Vector3[] positions, CardinalAxis flattenAxis)
        {
            // Calculate the local points to transform.
            var center = bounds.center;
            var extents = bounds.extents;

            float leftEdge;
            float rightEdge;
            float bottomEdge;
            float topEdge;

            // Allocate the array if needed.
            const int numPoints = LB_LT + 1;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            switch (flattenAxis)
            {
                default:
                case CardinalAxis.X:
                    leftEdge = center.z - extents.z;
                    rightEdge = center.z + extents.z;
                    bottomEdge = center.y - extents.y;
                    topEdge = center.y + extents.y;
                    // Transform all the local points to world space.
                    positions[LT] = transform.TransformPoint(0, topEdge, leftEdge);
                    positions[LB] = transform.TransformPoint(0, bottomEdge, leftEdge);
                    positions[RT] = transform.TransformPoint(0, topEdge, rightEdge);
                    positions[RB] = transform.TransformPoint(0, bottomEdge, rightEdge);
                    break;

                case CardinalAxis.Y:
                    leftEdge = center.z - extents.z;
                    rightEdge = center.z + extents.z;
                    bottomEdge = center.x - extents.x;
                    topEdge = center.x + extents.x;
                    // Transform all the local points to world space.
                    positions[LT] = transform.TransformPoint(topEdge, 0, leftEdge);
                    positions[LB] = transform.TransformPoint(bottomEdge, 0, leftEdge);
                    positions[RT] = transform.TransformPoint(topEdge, 0, rightEdge);
                    positions[RB] = transform.TransformPoint(bottomEdge, 0, rightEdge);
                    break;

                case CardinalAxis.Z:
                    leftEdge = center.x - extents.x;
                    rightEdge = center.x + extents.x;
                    bottomEdge = center.y - extents.y;
                    topEdge = center.y + extents.y;
                    // Transform all the local points to world space.
                    positions[LT] = transform.TransformPoint(leftEdge, topEdge, 0);
                    positions[LB] = transform.TransformPoint(leftEdge, bottomEdge, 0);
                    positions[RT] = transform.TransformPoint(rightEdge, topEdge, 0);
                    positions[RB] = transform.TransformPoint(rightEdge, bottomEdge, 0);
                    break;
            }

            positions[LT_RT] = Vector3.Lerp(positions[LT], positions[RT], 0.5f);
            positions[RT_RB] = Vector3.Lerp(positions[RT], positions[RB], 0.5f);
            positions[RB_LB] = Vector3.Lerp(positions[RB], positions[LB], 0.5f);
            positions[LB_LT] = Vector3.Lerp(positions[LB], positions[LT], 0.5f);
        }

        /// <summary>
        /// Method to get bounds from a collection of points.
        /// </summary>
        /// <param name="points">The points to construct a bounds around.</param>
        /// <param name="bounds">An AABB in world space around all the points.</param>
        /// <returns>True if bounds were calculated, if zero points are present bounds will not be calculated.</returns>
        public static bool GetPointsBounds(List<Vector3> points, out Bounds bounds)
        {
            if (points.Count != 0)
            {
                bounds = new Bounds(points[0], Vector3.zero);

                for (var i = 1; i < points.Count; ++i)
                {
                    bounds.Encapsulate(points[i]);
                }

                return true;
            }

            bounds = new Bounds();
            return false;
        }

        /// <summary>
        /// Method to get bounds using collider method.
        /// </summary>
        /// <param name="target">GameObject to generate the bounds around.</param>
        /// <param name="bounds">An AABB in world space around all the colliders in a gameObject hierarchy.</param>
        /// <param name="ignoreLayers">A LayerMask to restrict the colliders selected.</param>
        /// <returns>True if bounds were calculated, if zero colliders are present bounds will not be calculated.</returns>
        public static bool GetColliderBounds(GameObject target, out Bounds bounds, LayerMask ignoreLayers)
        {
            var boundsPoints = new List<Vector3>();
            GetColliderBoundsPoints(target, boundsPoints, ignoreLayers);

            return GetPointsBounds(boundsPoints, out bounds);
        }

        /// <summary>
        /// Calculates how much scale is required for this Bounds to match another Bounds.
        /// </summary>
        /// <param name="otherBounds">Object representation to be scaled to</param>
        /// <param name="padding">padding multiplied into another bounds</param>
        /// <returns>Scale represented as a Vector3 </returns>
        public static Vector3 GetScaleToMatchBounds(this Bounds bounds, Bounds otherBounds, Vector3 padding = default(Vector3))
        {
            Vector3 szA = otherBounds.size + new Vector3(otherBounds.size.x * padding.x, otherBounds.size.y * padding.y, otherBounds.size.z * padding.z);
            Vector3 szB = bounds.size;
            Assert.IsTrue(szB.x != 0 && szB.y != 0 && szB.z != 0, "The bounds of the object must not be zero.");
            return new Vector3(szA.x / szB.x, szA.y / szB.y, szA.z / szB.z);
        }

        /// <summary>
        /// Calculates how much scale is required for this Bounds to fit inside another bounds without stretching.
        /// </summary>
        /// <param name="containerBounds">The bounds of the container we're trying to fit this object.</param>
        /// <returns>A single scale factor that can be applied to this object to fit inside the container.</returns>
        public static float GetScaleToFitInside(this Bounds bounds, Bounds containerBounds)
        {
            var objectSize = bounds.size;
            var containerSize = containerBounds.size;
            Assert.IsTrue(objectSize.x != 0 && objectSize.y != 0 && objectSize.z != 0, "The bounds of the container must not be zero.");
            return Mathf.Min(containerSize.x / objectSize.x, containerSize.y / objectSize.y, containerSize.z / objectSize.z);
        }

        /// <summary>
        /// Method to get bounding box points using Collider method.
        /// </summary>
        /// <param name="target">gameObject that boundingBox bounds.</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        /// <param name="relativeTo">compute bounds relative to this transform</param>
        public static void GetColliderBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers, Transform relativeTo = null)
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                GetColliderBoundsPoints(colliders[i], boundsPoints, ignoreLayers, relativeTo);
            }
        }

        /// <summary>
        /// Method to get bounding box points using Collider method.
        /// </summary>
        /// <param name="target">gameObject that boundingBox bounds.</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        /// <param name="colliders">The colliders to use for calculating the bounds of this gameObject</param>
        public static void GetColliderBoundsPoints(GameObject target, ref List<Vector3> boundsPoints, LayerMask ignoreLayers, Collider[] colliders = null)
        {
            if (colliders == null)
            {
                colliders = target.GetComponentsInChildren<Collider>();
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                if (ignoreLayers == (1 << colliders[i].gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                switch (colliders[i])
                {
                    case SphereCollider sphereCollider:
                        var sphereBounds = new Bounds(sphereCollider.center, Vector3.one * sphereCollider.radius * 2);
                        sphereBounds.GetFacePositions(sphereCollider.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    case BoxCollider boxCollider:
                        var boxBounds = new Bounds(boxCollider.center, boxCollider.size);
                        boxBounds.GetCornerPositionsWorldSpace(boxCollider.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    case MeshCollider meshCollider:
                        var meshBounds = meshCollider.sharedMesh.bounds;
                        meshBounds.GetCornerPositionsWorldSpace(meshCollider.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;

                    case CapsuleCollider capsuleCollider:
                        var capsuleBounds = new Bounds(capsuleCollider.center, Vector3.zero);
                        var radius = capsuleCollider.radius;

                        switch (capsuleCollider.direction)
                        {
                            case 0:
                                capsuleBounds.size = new Vector3(capsuleCollider.height, capsuleCollider.radius * 2, radius * 2);
                                break;

                            case 1:
                                capsuleBounds.size = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height, capsuleCollider.radius * 2);
                                break;

                            case 2:
                                capsuleBounds.size = new Vector3(capsuleCollider.radius * 2, radius * 2, capsuleCollider.height);
                                break;
                        }

                        capsuleBounds.GetFacePositions(capsuleCollider.transform, ref corners);
                        boundsPoints.AddRange(corners);
                        break;
                }
            }
        }

        /// <summary>
        /// Method to get bounds from a single Collider
        /// </summary>
        /// <param name="collider">Target collider</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        public static void GetColliderBoundsPoints(Collider collider, List<Vector3> boundsPoints, LayerMask ignoreLayers, Transform relativeTo = null)
        {
            if (ignoreLayers == (1 << collider.gameObject.layer | ignoreLayers)) { return; }

            if (collider is SphereCollider)
            {
                SphereCollider sc = collider as SphereCollider;
                Bounds sphereBounds = new Bounds(sc.center, 2 * sc.radius * Vector3.one);
                sphereBounds.GetFacePositions(sc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);
            }
            else if (collider is BoxCollider)
            {
                BoxCollider bc = collider as BoxCollider;
                Bounds boxBounds = new Bounds(bc.center, bc.size);
                boxBounds.GetCornerPositions(bc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);

            }
            else if (collider is MeshCollider)
            {
                MeshCollider mc = collider as MeshCollider;
                Bounds meshBounds = mc.sharedMesh.bounds;
                meshBounds.GetCornerPositions(mc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);
            }
            else if (collider is CapsuleCollider)
            {
                CapsuleCollider cc = collider as CapsuleCollider;
                Bounds capsuleBounds = new Bounds(cc.center, Vector3.zero);
                switch (cc.direction)
                {
                    case CAPSULE_X_AXIS:
                        capsuleBounds.size = new Vector3(cc.height, cc.radius * 2, cc.radius * 2);
                        break;

                    case CAPSULE_Y_AXIS:
                        capsuleBounds.size = new Vector3(cc.radius * 2, cc.height, cc.radius * 2);
                        break;

                    case CAPSULE_Z_AXIS:
                        capsuleBounds.size = new Vector3(cc.radius * 2, cc.radius * 2, cc.height);
                        break;
                }
                capsuleBounds.GetFacePositions(cc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);
            }
        }

        /// <summary>
        /// Method to get bounds from a single Collider
        /// </summary>
        /// <param name="target">gameObject that bounding box bounds</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        /// <param name="renderers">The renderers to use for calculating the bounds of this gameObject</param>
        public static void GetRenderBoundsPoints(GameObject target, ref List<Vector3> boundsPoints, LayerMask ignoreLayers, Renderer[] renderers = null)
        {
            if (renderers == null)
            {
                renderers = target.GetComponentsInChildren<Renderer>();
            }

            for (int i = 0; i < renderers.Length; ++i)
            {
                var rendererObj = renderers[i];

                if (ignoreLayers == (1 << rendererObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                var bounds = rendererObj.transform.GetRenderBounds(ref renderers);

                bounds.GetCornerPositions(ref corners);
                boundsPoints.AddRange(corners);
            }
        }

        /// <summary>
        /// GetMeshFilterBoundsPoints - gets bounding box points using MeshFilter method.
        /// </summary>
        /// <param name="target">gameObject that bounding box bounds.</param>
        /// <param name="boundsPoints">array reference that gets filled with points.</param>
        /// <param name="ignoreLayers">layerMask to simplify search.</param>
        /// <param name="meshFilters">The mesh filters to use for calculating the bounds of this gameObject.</param>
        public static void GetMeshFilterBoundsPoints(GameObject target, ref List<Vector3> boundsPoints, LayerMask ignoreLayers, MeshFilter[] meshFilters = null)
        {
            if (meshFilters == null)
            {
                meshFilters = target.GetComponentsInChildren<MeshFilter>();
            }

            for (int i = 0; i < meshFilters.Length; i++)
            {
                var meshFilterObj = meshFilters[i];

                if (ignoreLayers == (1 << meshFilterObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                var meshBounds = meshFilterObj.sharedMesh.bounds;
                meshBounds.GetCornerPositionsWorldSpace(meshFilterObj.transform, ref corners);
                boundsPoints.AddRange(corners);
            }

            var rectTransforms = target.GetComponentsInChildren<RectTransform>();

            for (int i = 0; i < rectTransforms.Length; i++)
            {
                rectTransforms[i].GetWorldCorners(rectTransformCorners);
                boundsPoints.AddRange(rectTransformCorners);
            }
        }

        /// <summary>
        /// Transforms <see cref="Bounds"/> using the specified transform matrix.
        /// </summary>
        /// <remarks>
        /// Transforming a <see cref="Bounds"/> instance means that the function will construct a new <see cref="Bounds"/> instance which has its center translated using the translation information stored in the specified matrix and its size adjusted to account for rotation and scale.
        /// The size of the new <see cref="Bounds"/> instance will be calculated in such a way that it will contain the old <see cref="Bounds"/>.
        /// </remarks>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="transformMatrix">The specified <see cref="Bounds"/> instance will be transformed using this transform matrix. The function assumes that the matrix does not contain any projection or skew transformation.</param>
        /// <returns>The transformed <see cref="Bounds"/> instance.</returns>
        public static Bounds Transform(this Bounds bounds, Matrix4x4 transformMatrix)
        {
            // We will need access to the right, up and look vector which are encoded inside the transform matrix
            Vector3 rightAxis = transformMatrix.GetColumn(0);
            Vector3 upAxis = transformMatrix.GetColumn(1);
            Vector3 lookAxis = transformMatrix.GetColumn(2);

            // We will 'imagine' that we want to rotate the bounds' extents vector using the rotation information
            // stored inside the specified transform matrix. We will need these when calculating the new size if
            // the transformed bounds.
            var rotatedExtentsRight = rightAxis * bounds.extents.x;
            var rotatedExtentsUp = upAxis * bounds.extents.y;
            var rotatedExtentsLook = lookAxis * bounds.extents.z;

            // Calculate the new bounds size along each axis. The size on each axis is calculated by summing up the 
            // corresponding vector component values of the rotated extents vectors. We multiply by 2 because we want
            // to get a size and currently we are working with extents which represent half the size.
            var newSizeX = (Mathf.Abs(rotatedExtentsRight.x) + Mathf.Abs(rotatedExtentsUp.x) + Mathf.Abs(rotatedExtentsLook.x)) * 2.0f;
            var newSizeY = (Mathf.Abs(rotatedExtentsRight.y) + Mathf.Abs(rotatedExtentsUp.y) + Mathf.Abs(rotatedExtentsLook.y)) * 2.0f;
            var newSizeZ = (Mathf.Abs(rotatedExtentsRight.z) + Mathf.Abs(rotatedExtentsUp.z) + Mathf.Abs(rotatedExtentsLook.z)) * 2.0f;

            // Construct the transformed 'Bounds' instance
            var transformedBounds = new Bounds
            {
                center = transformMatrix.MultiplyPoint(bounds.center),
                size = new Vector3(newSizeX, newSizeY, newSizeZ)
            };

            // Return the instance to the caller
            return transformedBounds;
        }

        /// <summary>
        /// Returns the screen space corner points of the specified 'Bounds' instance.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="camera">The camera used for rendering to the screen. This is needed to perform the transformation to screen space.</param>
        /// <returns>An array of screen points for the camera corners.</returns>
        public static Vector2[] GetScreenSpaceCornerPoints(this Bounds bounds, Camera camera)
        {
            var aabbCenter = bounds.center;
            var aabbExtents = bounds.extents;

            //  Return the screen space point array
            return new Vector2[]
            {
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),

                camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z)),
                camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z))
            };
        }


        /// <summary>
        /// Returns the rectangle which encloses the specifies 'Bounds' instance in screen space.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="camera">The camera volume to calculate bounds from.</param>
        /// <returns>Returns a <see cref="Rect"/> for the encapsulated screen</returns>
        public static Rect GetScreenRectangle(this Bounds bounds, Camera camera)
        {
            // Retrieve the bounds' corner points in screen space
            var screenSpaceCornerPoints = bounds.GetScreenSpaceCornerPoints(camera);

            // Identify the minimum and maximum points in the array
            Vector3 minScreenPoint = screenSpaceCornerPoints[0], maxScreenPoint = screenSpaceCornerPoints[0];

            for (int screenPointIndex = 1; screenPointIndex < screenSpaceCornerPoints.Length; ++screenPointIndex)
            {
                minScreenPoint = Vector3.Min(minScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
                maxScreenPoint = Vector3.Max(maxScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
            }

            // Return the screen space rectangle
            return new Rect(minScreenPoint.x, minScreenPoint.y, maxScreenPoint.x - minScreenPoint.x, maxScreenPoint.y - minScreenPoint.y);
        }

        /// <summary>
        /// Returns the volume of the bounds.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <returns>A <see cref="float"/> representation of the volume.</returns>
        public static float Volume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }

        /// <summary>
        /// Returns bounds that contain both this bounds and the bounds passed in.
        /// </summary>
        /// <param name="originalBounds">The original bounding area.</param>
        /// <param name="otherBounds">The additional bounded area to expand the original bounds with.</param>
        /// <returns>Returns the collective <see cref="Bounds"/> volume.</returns>
        public static Bounds ExpandToContain(this Bounds originalBounds, Bounds otherBounds)
        {
            var tmpBounds = originalBounds;
            tmpBounds.Encapsulate(otherBounds);
            return tmpBounds;
        }

        /// <summary>
        /// Checks to see if bounds contains the other bounds completely.
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="otherBounds">The testing bounding volume for contact.</param>
        /// <returns></returns>
        public static bool ContainsBounds(this Bounds bounds, Bounds otherBounds)
        {
            return bounds.Contains(otherBounds.min) && bounds.Contains(otherBounds.max);
        }

        /// <summary>
        /// Checks to see whether point is closer to bounds or otherBounds
        /// </summary>
        /// <param name="bounds">The input bounding volume.</param>
        /// <param name="point">The point to test against.</param>
        /// <param name="otherBounds">The comparative bounds volume to compare against.</param>
        /// <returns></returns>
        public static bool CloserToPoint(this Bounds bounds, Vector3 point, Bounds otherBounds)
        {
            var distToClosestPoint1 = bounds.ClosestPoint(point) - point;
            var distToClosestPoint2 = otherBounds.ClosestPoint(point) - point;

            if (distToClosestPoint1.magnitude.Equals(distToClosestPoint2.magnitude))
            {
                var toCenter1 = point - bounds.center;
                var toCenter2 = point - otherBounds.center;
                return (toCenter1.magnitude <= toCenter2.magnitude);

            }

            return (distToClosestPoint1.magnitude <= distToClosestPoint2.magnitude);
        }

        /// <summary>
        /// Draws a wire frame <see href="https://docs.unity3d.com/ScriptReference/Bounds.html">Bounds</see> object using <see href="https://docs.unity3d.com/ScriptReference/Debug.DrawLine.html">Debug.DrawLine</see>.
        /// </summary>
        /// <param name="bounds">The <see href="https://docs.unity3d.com/ScriptReference/Bounds.html">Bounds</see> to draw.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="duration">How long the line should be visible for in seconds.</param>
        /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
        public static void DebugDraw(this Bounds bounds, Color color, float duration = 0.0f, bool depthTest = true)
        {
            var center = bounds.center;
            var x = bounds.extents.x;
            var y = bounds.extents.y;
            var z = bounds.extents.z;
            var a = new Vector3(-x, y, -z);
            var b = new Vector3(x, -y, -z);
            var c = new Vector3(x, y, -z);

            var vertices = new Vector3[]
            {
                bounds.min, center + a, center + b, center + c,
                bounds.max, center - a, center - b, center - c
            };

            for (var i = 0; i < boundsEdges.Length; i += 2)
            {
                Debug.DrawLine(vertices[boundsEdges[i]], vertices[boundsEdges[i + 1]], color, duration, depthTest);
            }
        }

        /// <summary>
        /// Calculate the intersection area between the rectangle and another.
        /// </summary>
        // https://forum.unity.com/threads/getting-the-area-rect-of-intersection-between-two-rectangles.299140/
        public static bool Intersects(this Rect thisRect, Rect rect, out Rect area)
        {
            area = new Rect();

            if (rect.Overlaps(thisRect))
            {
                float x1 = Mathf.Min(thisRect.xMax, rect.xMax);
                float x2 = Mathf.Max(thisRect.xMin, rect.xMin);
                float y1 = Mathf.Min(thisRect.yMax, rect.yMax);
                float y2 = Mathf.Max(thisRect.yMin, rect.yMin);
                area.x = Mathf.Min(x1, x2);
                area.y = Mathf.Min(y1, y2);
                area.width = Mathf.Max(0.0f, x1 - x2);
                area.height = Mathf.Max(0.0f, y1 - y2);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Compute Bounds, localized to <paramref name="root"/>, of all children of <paramref name="root"/>, excluding <paramref name="exclude"/>.
        /// </summary>
        /// <remarks>
        /// This is quite expensive, so call sparingly! This traverses the entire hierarchy and does quite a lot of work on each node.
        /// </remarks>
        /// <param name="root">The root transform under which this method will traverse + calculate composite bounds.</param>
        /// <param name="exclude">The transform to exclude from the bounds calculation. Only valid if a direct child of <paramref name="root"/>.</param>
        /// <param name="boundsCalculationMethod">Method to use to calculate bounds.</param>
        /// <param name="containsCanvas">
        /// Set to true if the bounds calculation finds a RectTransform. If true, it is recommended to re-run
        /// this bounds calculation once more after a single frame delay, to make sure the computed layout sizing is taken into account.
        /// </param>
        /// <param name="includeInactiveObjects">Should objects that are currently inactive be included in the bounds calculation?</param>
        /// <param name="abortOnCanvas">Should we early-out if we find a canvas element? Will still set <paramref name="containsCanvas"/> = true.</param>
        public static Bounds CalculateBounds(Transform root, Transform target, Transform exclude, out bool containsCanvas,
                                               BoundsCalculationMethod boundsCalculationMethod = BoundsCalculationMethod.RendererOverCollider,
                                               bool includeInactiveObjects = false,
                                               bool abortOnCanvas = false)
        {
            totalBoundsCorners.Clear();
            childTransforms.Clear();
            containsCanvas = false;

            // Iterate transforms and collect bound volumes
            foreach (Renderer childRenderer in target.GetComponentsInChildren<Renderer>(includeInactiveObjects))
            {
                // Reject if child of exclude 
                if (exclude != null)
                {
                    if (childRenderer.transform.IsChildOf(exclude)) { continue; }
                }


                containsCanvas |= childRenderer.transform is RectTransform;
                if (containsCanvas && abortOnCanvas) { break; }

                ExtractBoundsCorners(childRenderer.transform, boundsCalculationMethod);
            }

            if (totalBoundsCorners.Count == 0)
            {
                return new Bounds();
            }

            Bounds finalBounds = new Bounds(root.InverseTransformPoint(totalBoundsCorners[0]), Vector3.zero);

            for (int i = 1; i < totalBoundsCorners.Count; i++)
            {
                finalBounds.Encapsulate(root.InverseTransformPoint(totalBoundsCorners[i]));
            }

            return finalBounds;
        }

        /// <summary>
        /// Compute the flattening vector for a bounds of size <paramref name="size"/>.
        /// <returns>
        /// Returns a unit vector along the direction of the smallest component of <paramref name="size"/>.
        /// </returns>
        /// <remarks>
        /// Returns Vector3.forward if all components are approximately equal.
        /// </remarks>
        /// <param name="size">The size of the bounds to compute the flatten vector for.</param>
        public static Vector3 CalculateFlattenVector(Vector3 size)
        {
            if (size.x < size.y && size.x < size.z)
            {
                return Vector3.right;
            }
            else if (size.y < size.x && size.y < size.z)
            {
                return Vector3.up;
            }
            else
            {
                return Vector3.forward;
            }
        }

        #endregion

        #region Private Static Functions

        /// <summary>
        /// Returns the vector which is used to represent and invalid bounds size.
        /// </summary>
        private static Vector3 GetInvalidBoundsSize()
        {
            return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }


        /// <summary>
        /// This enum defines what volume type the bound calculation depends on and its priority.
        /// </summary>
        public enum BoundsCalculationMethod
        {
            /// <summary>
            /// Used Renderers for the bounds calculation and Colliders as a fallback
            /// </summary>
            RendererOverCollider = 0,

            /// <summary>
            /// Used Colliders for the bounds calculation and Renderers as a fallback
            /// </summary>
            ColliderOverRenderer,

            /// <summary>
            /// Omits Renderers and uses Colliders for the bounds calculation exclusively
            /// </summary>
            ColliderOnly,

            /// <summary>
            /// Omits Colliders and uses Renderers for the bounds calculation exclusively
            /// </summary>
            RendererOnly,
        }

        // Private scratchpad to reduce allocs.
        private static List<Vector3> totalBoundsCorners = new List<Vector3>(8);

        // Private scratchpad to reduce allocs.
        private static List<Transform> childTransforms = new List<Transform>();

        // Private scratchpad to reduce allocs.
        private static Vector3[] cornersToWorld = new Vector3[8];

        private static void ExtractBoundsCorners(Transform childTransform, BoundsCalculationMethod boundsCalculationMethod)
        {
            KeyValuePair<Transform, Collider> colliderByTransform = default;
            KeyValuePair<Transform, Bounds> rendererBoundsByTransform = default;

            if (boundsCalculationMethod != BoundsCalculationMethod.RendererOnly)
            {
                Collider collider = childTransform.GetComponent<Collider>();
                if (collider != null)
                {
                    colliderByTransform = new KeyValuePair<Transform, Collider>(childTransform, collider);
                }
                else
                {
                    colliderByTransform = new KeyValuePair<Transform, Collider>();
                }
            }

            if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
            {
                MeshFilter meshFilter = childTransform.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>(childTransform, meshFilter.sharedMesh.bounds);
                }
                else if (childTransform is RectTransform rt)
                {
                    rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>(childTransform, new Bounds(rt.rect.center, new Vector3(rt.rect.width, rt.rect.height, 0.1f)));
                }
                else
                {
                    rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>();
                }
            }

            // Encapsulate the collider bounds if criteria match
            if (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly ||
                boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer)
            {
                if (AddColliderBoundsCornersToTarget(colliderByTransform) && boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer ||
                    boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly) { return; }
            }

            // Encapsulate the renderer bounds if criteria match
            if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
            {
                if (AddRendererBoundsCornersToTarget(rendererBoundsByTransform) && boundsCalculationMethod == BoundsCalculationMethod.RendererOverCollider ||
                    boundsCalculationMethod == BoundsCalculationMethod.RendererOnly) { return; }
            }

            // Do the collider for the one case that we chose RendererOverCollider and did not find a renderer
            AddColliderBoundsCornersToTarget(colliderByTransform);
        }

        private static bool AddRendererBoundsCornersToTarget(KeyValuePair<Transform, Bounds> rendererBoundsByTarget)
        {
            if (rendererBoundsByTarget.Key == null) { return false; }

            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            totalBoundsCorners.AddRange(cornersToWorld);
            return true;
        }

        private static bool AddColliderBoundsCornersToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            if (colliderByTransform.Key != null)
            {
                BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, totalBoundsCorners, 0);
            }

            return colliderByTransform.Key != null;
        }

        private static void InverseTransformPoints(ref Vector3[] positions, Transform relativeTo)
        {
            if (relativeTo)
            {
                for (var i = 0; i < positions.Length; ++i)
                {
                    positions[i] = relativeTo.InverseTransformPoint(positions[i]);
                }
            }
        }
        #endregion
    }
}