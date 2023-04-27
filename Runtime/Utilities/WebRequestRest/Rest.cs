// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Utilities.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace RealityCollective.Utilities.WebRequestRest
{
    /// <summary>
    /// REST Class for CRUD Transactions.
    /// </summary>
    public static class Rest
    {
        #region Global Properties
        public static string DownloadLocation = Application.temporaryCachePath;
        public static Dictionary<string, string> Headers = new Dictionary<string, string>();
        public static IProgress<float> Progress = null;
        public static int Timeout = 0;
        public static CancellationToken CancellationToken;
        public static bool ReadResponseData = false;
        public static CertificateHandler CertificateHandler = null;
        public static bool DisposeCertificateHandlerOnDispose = true;
        public static bool ForceDownload = false;
        #endregion

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
        /// <param name="getArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> GetAsync(string query, RestArgs getArgs = default)
        {
            using var webRequest = UnityWebRequest.Get(query);
            try
            {
                return await ProcessRequestAsync(webRequest, getArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Get", query, ex));
                return Response.Failure;
            }
        }

        #endregion GET

        #region POST

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="postArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, RestArgs postArgs = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, null);
#else
            using var webRequest = UnityWebRequest.Post(query, null as string);
#endif
            try
            {
                return await ProcessRequestAsync(webRequest, postArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Post", query, ex));
                return Response.Failure;
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="formData">Form Data.</param>
        /// <param name="postArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, WWWForm formData, RestArgs postArgs = default)
        {

            using var webRequest = UnityWebRequest.Post(query, formData);
            try
            {
                return await ProcessRequestAsync(webRequest, postArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Post form", query, ex));
                return Response.Failure;
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">JSON data for the request.</param>
        /// <param name="postArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, string jsonData, RestArgs postArgs = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, UnityWebRequest.kHttpVerbPOST);
#else
            using var webRequest = UnityWebRequest.Post(query, UnityWebRequest.kHttpVerbPOST);
#endif
            var data = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(data);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            try
            {
                return await ProcessRequestAsync(webRequest, postArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Post json", query, ex));
                return Response.Failure;
            }
        }

        /// <summary>
        /// Rest POST.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">The raw data to post.</param>
        /// <param name="postArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PostAsync(string query, byte[] bodyData, RestArgs postArgs = default)
        {
#if UNITY_2022_2_OR_NEWER
            using var webRequest = UnityWebRequest.PostWwwForm(query, UnityWebRequest.kHttpVerbPOST);
#else
            using var webRequest = UnityWebRequest.Post(query, UnityWebRequest.kHttpVerbPOST);
#endif
            webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            try
            {
                return await ProcessRequestAsync(webRequest, postArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Post Bytes", query, ex));
                return Response.Failure;
            }
        }

        #endregion POST

        #region PUT

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">Data to be submitted.</param>
        /// <param name="putArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, string jsonData, RestArgs putArgs = default)
        {
            using var webRequest = UnityWebRequest.Put(query, jsonData);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            try
            {
                return await ProcessRequestAsync(webRequest, putArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Put json", query, ex));
                return Response.Failure;
            }
        }

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="bodyData">Data to be submitted.</param>
        /// <param name="putArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PutAsync(string query, byte[] bodyData, RestArgs putArgs = default)
        {
            using var webRequest = UnityWebRequest.Put(query, bodyData);
            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            try
            {
                return await ProcessRequestAsync(webRequest, putArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Put bytes", query, ex));
                return Response.Failure;
            }
        }

        #endregion PUT

        #region DELETE

        /// <summary>
        /// Rest DELETE.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="deleteArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> DeleteAsync(string query, RestArgs deleteArgs = default)
        {
            using var webRequest = UnityWebRequest.Delete(query);
            try
            {
                return await ProcessRequestAsync(webRequest, deleteArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorRequest("Delete", query, ex));
                return Response.Failure;
            }
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
        /// The download cache directory.<br/>
        /// </summary>
        public static string DownloadCacheDirectory
            => Path.Combine(Application.temporaryCachePath, DOWNLOAD_CACHE);

        /// <summary>
        /// Creates the <see cref="DownloadCacheDirectory"/> if it does not exist.
        /// </summary>
        public static void ValidateCacheDirectory(string DownloadDirectory)
        {
            if (!Directory.Exists(DownloadDirectory))
            {
                Directory.CreateDirectory(DownloadDirectory);
            }
        }

        /// <summary>
        /// Try to get a file out of the download cache by uri reference.
        /// </summary>
        /// <param name="uri">The uri key of the item.</param>
        /// <param name="filePath">The file path to the cached item.</param>
        /// <returns>True, if the item was in cache, otherwise false.</returns>
        public static bool TryGetDownloadCacheItem(string uri, out string filePath, string downloadDirectory = "", bool forceDownload = false)
        {
            var downloadLocation = string.IsNullOrEmpty(downloadDirectory) ? DownloadCacheDirectory : downloadDirectory;
            ValidateCacheDirectory(downloadLocation);
            bool exists;

            if (TryGetFileNameFromUrl(uri, out var fileName))
            {
                filePath = Path.Combine(downloadLocation, fileName);
                exists = File.Exists(filePath);
            }
            else
            {
                filePath = Path.Combine(downloadLocation, GenerateGuid(uri).ToString());
                exists = File.Exists(filePath);
            }

            if (exists)
            {
                if (forceDownload)
                {
                    return !TryDeleteCacheItem(uri, downloadDirectory);
                }
#if UNITY_STANDALONE || UNITY_WSA || UNITY_EDITOR_WIN
                filePath = $"{Path.GetFullPath(filePath)}";
#else
                filePath = $"file://{Path.GetFullPath(filePath)}";
#endif
            }

            return exists;
        }

        /// <summary>
        /// Try to delete the cached item at the uri.
        /// </summary>
        /// <param name="uri">The uri key of the item.</param>
        /// <returns>True, if the cached item was successfully deleted.</returns>
        public static bool TryDeleteCacheItem(string uri, string downloadDirectory = "")
        {
            if (!TryGetDownloadCacheItem(uri, out var filePath, downloadDirectory))
            {
                return false;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError(GenerateErrorRequest("Delete file", uri, e));
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
        /// <param name="downloadTextureArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>A new <see cref="Texture2D"/> instance.</returns>
        public static async Task<Texture2D> DownloadTextureAsync(string url, string fileName = null, RestArgs downloadTextureArgs = default)
        {
            await Awaiters.UnityMainThread;
            var downloadLocation = string.IsNullOrEmpty(downloadTextureArgs.DownloadLocation) ? DownloadLocation : downloadTextureArgs.DownloadLocation;

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
                isCached = TryGetDownloadCacheItem(fileName, out cachePath, downloadLocation, ForceDownload || downloadTextureArgs.ForceDownload);
            }

            using var webRequest = UnityWebRequestTexture.GetTexture(url);
            var response = await ProcessRequestAsync(webRequest, downloadTextureArgs);

            if (!response.Successful)
            {
                Debug.LogError(GenerateErrorMessage("texture", url, response));
                return null;
            }

            var downloadHandler = (DownloadHandlerTexture)webRequest.downloadHandler;

            if (!isCached && !File.Exists(cachePath))
            {
                try
                {
                    using var fileStream = File.OpenWrite(cachePath);
                    await fileStream.WriteAsync(downloadHandler.data, 0, downloadHandler.data.Length, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Debug.LogError(GenerateErrorMessage("texture", url, e));
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
        /// <param name="downloadAudioClipArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>A new <see cref="AudioClip"/> instance.</returns>
        public static async Task<AudioClip> DownloadAudioClipAsync(string url, AudioType audioType, string fileName = "", RestArgs downloadAudioClipArgs = default)
        {
            await Awaiters.UnityMainThread;
            var downloadLocation = string.IsNullOrEmpty(downloadAudioClipArgs.DownloadLocation) ? DownloadLocation : downloadAudioClipArgs.DownloadLocation;

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
                isCached = TryGetDownloadCacheItem(fileName, out cachePath, downloadLocation, ForceDownload || downloadAudioClipArgs.ForceDownload);
            }

            if (isCached)
            {
                url = cachePath;
            }

            using var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            var response = await ProcessRequestAsync(webRequest, downloadAudioClipArgs);

            if (!response.Successful)
            {
                Debug.LogError(GenerateErrorMessage("audio clip", url, response));
                return null;
            }

            var downloadHandler = (DownloadHandlerAudioClip)webRequest.downloadHandler;

            if (!isCached && !File.Exists(cachePath))
            {
                try
                {
                    using var fileStream = File.OpenWrite(cachePath);
                    await fileStream.WriteAsync(downloadHandler.data, 0, downloadHandler.data.Length, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Debug.LogError(GenerateErrorMessage("audio clip", url, e));
                }
            }

            return downloadHandler.audioClip;
        }

        /// <summary>
        /// Download a <see cref="AssetBundle"/> from the provided <see cref="url"/>.
        /// </summary>
        /// <param name="url">The url to download the <see cref="AssetBundle"/> from.</param>
        /// <param name="options">Asset bundle request options.</param>
        /// <param name="downloadAssetBundleArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>A new <see cref="AssetBundle"/> instance.</returns>
        public static async Task<AssetBundle> DownloadAssetBundleAsync(string url, AssetBundleRequestOptions options, RestArgs downloadAssetBundleArgs = default)
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
                    if (downloadAssetBundleArgs.Timeout == 0 && options?.Timeout > 0)
                    {
                        downloadAssetBundleArgs.Timeout = options.Timeout;
                    }
                    response = await ProcessRequestAsync(webRequest, downloadAssetBundleArgs);
                }
                catch (Exception e)
                {
                    Debug.LogError(GenerateErrorRequest("Asset bundle download", url, e));
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
        /// <param name="downloadFileArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The path to the downloaded file.</returns>
        public static async Task<string> DownloadFileAsync(string url, string fileName = null, RestArgs downloadFileArgs = default)
        {
            await Awaiters.UnityMainThread;
            var downloadLocation = string.IsNullOrEmpty(downloadFileArgs.DownloadLocation) ? DownloadLocation : downloadFileArgs.DownloadLocation;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                TryGetFileNameFromUrl(url, out fileName);
            }

            if (TryGetDownloadCacheItem(fileName, out var filePath, downloadLocation, ForceDownload || downloadFileArgs.ForceDownload))
            {
                return filePath;
            }

            using var webRequest = UnityWebRequest.Get(url);
            using var fileDownloadHandler = new DownloadHandlerFile(filePath)
            {
                removeFileOnAbort = true
            };

            webRequest.downloadHandler = fileDownloadHandler;
            try
            {
                var response = await ProcessRequestAsync(webRequest, downloadFileArgs);
                if (!response.Successful)
                {
                    Debug.LogError(GenerateErrorMessage("file", url, response));
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(GenerateErrorMessage("file", url, ex));
                return null;
            }

            return filePath;
        }

        /// <summary>
        /// Download a file from the provided <see cref="url"/> and return its contents as a <see cref="byte[]"/>.
        /// </summary>
        /// <param name="url">The url to download the file from.</param>
        /// <param name="fileName">Optional file name to download (including extension).</param>
        /// <param name="downloadFileArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The path to the downloaded file.</returns>
        /// <remarks>Intended for use where you need the raw contents of the target file</remarks>
        public static async Task<byte[]> DownloadFileBytesAsync(string url, string fileName = null, RestArgs downloadFileArgs = default)
        {
            byte[] bytes = null;
            await Awaiters.UnityMainThread;
            var downloadLocation = string.IsNullOrEmpty(downloadFileArgs.DownloadLocation) ? DownloadLocation : downloadFileArgs.DownloadLocation;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                TryGetFileNameFromUrl(url, out fileName);
            }

            if (!TryGetDownloadCacheItem(fileName, out var filePath, downloadLocation))
            {
                filePath = await DownloadFileAsync(url, fileName, downloadFileArgs);
            }

            if (File.Exists(filePath))
            {
                try
                {
                    bytes = File.ReadAllBytes(filePath);
                }
                catch (Exception ex) 
                {
                    Debug.LogError(GenerateErrorMessage("File as Bytes", url, ex));
                }
            }

            return bytes;
        }

        /// <summary>
        /// Download a file from the provided <see cref="url"/>and return its contents as a <see cref="string"/>.
        /// </summary>
        /// <param name="url">The url to download the file from.</param>
        /// <param name="fileName">Optional file name to download (including extension).</param>
        /// <param name="downloadFileArgs">Optional additional argumens for the Rest request, <see cref="RestArgs"/>.</param>
        /// <returns>The path to the downloaded file.</returns>
        /// <remarks>Intended for use in configuration/json files.</remarks>
        public static async Task<string> DownloadFileStringAsync(string url, string fileName = null, RestArgs downloadFileArgs = default)
        {
            string fileContents = null;
            await Awaiters.UnityMainThread;
            var downloadLocation = string.IsNullOrEmpty(downloadFileArgs.DownloadLocation) ? DownloadLocation : downloadFileArgs.DownloadLocation;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                TryGetFileNameFromUrl(url, out fileName);
            }

            if (!TryGetDownloadCacheItem(fileName, out string filePath, downloadLocation))
            {
                filePath = await DownloadFileAsync(url, fileName, downloadFileArgs);
            }

            if (File.Exists(filePath))
            {
                try
                {
                    using var sr = new StreamReader(filePath);
                    fileContents = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    Debug.LogError(GenerateErrorMessage("File as String", url, ex));
                }
            }

            return fileContents;
        }

#endregion Get Multimedia Content

        #region Private Functions

        private static async Task<Response> ProcessRequestAsync(UnityWebRequest webRequest, RestArgs processArgs)
        {
            await Awaiters.UnityMainThread;

            var timeout = Timeout + processArgs.Timeout;
            if (timeout > 0)
            {
                webRequest.timeout = timeout;
            }

            // Use defaults
            if (Headers.Count > 0)
            {
                foreach (var header in Headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            // Apply overrides if supplied
            if (processArgs.Headers != null)
            {
                foreach (var header in processArgs.Headers)
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

            if (Progress != null || processArgs.Progress != null)
            {
                async void ProgressReportingThread()
                {
                    try
                    {
                        await Awaiters.UnityMainThread;

                        while (!webRequest.isDone)
                        {
                            // Update default progress if set
                            Progress?.Report(isUpload ? webRequest.uploadProgress : webRequest.downloadProgress * 100f);

                            processArgs.Progress?.Report(isUpload ? webRequest.uploadProgress : webRequest.downloadProgress * 100f);

                            if (CancellationToken.IsCancellationRequested || processArgs.CancellationToken.IsCancellationRequested)
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
                // Use defaults
                if (CertificateHandler != null)
                {
                    webRequest.certificateHandler = CertificateHandler;
                    webRequest.disposeCertificateHandlerOnDispose = DisposeCertificateHandlerOnDispose;
                }
                // Apply overrides if supplied
                if (processArgs.CertificateHandler != null)
                {
                    webRequest.certificateHandler = processArgs.CertificateHandler;
                    webRequest.disposeCertificateHandlerOnDispose = processArgs.DisposeCertificateHandlerOnDispose;
                }
                await webRequest.SendWebRequest();
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Rest)}.{nameof(ProcessRequestAsync)}::Send Web Request Failed! {e}");
            }

            backgroundThread?.Join();
            Progress?.Report(100f);
            processArgs.Progress?.Report(100f);

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

            if (!string.IsNullOrEmpty(webRequest.downloadHandler?.error))
            {
                return new Response(false, webRequest.downloadHandler.error, webRequest.downloadHandler.data, webRequest.responseCode);
            }

            if (ReadResponseData || processArgs.ReadResponseData)
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

        private static string GenerateErrorMessage(string typeName, string url, Response response)
        {
            return $"Failed to download {typeName} from \"{url}\"!\n{response.ResponseCode}:{response.ResponseBody}";
        }

        private static string GenerateErrorMessage(string typeName, string url, Exception exception)
        {
            return $"Failed to download {typeName} from \"{url}\"!\n{exception.Message}\n{exception.StackTrace}";
        }

        private static string GenerateErrorRequest(string requestName, string url, Exception exception)
        {
            return $"A failure occured in a {requestName} call targetting \"{url}\"!\n{exception.Message}\n{exception.StackTrace}";
        }
        #endregion Private Functions
    }
}