// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;

namespace RealityCollective.Extensions
{
    /// <summary>
    /// <see cref="HandednessExtensions"/> type method extensions.
    /// </summary>
    public static class HandednessExtensions
    {
        /// <summary>
        /// Gets the opposite "hand" flag for the current Handedness value.
        /// </summary>
        /// <remarks>
        /// If current = Left, returns Right.
        /// If current = Right, returns Left.
        /// Otherwise, returns None
        /// </remarks>
        public static Handedness GetOppositeHandedness(this Handedness inputHandedness)
        {
            switch (inputHandedness)
            {
                case Handedness.Left:
                    return Handedness.Right;
                case Handedness.Right:
                    return Handedness.Left;
                default:
                    return Handedness.None;
            }
        }

        /// <summary>
        /// Returns true if the current Handedness is the Right (i.e == Handedness.Right), false otherwise
        /// </summary>
        public static bool IsRight(this Handedness inputHandedness)
        {
            return inputHandedness == Handedness.Right;
        }

        /// <summary>
        /// Returns true if the current Handedness is the Right (i.e == Handedness.Right), false otherwise
        /// </summary>
        public static bool IsLeft(this Handedness inputHandedness)
        {
            return inputHandedness == Handedness.Left;
        }

        /// <summary>
        /// Returns true if the current Handedness is the Right (i.e == Handedness.Right), false otherwise
        /// </summary>
        public static bool IsNone(this Handedness inputHandedness)
        {
            return inputHandedness == Handedness.None;
        }

        /// <summary>
        /// Returns true if the current Handedness flags are a match with the comparison Handedness flags, false otherwise
        /// </summary>
        public static bool Equals(this Handedness inputHandedness, Handedness comparisonHandedness)
        {
            return (inputHandedness & comparisonHandedness) != 0;
        }
    }
}