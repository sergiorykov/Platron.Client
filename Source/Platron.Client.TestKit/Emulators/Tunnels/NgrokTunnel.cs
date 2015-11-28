using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Nancy;

namespace Platron.Client.TestKit.Emulators.Tunnels
{
    public sealed class NgrokTunnel : IDisposable
    {
        public const string FileName = "ngrok.exe";
        public const string ProcessName = "ngrok";
        public static readonly Uri LocalHttpServerUri = new Uri("http://127.0.0.1:4040/inspect/http");

        private readonly Process _ngrok;

        public NgrokTunnel(int port, TimeSpan timeout)
        {
            ShutdownRunningInstances();

            // ngrok.exe http 8787 : https://asdf12312.ngrok.com/your/path -> http://localhost:8787/your/path
            _ngrok = Process.Start(new ProcessStartInfo
                                   {
                                       FileName = FileName,
                                       Arguments = $"http {port}",
                                       UseShellExecute = false,
                                       // we don't want to wonder - what's the thing it is? - when it's completely hidden
                                       WindowStyle = ProcessWindowStyle.Minimized,
                                   });

            AwaitStarting(timeout);
        }

        private void AwaitStarting(TimeSpan timeout)
        {
            bool ngrokStarted = SpinWait.SpinUntil(() =>
            {
                var forwarding = GetForwarding();

                if (forwarding.Count == 0)
                {
                    return false;
                }

                HttpAddress = forwarding.FirstOrDefault(x => x.Scheme == "http");
                HttpsAddress = forwarding.FirstOrDefault(x => x.Scheme == "https");

                return true;
            }, timeout);

            if (!ngrokStarted)
            {
                throw new InvalidOperationException("ngrok not started");
            }
        }

        public Url HttpAddress { get; private set; }
        public Url HttpsAddress { get; private set; }

        private static void ShutdownRunningInstances()
        {
            foreach (var existingNgrok in Process.GetProcessesByName(ProcessName))
            {
                existingNgrok.Kill();
            }
        }

        private List<Uri> GetForwarding()
        {
            string content;

            try
            {
                var http = new HttpClient();
                content = http.GetStringAsync(LocalHttpServerUri).Result;
            }
            catch (Exception)
            {
                return new List<Uri>();
            }

            //             window.common = JSON.parse("{\"Session\":{\"Status\":1,\"LastError\":\"\",\"Version\":\"2.0.19\",\"Tunnels\":{\"command_line\":{\"URL\":\"https://ba2c0651.ngrok.io\",\"Proto\":\"https\",\"Metrics\":{\"conns\":{\"count\":0,\"gauge\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0},\"http\":{\"count\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0}},\"Session\":{\"Logger\":{}},\"Config\":{\"name\":\"command_line\",\"inspect\":true,\"addr\":\"localhost:61590\",\"Subdomain\":\"\",\"Hostname\":\"\",\"Auth\":\"\",\"HostHeader\":\"\",\"BindTLS\":\"both\"}},\"command_line (http)\":{\"URL\":\"http://ba2c0651.ngrok.io\",\"Proto\":\"http\",\"Metrics\":{\"conns\":{\"count\":0,\"gauge\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0},\"http\":{\"count\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0}},\"Session\":{\"Logger\":{}},\"Config\":{\"name\":\"command_line (http)\",\"inspect\":true,\"addr\":\"localhost:61590\",\"Subdomain\":\"\",\"Hostname\":\"ba2c0651.ngrok.io\",\"Auth\":\"\",\"HostHeader\":\"\",\"BindTLS\":\"false\"}}},\"Metrics\":{\"conns\":{\"count\":0,\"gauge\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0},\"http\":{\"count\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0}}},\"Update\":{\"Status\":0,\"Version\":\"\",\"Error\":\"\"}}");
            var htmlPartWithJson = content
                .Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(x => x.Contains("window.common") && x.Contains("Session"));

            if (string.IsNullOrEmpty(htmlPartWithJson))
            {
                return new List<Uri>();
            }

            // \"URL\":\"https://ba2c0651.ngrok.io\"
            var addresses = htmlPartWithJson
                .Split(new[] {"\\\"URL\\\""}, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.StartsWith(":\\\"http"))
                .Select(x => x.Substring(":\\\"".Length))
                .Select(x => x.Substring(0, x.IndexOf('\\')).Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            return addresses.ConvertAll(x => new Uri(x));
        }

        public void Dispose()
        {
            _ngrok.Kill();
        }
    }
}