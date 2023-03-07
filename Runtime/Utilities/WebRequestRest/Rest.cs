// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RealityCollective.Utilities.Async;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceProviders;
using RealityCollective.Extensions;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;

namespace RealityCollective.Utilities.WebRequestRest
{
    /// <summary>
    /// REST Class for CRUD Transactions.
    /// </summary>
    public static class Rest
    {
        /// <summary>
        /// Use SSL Connections when making rest calls.
        /// </summary>
        public static bool UseSSL { get; set; } = true;

        #region Authentication

        /// <summary>
        /// Gets the Basic auth header.
        /// </summary>
        /// <param name="username">The Username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The Basic authorization header encoded to base 64.</returns>
        public static string GetBasicAuthentication(string username, string password)
        {
            return $"Basic {Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{username}:{password}"))}";
        }

        /// <summary>
        /// Gets the Bearer auth header.
        /// </summary>
        /// <param name="authToken">OAuth Token to be used.</param>
        /// <returns>The Bearer authorization header.</returns>
        public static string GetBearerOAuthToken(string authToken)
        {
            return $"Bearer {authToken}";
        }

        #endregion Authentication

        #region GET

        /// <summary>
        /// Rest GET.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> GetAsync(string query, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Get(query);
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest GET.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="readResponseData">Optional bool. If its true, response data will be read from web request download handler.</param>
        /// <param name="certificateHandler">Optional certificate handler for custom certificate verification</param>
        /// <param name="disposeCertificateHandlerOnDispose">Optional bool. If true and <paramref name="certificateHandler"/> is not null, <paramref name="certificateHandler"/> will be disposed, when the underlying UnityWebRequest is disposed.</param>
        /// <param name="downloadHandler">Optional DownloadHandler for the request.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> GetAsync(
            string query,
            bool readResponseData,
            CertificateHandler certificateHandler,
            bool disposeCertificateHandlerOnDispose,
            DownloadHandler downloadHandler = null,
            Dictionary<string, string> headers = null,
            IProgress<float> progress = null,
            int timeout = -1,
            CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Get(query);
            if (downloadHandler != null)
            {
                webRequest.downloadHandler = downloadHandler;
            }
            cancellationToken.Register(() =>
            {
                webRequest.Abort();
            });

            return await ProcessRequestAsync(webRequest, certificateHandler, disposeCertificateHandlerOnDispose, headers, progress, timeout, cancellationToken, readResponseData);
        }

        #endregion GET

        #region POST

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, null);
#else
            using var webRequest = UnityWebRequest.Post(query, null as string);
#endif
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="formData">Form Data.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, WWWForm formData, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, formData);
#else
            using var webRequest = UnityWebRequest.Post(query, formData);
#endif
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">JSON data for the request.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, string jsonData, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, "POST");
#else
            using var webRequest = UnityWebRequest.Post(query, "POST");
#endif
            var data = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(data);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">The raw data to post.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, byte[] bodyData, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, "POST");
#else
            using var webRequest = UnityWebRequest.Post(query, "POST");
#endif
            webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="certificateHandler">Optional certificate handler for custom certificate verification</param>
        /// <param name="disposeCertificateHandlerOnDispose">Optional bool. If true and <paramref name="certificateHandler"/> is not null, <paramref name="certificateHandler"/> will be disposed, when the underlying UnityWebRequest is disposed.</param>
        /// <param name="readResponseData">Optional bool. If its true, response data will be read from web request download handler.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(
            string query,
            CertificateHandler certificateHandler,
            bool disposeCertificateHandlerOnDispose,
            bool readResponseData = false,
            Dictionary<string, string> headers = null,
            IProgress<float> progress = null,
            int timeout = -1,
            CancellationToken cancellationToken = default
            )
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, "POST");
#else
            using var webRequest = UnityWebRequest.Post(query, "POST");
#endif

            return await ProcessRequestAsync(webRequest, certificateHandler, disposeCertificateHandlerOnDispose, headers, progress, timeout, cancellationToken, readResponseData);
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="formData">Form Data.</param>
        /// <param name="readResponseData">Optional bool. If its true, response data will be read from web request download handler.</param>
        /// <param name="certificateHandler">Optional certificate handler for custom certificate verification</param>
        /// <param name="disposeCertificateHandlerOnDispose">Optional bool. If true and <paramref name="certificateHandler"/> is not null, <paramref name="certificateHandler"/> will be disposed, when the underlying UnityWebRequest is disposed.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(
            string query,
            WWWForm formData,
            bool readResponseData,
            CertificateHandler certificateHandler,
            bool disposeCertificateHandlerOnDispose,
            Dictionary<string, string> headers = null,
            IProgress<float> progress = null,
            int timeout = -1,
            CancellationToken cancellationToken = default
            )
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, formData);
#else
            using var webRequest = UnityWebRequest.Post(query, formData);
#endif

            return await ProcessRequestAsync(webRequest, certificateHandler, disposeCertificateHandlerOnDispose, headers, progress, timeout, cancellationToken, readResponseData);
        }

        #endregion POST

        #region PUT

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">Data to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, string jsonData, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Put(query, jsonData);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">Data to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, byte[] bodyData, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Put(query, bodyData);
            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">Data to be submitted.</param>
        /// <param name="readResponseData">Optional bool. If its true, response data will be read from web request download handler.</param>
        /// <param name="certificateHandler">Optional certificate handler for custom certificate verification</param>
        /// <param name="disposeCertificateHandlerOnDispose">Optional bool. If true and <paramref name="certificateHandler"/> is not null, <paramref name="certificateHandler"/> will be disposed, when the underlying UnityWebRequest is disposed.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(
            string query,
            byte[] bodyData,
            bool readResponseData,
            CertificateHandler certificateHandler,
            bool disposeCertificateHandlerOnDispose,
            Dictionary<string, string> headers = null,
            IProgress<float> progress = null,
            int timeout = -1,
            CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Put(query, bodyData);
            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");

            return await ProcessRequestAsync(webRequest, certificateHandler, disposeCertificateHandlerOnDispose, headers, progress, timeout, cancellationToken, readResponseData);
        }

        #endregion PUT

        #region DELETE

        /// <summary>
        /// Rest DELETE.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> DeleteAsync(string query, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Delete(query);
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);
        }

        /// <summary>
        /// Rest DELETE.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="readResponseData">Optional bool. If its true, response data will be read from web request download handler.</param>
        /// <param name="certificateHandler">Optional certificate handler for custom certificate verification</param>
        /// <param name="disposeCertificateHandlerOnDispose">Optional bool. If true and <paramref name="certificateHandler"/> is not null, <paramref name="certificateHandler"/> will be disposed, when the underlying UnityWebRequest is disposed.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> DeleteAsync(
            string query,
            bool readResponseData,
            CertificateHandler certificateHandler,
            bool disposeCertificateHandlerOnDispose,
            Dictionary<string, string> headers = null,
            IProgress<float> progress = null,
            int timeout = -1,
            CancellationToken cancellationToken = default)
        {
            using var webRequest = UnityWebRequest.Delete(query);

            return await ProcessRequestAsync(webRequest, certificateHandler, disposeCertificateHandlerOnDispose, headers, progress, timeout, cancellationToken, readResponseData);
        }

        #endregion DELETE

        #region Get Multimedia Content

        #region Download Cache

        private const string DOWNLOAD_CACHE = "download_cache";

        /// <summary>
        /// Generates a <see cref="Guid"/> based on the string.
        /// </summary>
        /// <param name="string">The string to generate the <see cref="Guid"/>.</param>
        /// <returns>A new <see cref="Guid"/> that represents the string.</returns>
        private static Guid GenerateGuid(string @string)
        {
            using MD5 md5 = MD5.Create();
            return new Guid(md5.ComputeHash(Encoding.Default.GetBytes(@string)));
        }

        /// <summary>
        /// The download cache directory.
        /// </summary>
        public static string DownloadCacheDirectory
            => Path.Combine(Application.temporaryCachePath, DOWNLOAD_CACHE);

        /// <summary>
        /// Creates the <see cref="DownloadCacheDirectory"/> if it does not exist.
        /// </summary>
        public static void ValidateCacheDirectory()
        {
            if (!Directory.Exists(DownloadCacheDirectory))
            {
                Directory.CreateDirectory(DownloadCacheDirectory);
            }
        }

        /// <summary>
        /// Try to get a file out of the download cache by uri reference.
        /// </summary>
        /// <param name="uri">The uri key of the item.</param>
        /// <param name="filePath">The file path to the cached item.</param>
        /// <returns>True, if the item was in cache, otherwise false.</returns>
        public static bool TryGetDownloadCacheItem(string uri, out string filePath)
        {
            ValidateCacheDirectory();
            bool exists;

            if (TryGetFileNameFromUrl(uri, out var fileName))
            {
                filePath = Path.Combine(DownloadCacheDirectory, fileName);
                exists = File.Exists(filePath);
            }
            else
            {
                filePath = Path.Combine(DownloadCacheDirectory, GenerateGuid(uri).ToString());
                exists = File.Exists(filePath);
            }

            if (exists)
            {
                filePath = $"file://{Path.GetFullPath(filePath)}";
            }

            return exists;
        }

        /// <summary>
        /// Try to delete the cached item at the uri.
        /// </summary>
        /// <param name="uri">The uri key of the item.</param>
        /// <returns>True, if the cached item was successfully deleted.</returns>
        public static bool TryDeleteCacheItem(string uri)
        {
            if (!TryGetDownloadCacheItem(uri, out var filePath))
            {
                return false;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return !File.Exists(filePath);
        }

        /// <summary>
        /// Deletes all the files in the download cache.
        /// </summary>
        public static void DeleteDownloadCache()
        {
            if (Directory.Exists(DownloadCacheDirectory))
            {
                Directory.Delete(DownloadCacheDirectory, true);
            }
        }

        /// <summary>
        /// Find the filename based on the url.
        /// </summary>
        /// <param name="url">The url to parse to try to guess file name.</param>
        /// <param name="fileName">The filename if found.</param>
        /// <returns>True, if a valid filename is found.</returns>
        private static bool TryGetFileNameFromUrl(string url, out string fileName)
        {
            var baseUrl = url.Split('?')[0];
            var index = baseUrl.LastIndexOf('/') + 1;
            fileName = baseUrl.Substring(index, baseUrl.Length - index);
            return Path.HasExtension(fileName);
        }

        #endregion Download Cache

        /// <summary>
        /// Download a <see cref="Texture2D"/> from the provided <see cref="url"/>.
        /// </summary>
        /// <param name="url">The url to download the <see cref="Texture2D"/> from.</param>
        /// <param name="fileName">Optional, file name to download (including extension).</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A new <see cref="Texture2D"/> instance.</returns>
        public static async Task<Texture2D> DownloadTextureAsync(string url, string fileName = null, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            await Awaiters.UnityMainThread;

            bool isCached;
            string cachePath;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                TryGetFileNameFromUrl(url, out fileName);
            }

            if (url.Contains("file://"))
            {
                isCached = true;
                cachePath = url;
            }
            else
            {
                isCached = TryGetDownloadCacheItem(fileName, out cachePath);
            }

            using var webRequest = UnityWebRequestTexture.GetTexture(url);
            var response = await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);

            if (!response.Successful)
            {
                Debug.LogError(GenerateErrorMessage("texture", url, response));
                return null;
            }

            var downloadHandler = (DownloadHandlerTexture)webRequest.downloadHandler;

            if (!isCached &&
                !File.Exists(cachePath))
            {
                try
                {
                    using var fileStream = File.OpenWrite(cachePath);
                    await fileStream.WriteAsync(downloadHandler.data, 0, downloadHandler.data.Length, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to write texture to disk!\n{e}");
                }
            }

            return downloadHandler.texture;
        }

        /// <summary>
        /// Download a <see cref="AudioClip"/> from the provided <see cref="url"/>.
        /// </summary>
        /// <param name="url">The url to download the <see cref="AudioClip"/> from.</param>
        /// <param name="audioType"><see cref="AudioType"/> to download.</param>
        /// <param name="fileName">Optional, file name to download (including extension).</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A new <see cref="AudioClip"/> instance.</returns>
        public static async Task<AudioClip> DownloadAudioClipAsync(string url, AudioType audioType, string fileName = "", Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            await Awaiters.UnityMainThread;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                TryGetFileNameFromUrl(url, out fileName);
            }

            bool isCached;
            string cachePath;

            if (url.Contains("file://"))
            {
                isCached = true;
                cachePath = url;
            }
            else
            {
                isCached = TryGetDownloadCacheItem(fileName, out cachePath);
            }

            if (isCached)
            {
                url = cachePath;
            }

            using var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            var response = await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);

            if (!response.Successful)
            {
                Debug.LogError(GenerateErrorMessage("audio clip", url, response));
                return null;
            }

            var downloadHandler = (DownloadHandlerAudioClip)webRequest.downloadHandler;

            if (!isCached &&
                !File.Exists(cachePath))
            {
                try
                {
                    using var fileStream = File.OpenWrite(cachePath);
                    await fileStream.WriteAsync(downloadHandler.data, 0, downloadHandler.data.Length, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to write audio asset to disk! {e}");
                }
            }

            return downloadHandler.audioClip;
        }

        /// <summary>
        /// Download a <see cref="AssetBundle"/> from the provided <see cref="url"/>.
        /// </summary>
        /// <param name="url">The url to download the <see cref="AssetBundle"/> from.</param>
        /// <param name="options">Asset bundle request options.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A new <see cref="AssetBundle"/> instance.</returns>
        public static async Task<AssetBundle> DownloadAssetBundleAsync(string url, AssetBundleRequestOptions options, Dictionary<string, string> headers = null, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            await Awaiters.UnityMainThread;

            UnityWebRequest webRequest;

            if (options == null)
            {
                webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            }
            else
            {
                if (!string.IsNullOrEmpty(options.Hash))
                {
                    CachedAssetBundle cachedBundle = new CachedAssetBundle(options.BundleName, Hash128.Parse(options.Hash));
#if ENABLE_CACHING
                    if (options.UseCrcForCachedBundle || !Caching.IsVersionCached(cachedBundle))
                    {
                        webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, cachedBundle, options.Crc);
                    }
                    else
                    {
                        webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, cachedBundle);
                    }
#else
                    webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, cachedBundle, options.Crc);
#endif
                }
                else
                {
                    webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, options.Crc);
                }

                if (options.Timeout > 0)
                {
                    webRequest.timeout = options.Timeout;
                }

                if (options.RedirectLimit > 0)
                {
                    webRequest.redirectLimit = options.RedirectLimit;
                }

#if !UNITY_2019_3_OR_NEWER
                webRequest.chunkedTransfer = options.ChunkedTransfer;
#endif
            }

            using (webRequest)
            {
                Response response;

                try
                {
                    response = await ProcessRequestAsync(webRequest, headers, progress, options?.Timeout ?? -1, cancellationToken);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }

                if (!response.Successful)
                {
                    Debug.LogError(GenerateErrorMessage("asset bundle", url, response));
                    return null;
                }

                var downloadHandler = (DownloadHandlerAssetBundle)webRequest.downloadHandler;
                return downloadHandler.assetBundle;
            }
        }

        /// <summary>
        /// Download a file from the provided <see cref="url"/>.
        /// </summary>
        /// <param name="url">The url to download the file from.</param>
        /// <param name="fileName">Optional file name to download (including extension).</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="progress">Optional <see cref="IProgress{T}"/> handler.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static async Task<string> DownloadFileAsync(string url, string fileName = null, Dictionary<string, string> headers = null, IProgress<float> progress = null, int timeout = -1, CancellationToken cancellationToken = default)
        {
            await Awaiters.UnityMainThread;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                TryGetFileNameFromUrl(url, out fileName);
            }

            if (TryGetDownloadCacheItem(fileName, out var filePath))
            {
                return filePath;
            }

            using var webRequest = UnityWebRequest.Get(url);
            using var fileDownloadHandler = new DownloadHandlerFile(filePath)
            {
                removeFileOnAbort = true
            };

            webRequest.downloadHandler = fileDownloadHandler;
            var response = await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken);

            if (!response.Successful)
            {
                Debug.LogError(GenerateErrorMessage("file", url, response));
                return null;
            }

            return filePath;
        }

        #endregion Get Multimedia Content

        #region Private Functions

        private static async Task<Response> ProcessRequestAsync(UnityWebRequest webRequest, Dictionary<string, string> headers, IProgress<float> progress, int timeout, CancellationToken cancellationToken, bool readResponseData = false)
        {
            await Awaiters.UnityMainThread;

            if (timeout > 0)
            {
                webRequest.timeout = timeout;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            var isUpload = webRequest.method == UnityWebRequest.kHttpVerbPOST ||
                           webRequest.method == UnityWebRequest.kHttpVerbPUT;

            if (isUpload)
            {
                var contentType = webRequest.GetRequestHeader("Content-Type");

                if (contentType != null)
                {
                    contentType = contentType.Replace("\"", "");
                    webRequest.SetRequestHeader("Content-Type", contentType);
                }
            }

            webRequest.disposeCertificateHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeUploadHandlerOnDispose = true;

            Thread backgroundThread = null;

            if (progress != null)
            {
                async void ProgressReportingThread()
                {
                    try
                    {
                        await Awaiters.UnityMainThread;

                        while (!webRequest.isDone)
                        {
                            progress.Report(isUpload ? webRequest.uploadProgress : webRequest.downloadProgress * 100f);

                            if (cancellationToken.IsCancellationRequested)
                            {
                                webRequest.Abort();
                            }

                            await Awaiters.UnityMainThread;
                        }
                    }
                    catch (Exception)
                    {
                        // Throw away
                    }
                }

                backgroundThread = new Thread(ProgressReportingThread)
                {
                    IsBackground = true
                };
            }

            backgroundThread?.Start();

            try
            {
                await webRequest.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Rest)}.{nameof(ProcessRequestAsync)}::Send Web Request Failed! {e}");
            }

            backgroundThread?.Join();
            progress?.Report(100f);

            if (webRequest.result ==
                UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                if (webRequest.responseCode == 401)
                {
                    return new Response(false, "Invalid Credentials", null, webRequest.responseCode);
                }

                if (webRequest.GetResponseHeaders() == null)
                {
                    return new Response(false, "Invalid Headers", null, webRequest.responseCode);
                }

                var responseHeaders = webRequest.GetResponseHeaders().Aggregate(string.Empty, (_, header) => $"\n{header.Key}: {header.Value}");
                Debug.LogError($"REST Error {webRequest.responseCode}:{webRequest.downloadHandler?.text}{responseHeaders}");
                return new Response(false, $"{responseHeaders}\n{webRequest.downloadHandler?.text}", null, webRequest.responseCode);
            }

            if (!string.IsNullOrEmpty(webRequest.downloadHandler.error))
            {
                return new Response(false, webRequest.downloadHandler.error, webRequest.downloadHandler.data, webRequest.responseCode);
            }

            if (readResponseData)
            {
                return new Response(true, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
            }

            switch (webRequest.downloadHandler)
            {
                case DownloadHandlerFile _:
                case DownloadHandlerScript _:
                case DownloadHandlerTexture _:
                case DownloadHandlerAudioClip _:
                case DownloadHandlerAssetBundle _:
                    return new Response(true, null, null, webRequest.responseCode);
                case DownloadHandlerBuffer _:
                    return new Response(true, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
                default:
                    return new Response(true, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
            }
        }

        private static async Task<Response> ProcessRequestAsync(UnityWebRequest webRequest, CertificateHandler certificateHandler, bool disposeCertificateHandlerOnDispose, Dictionary<string, string> headers, IProgress<float> progress, int timeout, CancellationToken cancellationToken, bool readResponseData = false)
        {
            webRequest.certificateHandler = certificateHandler;
            webRequest.disposeCertificateHandlerOnDispose = disposeCertificateHandlerOnDispose;
            return await ProcessRequestAsync(webRequest, headers, progress, timeout, cancellationToken, readResponseData);
        }

        private static string GenerateErrorMessage(string typeName, string url, Response response)
        {
            return $"Failed to download {typeName} from \"{url}\"!\n{response.ResponseCode}:{response.ResponseBody}";
        }
        #endregion Private Functions
    }
}