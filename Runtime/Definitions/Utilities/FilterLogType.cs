// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

namespace RealityCollective.Definitions.Utilities
{
    public enum FilterLogType
    {
        /// <summary>
        /// LogType used for Errors.
        /// </summary>
        Error,
        /// <summary>
        /// LogType used for Asserts. (These could also indicate an error inside Unity itself.)
        /// </summary>
        Assert,
        /// <summary>
        /// LogType used for Warnings.
        /// </summary>
        Warning,
        /// <summary>
        /// LogType used for regular log messages.
        /// </summary>
        Log,
        /// <summary>
        /// LogType used for Exceptions.
        /// </summary>
        Exception,
        /// <summary>
        /// Do not filter, show all logs
        /// </summary>
        All
    }
}
