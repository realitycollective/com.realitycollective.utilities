// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityCollective.Definitions.Utilities
{
    /// <summary>
    /// The AxisType identifies the type of button or input being sent to the framework from a controller.
    /// This is mainly information only or for advanced users to understand the input coming directly from the controller.
    /// </summary>
    public enum AxisType
    {
        /// <summary>
        /// No Specified type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Raw stream from input (proxy only).
        /// </summary>
        Raw,
        /// <summary>
        /// Digital On/Off UnityEngine.Input.
        /// </summary>
        Digital,
        /// <summary>
        /// Single Axis analogue UnityEngine.Input.
        /// </summary>
        SingleAxis,
        /// <summary>
        /// Dual Axis analogue UnityEngine.Input.
        /// </summary>
        DualAxis,
        /// <summary>
        /// Position only Axis analogue UnityEngine.Input.
        /// </summary>
        ThreeDofPosition,
        /// <summary>
        /// Rotation only Axis analogue UnityEngine.Input.
        /// </summary>
        ThreeDofRotation,
        /// <summary>
        /// Position AND Rotation analogue UnityEngine.Input.
        /// </summary>
        SixDof
    }
}