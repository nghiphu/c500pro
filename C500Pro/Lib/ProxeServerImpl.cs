using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace C500Pro.Lib
{
    // Install-Package Titanium.Web.Proxy -Pre
    // https://github.com/justcoding121/titanium-web-proxy
    internal class ProxeServerImpl
    {
        /// <summary>
        /// Danh sách domain được phép. Nếu danh sách rỗng sẽ chặn tất cả
        /// </summary>
        public List<string> WhiteListDomain { get; set; }

        /// <summary>
        /// Danh sách domain được phép. Nếu danh sách rỗng sẽ chặn tất cả
        /// </summary>
        public List<string> BlackListDomain { get; set; }

        ProxyServer proxyServer;
        int _proxyPort;

        /// <summary>
        /// Khởi tạo đối tượng implement proxy server của Titanium.Web.Proxy
        /// </summary>
        /// <param name="proxyPort"></param>
        public ProxeServerImpl(int proxyPort)
        {
            _proxyPort = proxyPort;
            WhiteListDomain = new List<string>();
            BlackListDomain = new List<string>();
        }

        /// <summary>
        /// Khởi động proxy server
        /// </summary>
        public void StartServer()
        {
            proxyServer = new ProxyServer();
            // proxyServer.CertificateManager.TrustRootCertificate(true);

            proxyServer.BeforeRequest += OnRequest;
            proxyServer.BeforeResponse += OnResponse;
            proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;

            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, _proxyPort, true);

            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            foreach (var endPoint in proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            // Only explicit proxies can be set as system proxy!
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
        }

        /// <summary>
        /// Tắt proxy server
        /// </summary>
        public void StopServer()
        {
            if (proxyServer != null)
            {
                proxyServer.BeforeRequest -= OnRequest;
                proxyServer.BeforeResponse -= OnResponse;
                proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

                proxyServer.Stop();
            }
        }

        #region Các sự kiện xử lý request

        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;

            if (hostname.Contains("dropbox.com"))
            {
                // Exclude Https addresses you don't want to proxy
                // Useful for clients that use certificate pinning
                // for example dropbox.com
                e.DecryptSsl = false;
                await Task.Delay(0);// để code đỡ warning
            }
        }

        private async Task OnRequest(object sender, SessionEventArgs e)
        {
            // Console.WriteLine(e.HttpClient.Request.Url);

            // read request headers
            var requestHeaders = e.HttpClient.Request.Headers;

            var method = e.HttpClient.Request.Method.ToUpper();
            if ((method == "POST" || method == "PUT" || method == "PATCH"))
            {
                // Get/Set request body bytes
                byte[] bodyBytes = await e.GetRequestBody();
                e.SetRequestBody(bodyBytes);

                // Get/Set request body as string
                string bodyString = await e.GetRequestBodyAsString();
                e.SetRequestBodyString(bodyString);

                // store request 
                // so that you can find it from response handler 
                e.UserData = e.HttpClient.Request;
            }

            // To cancel a request with a custom HTML content
            // Filter URL
            string url = e.HttpClient.Request.RequestUri.AbsoluteUri.ToLower();
            if (this.WhiteListDomain.FirstOrDefault(p => url.Contains(p)) == null || this.BlackListDomain.FirstOrDefault(p => url.Contains(p)) != null)
            {
                C500.WriteToBinaryFile<string>($"Band {e.HttpClient.Request.Url}\r\n", true);
                e.Ok("<!DOCTYPE html>" +
                    "<html><head<meta charset=\"UTF-8\"</head><body><h1>" +
                    "Trang nay da bi chan" +
                    "</h1>" +
                    "<p>Chan boi phu huynh roi chau oi ^^</p>" +
                    "</body>" +
                    "</html>");
                // Hoặc có thể dùng redirect sang url khác
                // e.Redirect("https://www.paypal.com");
            }

            C500.WriteToBinaryFile<string>($"Allow {e.HttpClient.Request.Url}\r\n", true);
        }

        // Modify response
        private static async Task OnResponse(object sender, SessionEventArgs e)
        {
            // read response headers
            var responseHeaders = e.HttpClient.Response.Headers;

            //if (!e.ProxySession.Request.Host.Equals("medeczane.sgk.gov.tr")) return;
            if (e.HttpClient.Request.Method == "GET" || e.HttpClient.Request.Method == "POST")
            {
                if (e.HttpClient.Response.StatusCode == 200)
                {
                    if (e.HttpClient.Response.ContentType != null && e.HttpClient.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        byte[] bodyBytes = await e.GetResponseBody();
                        e.SetResponseBody(bodyBytes);

                        string body = await e.GetResponseBodyAsString();
                        e.SetResponseBodyString(body);
                    }
                }
            }

            if (e.UserData != null)
            {
                // access request from UserData property where we stored it in RequestHandler
                var request = (Request)e.UserData;
            }
        }

        // Allows overriding default certificate validation logic
        private async Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;
            if (sender is int)
                await Task.Delay(0); // để code đỡ warning
            //return Task.CompletedTask;
        }

        // Allows overriding default client certificate selection logic during mutual authentication
        private async Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            if (sender is int)
                await Task.Delay(0); // để code đỡ warning

            // set e.clientCertificate to override
            //return Task.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Nhận diện các đường dẫn liên quan của 1 domain (ví dụ link CDN) nhằm chọn hết vào whitelist
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<string> DetectAllLink(string domain)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) coc_coc_browser/97.0.206 Chrome/91.0.4472.206 Safari/537.36");
            // https://stackoverflow.com/questions/10576686/c-sharp-regex-pattern-to-extract-urls-from-given-string-not-full-html-urls-but
            string rawString = webClient.DownloadString(domain);
            var links = rawString.Split("\t\n \"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("http://") || s.StartsWith("www.") || s.StartsWith("https://"));
            List<string> res = new List<string>();
            foreach(string l in links)
            {
                try
                {
                    Uri temp = new Uri(l);
                    if (!res.Contains(temp.Host))
                        res.Add(temp.Host);
                }
                catch { }
            }
            return res;
        }
    }
}
