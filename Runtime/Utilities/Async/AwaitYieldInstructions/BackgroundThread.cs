// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RealityCollective.Utilities.Async.AwaitYieldInstructions
{
    /// <summary>
    /// Helper class for continuing executions on a background thread.
    /// </summary>
    public class BackgroundThread
    {
        public static ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
        }
    }
}