// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using RealityCollective.Utilities.Definitions;
using UnityEngine;

namespace RealityCollective.Utilities.Logging
{
    /// <summary>
    /// Helper function to attach to a persistent gameobject in the loading scece, relays Unity events to the logging entity
    /// </summary>
    public class CaptureApplicationLog : MonoBehaviour
    {
        private void Awake()
        {
            Application.logMessageReceived += Log;
        }

        private void Log(string condition, string stackTrace, LogType logType)
        {
            if (StaticLogger.CurrentLogFilter == FilterLogType.All || ((int)logType) == ((int)StaticLogger.CurrentLogFilter))
            {
                StaticLogger.Log($"{logType}-{condition}\n", logType, true);
            }
        }
    }
}