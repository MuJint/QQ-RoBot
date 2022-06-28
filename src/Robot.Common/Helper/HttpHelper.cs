using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Common
{
    public class HttpHelper
    {
        /// <summary>
        /// 同步GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <returns></returns>
        public static string HttpGet(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using HttpClient client = new();
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            if (timeout > 0)
            {
                client.Timeout = new TimeSpan(0, 0, timeout);
            }
            Byte[] resultBytes = client.GetByteArrayAsync(url).Result;
            return Encoding.UTF8.GetString(resultBytes);
        }

        /// <summary>
        /// 异步GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url, Dictionary<string, string> headers = null, int timeout = 15)
        {
            using HttpClient client = new();
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            if (timeout > 0)
            {
                client.Timeout = new TimeSpan(0, 0, timeout);
            }
            Byte[] resultBytes = await client.GetByteArrayAsync(url);
            return Encoding.Default.GetString(resultBytes);
        }


        /// <summary>
        /// 同步POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <param name="encoding">默认UTF8</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using HttpClient client = new();
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            if (timeout > 0)
            {
                client.Timeout = new TimeSpan(0, 0, timeout);
            }
            using HttpContent content = new StringContent(postData ?? "", encoding ?? Encoding.UTF8);
            if (contentType != null)
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }
            using HttpResponseMessage responseMessage = client.PostAsync(url, content).Result;
            Byte[] resultBytes = responseMessage.Content.ReadAsByteArrayAsync().Result;
            return Encoding.UTF8.GetString(resultBytes);
        }

        /// <summary>
        /// 异步POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout">请求响应超时时间，单位/s(默认100秒)</param>
        /// <param name="encoding">默认UTF8</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            using HttpClient client = new();
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            if (timeout > 0)
            {
                client.Timeout = new TimeSpan(0, 0, timeout);
            }
            using HttpContent content = new StringContent(postData ?? "", encoding ?? Encoding.UTF8);
            if (contentType != null)
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            }
            using HttpResponseMessage responseMessage = await client.PostAsync(url, content);
            Byte[] resultBytes = await responseMessage.Content.ReadAsByteArrayAsync();
            return Encoding.UTF8.GetString(resultBytes);
        }
    }
}
