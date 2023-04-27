// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Networking;

namespace RealityCollective.Utilities.WebRequestRest
{
    public struct RestArgs
    {
        public string DownloadLocation;
        public Dictionary<string, string> Headers;
        public IProgress<float> Progress;
        public int Timeout;
        public CancellationToken CancellationToken;
        public bool ReadResponseData;
        public CertificateHandler CertificateHandler;
        public bool DisposeCertificateHandlerOnDispose;
        public bool ForceDownload;
    }
}
