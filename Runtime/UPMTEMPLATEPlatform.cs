// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces;

namespace RealityToolkit.UPMTEMPLATE
{
    /// <summary>
    /// Used by the toolkit to signal that a feature is available on the UPMTEMPLATE platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("UPMTEMPLATEGUID")]
    public class UPMTEMPLATEPlatform : BasePlatform
    {
        private const string xrDisplaySubsystemDescriptorId = "UPMTEMPLATE Display";
        private const string xrInputSubsystemDescriptorId = "UPMTEMPLATE Input";

        /// <inheritdoc />
        public override IMixedRealityPlatform[] PlatformOverrides { get; } =
        {
            //new AndroidPlatform()
            //new WindowsStandalonePlatform()
		};
/*		
		Choose a path for Is Platform available at runtime
        /// <inheritdoc />
        public override bool IsAvailable =>
            !Application.isEditor && UPMTEMPLATEApi.Version > NoVersion && UPMTEMPLATEApi.Initialized;		
*/
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
                var displaySubsystems = new List<XRDisplaySubsystem>();
                SubsystemManager.GetSubsystems(displaySubsystems);
                var xrDisplaySubsystemDescriptorFound = false;

                for (var i = 0; i < displaySubsystems.Count; i++)
                {
                    var displaySubsystem = displaySubsystems[i];
                    if (displaySubsystem.SubsystemDescriptor.id.Equals(xrDisplaySubsystemDescriptorId) &&
                        displaySubsystem.running)
                    {
                        xrDisplaySubsystemDescriptorFound = true;
                    }
                }

                // The XR Display Subsystem is not available / running,
                // the platform doesn't seem to be available.
                if (!xrDisplaySubsystemDescriptorFound)
                {
                    return false;
                }

                var inputSubsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetSubsystems(inputSubsystems);
                var xrInputSubsystemDescriptorFound = false;

                for (var i = 0; i < inputSubsystems.Count; i++)
                {
                    var inputSubsystem = inputSubsystems[i];
                    if (inputSubsystem.SubsystemDescriptor.id.Equals(xrInputSubsystemDescriptorId) &&
                        inputSubsystem.running)
                    {
                        xrInputSubsystemDescriptorFound = true;
                    }
                }

                // The XR Input Subsystem is not available / running,
                // the platform doesn't seem to be available.
                if (!xrInputSubsystemDescriptorFound)
                {
                    return false;
                }

                // Only if both, Display and Input XR Subsystems are available
                // and running, the platform is considered available.
                return true;
            }
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override UnityEditor.BuildTarget[] ValidBuildTargets { get; } =
        {
			//Choose which Platforms this runtime is available for in the Editor
            //UnityEditor.BuildTarget.Android
			//UnityEditor.BuildTarget.WSAPlayer
            //UnityEditor.BuildTarget.StandaloneWindows64,
            //UnityEditor.BuildTarget.StandaloneWindows			
        };
#endif
    }
}