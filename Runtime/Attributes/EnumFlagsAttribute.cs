// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace RealityCollective.Attributes
{
    /// <summary>
    /// From https://answers.unity.com/questions/486694/default-editor-enum-as-flags-.html
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }
    }
}