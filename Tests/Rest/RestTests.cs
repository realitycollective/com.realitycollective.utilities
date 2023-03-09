using NUnit.Framework;
using RealityCollective.Extensions;
using RealityCollective.Utilities.Async;
using RealityCollective.Utilities.WebRequestRest;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealityCollective.Utilities.Tests.Extensions
{
    public class RestTests
    {
        #region Get Request

        [Test]
        public async void Test_01_01_GetRequest()
        {
            string getURL = "https://httpbin.org/get";

            await Rest.GetAsync(getURL).ContinueWith((response) =>
            {
                Assert.IsNotNull(response.Result, "No result returned from Get request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Get request, result code was [{response.Result.ResponseCode}]");
                Assert.IsNotEmpty(response.Result.ResponseBody, "Returned Get result was empty");
                var resultText = System.Text.Encoding.Default.GetString(response.Result.ResponseData);
                Assert.IsTrue(response.Result.ResponseBody == resultText, "Returned Text does not match Get response data");
            });
        }

        [Test]
        public async void Test_01_02_GetRequestWithHeaders()
        {
            string getURL = "https://httpbin.org/get";
            Dictionary<string, string> Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "application/json");

            await Rest.GetAsync(getURL, new RestArgs() { Headers = Headers }).ContinueWith((response) =>
            {
                Assert.IsNotNull(response.Result, "No result returned from Get request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Get request, result code was [{response.Result.ResponseCode}]");
                Assert.IsNotEmpty(response.Result.ResponseBody, "Returned Get result was empty");
                Assert.IsTrue(response.Result.ResponseBody.Contains("application/json"), "Get header was missing");
            });
        }
        #endregion Get Request

        #region Post Request

        [Test]
        public async void Test_02_01_PostRequest()
        {
            string postURL = "https://httpbin.org/post?data='The Result that was intended'";

            await Rest.PostAsync(postURL).ContinueWith((response) =>
            {
                Assert.IsNotNull(response.Result, "No result returned from Post request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Post request, result code was [{response.Result.ResponseCode}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("The Result that was intended"), "Returned Post string was not as expected");
            });
        }

        [Test]
        public async void Test_02_02_PostRequestWithHeaders()
        {
            string postURL = "https://httpbin.org/post?data='The Result that was intended with headers'";

            Dictionary<string, string> Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "application/json");

            await Rest.PostAsync(postURL, new RestArgs() { Headers = Headers }).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Post request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Post request, result code was [{response.Result.ResponseCode}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("The Result that was intended"), "Returned Post string was not as expected");
                Assert.IsTrue(response.Result.ResponseBody.Contains("application/json"), "Post header was missing");
            });
        }

        [Test]
        public async void Test_02_03_PostFormRequest()
        {
            string postURL = "https://httpbin.org/post";
            WWWForm form = new WWWForm();
            form.AddField("MyForm", "MyData");

            await Rest.PostAsync(postURL, form).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Get request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from request, result code was [{response.Result.ResponseCode}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("MyForm"), "Post MyForm Parameter was missing");
                Assert.IsTrue(response.Result.ResponseBody.Contains("MyData"), "Post MyData value was missing");
            });
        }

        [Serializable]
        struct myJson { public string MyJsonProperty; }

        [Test]
        public async void Test_02_04_PostJSONRequest()
        {
            string postURL = "https://httpbin.org/post";
            var input = new myJson() { MyJsonProperty = "MyJsonValue" };
            var json = JsonUtility.ToJson(input);

            await Rest.PostAsync(postURL, json).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Get request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Post request, result code was [{response.Result.ResponseCode}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("MyJsonProperty"),$"JSON Property was not found in th Post response\n[{response.Result.ResponseBody}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("MyJsonValue"), $"JSON Value was not found in the Post response\n[{response.Result.ResponseBody}]");
            });
        }

        [Test]
        public async void Test_02_05_PostBytesRequest()
        {
            string postURL = "https://httpbin.org/post";
            string postString = "My word in my bond";
            var postBytes = Encoding.UTF8.GetBytes(postString);

            await Rest.PostAsync(postURL, postBytes).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Post request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Post request, result code was [{response.Result.ResponseCode}]");
                Assert.IsNotNull(response.Result.ResponseData, "No data returned from Post request");

                var outputString = Encoding.UTF8.GetString(response.Result.ResponseData);
                Assert.IsTrue(outputString.Contains(postString), "The decoded Post message does not match the input string");
            });
        }
        #endregion Post Request

        #region Put Request

        [Test]
        public async void Test_03_01_PutRequest()
        {
            string putURL = "https://httpbin.org/put?data='The Result that was intended'";
            var input = new myJson() { MyJsonProperty = "MyJsonValue" };
            var json = JsonUtility.ToJson(input);

            await Rest.PutAsync(putURL, json).ContinueWith((response) =>
            {
                Assert.IsNotNull(response.Result, "No result returned from Put request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Put request, result code was [{response.Result.ResponseCode}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("The Result that was intended"), "Returned Put string was not as expected");
            });
        }

        [Test]
        public async void Test_03_02_PutRequestWithHeaders()
        {
            string putURL = "https://httpbin.org/put?data='The Result that was intended with headers'";
            var input = new myJson() { MyJsonProperty = "MyJsonValue" };
            var json = JsonUtility.ToJson(input);

            Dictionary<string, string> Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "application/json");

            await Rest.PutAsync(putURL, json, new RestArgs() { Headers = Headers }).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Put request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Put request, result code was [{response.Result.ResponseCode}]");
                Assert.IsTrue(response.Result.ResponseBody.Contains("The Result that was intended"), "Returned Put string was not as expected");
                Assert.IsTrue(response.Result.ResponseBody.Contains("application/json"), "Put header was missing");
            });
        }

        [Test]
        public async void Test_03_03_PutBytesRequest()
        {
            string putURL = "https://httpbin.org/put";
            string putString = "My word in my bond";
            var postBytes = Encoding.UTF8.GetBytes(putString);

            await Rest.PutAsync(putURL, postBytes).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Get request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Put request, result code was [{response.Result.ResponseCode}]");
                Assert.IsNotNull(response.Result.ResponseData, "No data returned from Put request");

                var outputString = Encoding.UTF8.GetString(response.Result.ResponseData);
                Assert.IsTrue(outputString.Contains(putString), "The decoded Put message does not match the input string");
            });
        }
        #endregion Put Request

        #region Delete Request

        [Test]
        public async void Test_04_01_DeleteRequest()
        {
            string putURL = "https://httpbin.org/delete?data='The Result that was intended'";

            await Rest.DeleteAsync(putURL).ContinueWith((response) =>
            {
                Assert.IsNotNull(response.Result, "No result returned from Delete request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Delete request, result code was [{response.Result.ResponseCode}]");
            });
        }

        [Test]
        public async void Test_04_02_DeleteRequestWithHeaders()
        {
            string putURL = "https://httpbin.org/delete?data='The Result that was intended with headers'";

            Dictionary<string, string> Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "application/json");

            await Rest.DeleteAsync(putURL, new RestArgs() { Headers = Headers }).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Delete request");
                Assert.IsTrue(response.Result.ResponseCode == 200, $"Bad result from Delete request, result code was [{response.Result.ResponseCode}]");
            });
        }
        #endregion Delete Request

        #region Download Handlers

        [Test]
        public async void Test_05_01_DownloadTexture()
        {
            string textureURL = "https://httpbin.org/image/jpeg";

            await Rest.DownloadTextureAsync(textureURL).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from Texture request");
            });
        }

        [Test]
        public async void Test_05_02_DownloadFile()
        {
            string fileURL = "https://httpbin.org/image/jpeg";

            await Rest.DownloadFileAsync(fileURL).ContinueWith((response) =>
            {
                Assert.IsNotNull(response, "No result returned from File request");
            });
        }
        #endregion Download Handlers

    }
}