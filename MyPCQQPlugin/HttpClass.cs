using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#pragma warning disable IDE1006 // 命名样式
namespace MyPCQQPlugin
{
    public static class HttpHelper
    {
        public const string UserAgent = "MyPCQQPlugin/1.0.1 .NET CLR v4.0.30319";
        //public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
        private static HttpWebRequest PrepareHttpRequest(string url, string method, int timeout, string userAgent, string cookie, IDictionary<string, string> headers, IWebProxy proxy)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "*/*";
            request.Method = method;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Proxy = proxy;
            request.Timeout = request.ReadWriteTimeout = (timeout > 0 ? timeout : 10) * 1000;
            request.UserAgent = userAgent;
            if (request.Method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Add("Cookie", cookie);
            }
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    switch (key.ToLower())
                    {
                        case "accept":
                            {
                                request.Accept = headers[key];
                                break;
                            }
                        case "host":
                            {
                                request.Host = headers[key];
                                break;
                            }
                        case "referer":
                            {
                                request.Referer = headers[key];
                                break;
                            }
                        case "content-type":
                            {
                                request.ContentType = headers[key];
                                break;
                            }
                        case "range":
                            {
                                Match m = Regex.Match(headers[key], @"(\S+)=(\d+)-(\d+)");
                                if (m.Success)
                                {
                                    request.AddRange(m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                                }
                                break;
                            }
                        default:
                            {
                                request.Headers.Add(key, headers[key]);
                                break;
                            }
                    }
                }
            }
            if (request.Method.ToUpper() == "POST" && request.ContentType == null)
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            return request;
        }

        public static HttpWebResponse _HttpGet(string url, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "GET", timeout, userAgent, cookie, headers, proxy);
            return (HttpWebResponse)request.GetResponse();
        }
        public static async Task<HttpWebResponse> _HttpGetAsync(string url, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "GET", timeout, userAgent, cookie, headers, proxy);
            return (HttpWebResponse)await request.GetResponseAsync();
        }
        public static string HttpGet(string url, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
            => _HttpGet(url, timeout, userAgent, cookie, headers, proxy).GetResponseString();
        public static async Task<string> HttpGetAsync(string url, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebResponse resp = await _HttpGetAsync(url, timeout, userAgent, cookie, headers, proxy);
            return await resp.GetResponseStringAsync();
        }

        public static HttpWebResponse _HttpPost(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "POST", timeout, userAgent, cookie, headers, proxy);
            if (buffer?.Length > 0)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            return (HttpWebResponse)request.GetResponse();
        }
        public static async Task<HttpWebResponse> _HttpPostAsync(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "POST", timeout, userAgent, cookie, headers, proxy);
            if (buffer?.Length > 0)
            {
                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            return (HttpWebResponse)await request.GetResponseAsync();
        }
        public static string HttpPost(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
            => _HttpPost(url, buffer, timeout, userAgent, cookie, headers, proxy).GetResponseString();
        public static string HttpPost(string url, string formdata, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(formdata);
            return _HttpPost(url, buffer, timeout, userAgent, cookie, headers, proxy).GetResponseString();
        }
        public static string HttpPost(string url, IDictionary<string, object> parameters = null, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            string formdata = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            return HttpPost(url, formdata, timeout, userAgent, cookie, headers, proxy);
        }
        public static async Task<string> HttpPostAsync(string url, string formdata = null, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(formdata);
            HttpWebResponse resp = await _HttpPostAsync(url, buffer, timeout, userAgent, cookie, headers, proxy);
            return await resp.GetResponseStringAsync();
        }
        public static async Task<string> HttpPostAsync(string url, IDictionary<string, object> parameters = null, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            string formdata = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            return await HttpPostAsync(url, formdata, timeout, userAgent, cookie, headers, proxy);
        }

        public static string HttpOption(string url, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "OPTIONS", timeout, userAgent, cookie, headers, proxy);
            return ((HttpWebResponse)request.GetResponse()).GetResponseString();
        }
        public static void HttpDownloadFile(string url, string path, int kb = 0, IWebProxy proxy = null)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = UserAgent;
            request.Proxy = proxy;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                byte[] buffer = new byte[1024];
                int size = responseStream.Read(buffer, 0, buffer.Length);
                int downloadedBytes = 0;
                while (size > 0)
                {
                    stream.Write(buffer, 0, size);
                    size = responseStream.Read(buffer, 0, buffer.Length);
                    downloadedBytes += size;
                    if (downloadedBytes > kb * 1024)
                    {
                        break;
                    }
                }
            }
        }
        public static void HttpDownloadFile(string url, string path, IWebProxy proxy = null)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = UserAgent;
            request.Proxy = proxy;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                responseStream.CopyTo(stream);
            }
        }
        public static byte[] HttpDownloadFile(string url, int timeout = 0, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "*/*";
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Proxy = proxy;
            if (timeout != 0) { request.Timeout = timeout * 1000; request.ReadWriteTimeout = timeout * 1000; }
            else request.ReadWriteTimeout = 10000;
            request.UserAgent = userAgent;
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Add("Cookie", cookie);
            }
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (key.ToLower() == "accept")
                        request.Accept = headers[key];
                    else if (key.ToLower() == "host")
                        request.Host = headers[key];
                    else if (key.ToLower() == "referer")
                        request.Referer = headers[key];
                    else
                        request.Headers.Add(key, headers[key]);
                }
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                responseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static byte[] HttpDownloadFile(string url, int kb = -1, double timeout = -1, string userAgent = UserAgent, string cookie = null, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "*/*";
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Proxy = proxy;
            request.Timeout = request.ReadWriteTimeout = 5000;
            request.UserAgent = userAgent;
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Add("Cookie", cookie);
            }
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (key.ToLower() == "accept")
                        request.Accept = headers[key];
                    else if (key.ToLower() == "host")
                        request.Host = headers[key];
                    else if (key.ToLower() == "referer")
                        request.Referer = headers[key];
                    else
                        request.Headers.Add(key, headers[key]);
                }
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[10240];
                int size = responseStream.Read(buffer, 0, buffer.Length);
                long downloadedBytes = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while (size > 0 && downloadedBytes < (uint)kb * 1024L && sw.Elapsed.TotalSeconds < (uint)timeout)
                {
                    ms.Write(buffer, 0, size);
                    size = responseStream.Read(buffer, 0, buffer.Length);
                    downloadedBytes += size;
                }
                return ms.ToArray();
            }
        }

        public static string GetResponseString(this HttpWebResponse response, Encoding encoding = null)
        {
            using (response)
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        public static async Task<string> GetResponseStringAsync(this HttpWebResponse response, Encoding encoding = null)
        {
            using (response)
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public static byte[] GetResponseBytes(this HttpWebResponse response)
        {
            using (response)
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static async Task<byte[]> GetResponseBytesAsync(this HttpWebResponse response)
        {
            using (response)
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }

    public class HttpSession
    {
        public CookieContainer Cookie { get; private set; } = new CookieContainer();
        public const string UserAgent = "ExRobot.BiliUtils/1.4.9 .NET CLR v4.0.30319";
        public Exception LastException { get; private set; } = null;
        public HttpSession() { }
        #region 构造函数重载
        public HttpSession(Uri url, string cookies)
        {
            foreach (string t in cookies.Split("; ".ToCharArray()))
            {
                var t1 = t.Split('=');
                Cookie.Add(url, new Cookie(t1[0], t1[1]));
            }
        }
        public HttpSession(string url, string cookies)
        {
            foreach (string t in cookies.Split("; ".ToCharArray()))
            {
                var t1 = t.Split('=');
                Cookie.Add(new Uri(url), new Cookie(t1[0], t1[1]));
            }
        }
        public HttpSession(Uri url, CookieCollection cookies)
        {
            Cookie.Add(url, cookies);
        }
        public HttpSession(string url, CookieCollection cookies)
        {
            Cookie.Add(new Uri(url), cookies);
        }
        public HttpSession(CookieContainer cookies)
        {
            Cookie = cookies;
        }
        #endregion
        public HttpWebRequest PrepareHttpRequest(string url, string method, int timeout, string userAgent, IDictionary<string, string> headers, IWebProxy proxy)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "*/*";
            request.Method = method;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Proxy = proxy;
            request.Timeout = request.ReadWriteTimeout = (timeout > 0 ? timeout : 10) * 1000;
            request.UserAgent = userAgent;
            request.CookieContainer = Cookie;
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    switch (key.ToLower())
                    {
                        case "accept":
                            {
                                request.Accept = headers[key];
                                break;
                            }
                        case "host":
                            {
                                request.Host = headers[key];
                                break;
                            }
                        case "referer":
                            {
                                request.Referer = headers[key];
                                break;
                            }
                        case "range":
                            {
                                Match m = Regex.Match(headers[key], @"(\S+)=(\d+)-(\d+)");
                                if (m.Success)
                                {
                                    request.AddRange(m.Groups[1].Value, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                                }
                                break;
                            }
                        default:
                            {
                                request.Headers.Add(key, headers[key]);
                                break;
                            }
                    }
                }
            }
            if (request.Method.ToUpper() == "POST" && request.ContentType == null)
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            return request;
        }

        public HttpWebResponse _HttpGet(string url, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "GET", timeout, userAgent, headers, proxy);
            return (HttpWebResponse)request.GetResponse();
        }
        public async Task<HttpWebResponse> _HttpGetAsync(string url, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "GET", timeout, userAgent, headers, proxy);
            return (HttpWebResponse)await request.GetResponseAsync();
        }
        public string HttpGet(string url, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
            => _HttpGet(url, timeout, userAgent, headers, proxy).GetResponseString();
        public async Task<string> HttpGetAsync(string url, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebResponse resp = await _HttpGetAsync(url, timeout, userAgent, headers, proxy);
            return await resp.GetResponseStringAsync();
        }

        public HttpWebResponse _HttpPost(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
#pragma warning restore IDE1006 // 命名样式
        {
            HttpWebRequest request = PrepareHttpRequest(url, "POST", timeout, userAgent, headers, proxy);
            if (buffer?.Length > 0)
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            return (HttpWebResponse)request.GetResponse();
        }
        public async Task<HttpWebResponse> _HttpPostAsync(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebRequest request = PrepareHttpRequest(url, "POST", timeout, userAgent, headers, proxy);
            if (buffer?.Length > 0)
            {
                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            return (HttpWebResponse)await request.GetResponseAsync();
        }
        public string HttpPost(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
            => _HttpPost(url, buffer, timeout, userAgent, headers, proxy).GetResponseString();
        public string HttpPost(string url, string formdata, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(formdata);
            return _HttpPost(url, buffer, timeout, userAgent, headers, proxy).GetResponseString();
        }
        public string HttpPost(string url, IDictionary<string, object> parameters = null, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            string formdata = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            return HttpPost(url, formdata, timeout, userAgent, headers, proxy);
        }
        public async Task<string> HttpPostAsync(string url, byte[] buffer, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            HttpWebResponse resp = await _HttpPostAsync(url, buffer, timeout, userAgent, headers, proxy);
            return await resp.GetResponseStringAsync();
        }
        public async Task<string> HttpPostAsync(string url, string formdata = null, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(formdata);
            HttpWebResponse resp = await _HttpPostAsync(url, buffer, timeout, userAgent, headers, proxy);
            return await resp.GetResponseStringAsync();
        }
        public async Task<string> HttpPostAsync(string url, IDictionary<string, object> parameters = null, int timeout = 0, string userAgent = UserAgent, IDictionary<string, string> headers = null, IWebProxy proxy = null)
        {
            string formdata = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            return await HttpPostAsync(url, formdata, timeout, userAgent, headers, proxy);
        }
        public string GetCookieString(Uri url)
        {
            string result = "";
            foreach (Cookie c in Cookie.GetCookies(url))
                result += c.Name + "=" + c.Value + "; ";
            return result.Replace("\n", "").TrimEnd("; ".ToCharArray());
        }
        public string GetCookieString(string url)
        {
            return GetCookieString(new Uri(url));
        }
        public void Reset()
        {
            Cookie = new CookieContainer();
            LastException = null;
        }
    }
}