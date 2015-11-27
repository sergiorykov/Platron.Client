using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Nancy;

namespace Platron.Client.TestKit.Emulators.Tunnels
{
    public class NgrokTunnel : IDisposable
    {
        public const string FileName = "ngrok.exe";
        public const string ProcessName = "ngrok";
        private readonly Process _ngrok;
        private readonly ManualResetEvent _awaitForwarding = new ManualResetEvent(false);

        public NgrokTunnel(int port, TimeSpan timeout)
        {
            foreach (var existingNgrok in Process.GetProcessesByName(ProcessName))
            {
                existingNgrok.Kill();
            }

            _ngrok = Process.Start(new ProcessStartInfo
                                   {
                                       FileName = FileName,
                                       Arguments = $"http {port}",
                                       UseShellExecute = false,
                                       WindowStyle = ProcessWindowStyle.Hidden,
                                       //CreateNoWindow = true,
                                       RedirectStandardOutput = true,
                                       RedirectStandardError = true
                                   });

            _ngrok.EnableRaisingEvents = true;

            _ngrok.OutputDataReceived += (sender, args) =>
            {
                var lines = args.Data
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                var forwarding = lines
                    .Where(x => x.StartsWith("Forwarding"))
                    .Select(ExtractForwarding)
                    .ToList();

                HttpAddress = forwarding.FirstOrDefault(x => !x.IsSecure);
                HttpsAddress = forwarding.FirstOrDefault(x => x.IsSecure);

                _awaitForwarding.Set();
            };

            _ngrok.ErrorDataReceived += (sender, args) =>
            {
                _awaitForwarding.Set();
            };

            _awaitForwarding.WaitOne(timeout);

            if (HttpAddress == null)
            {
                var forwarding = GetForwarding();

                HttpAddress = forwarding.FirstOrDefault(x => !x.IsSecure);
                HttpsAddress = forwarding.FirstOrDefault(x => x.IsSecure);
            }
        }

        private List<Url> GetForwarding()
        {
            var http = new HttpClient();
            var content = http.GetStringAsync("http://127.0.0.1:4040/inspect/http").Result;

            string htmlPart = content
                .Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(x => x.Contains("window.common") && x.Contains("Session"));

            //             window.common = JSON.parse("{\"Session\":{\"Status\":1,\"LastError\":\"\",\"Version\":\"2.0.19\",\"Tunnels\":{\"command_line\":{\"URL\":\"https://ba2c0651.ngrok.io\",\"Proto\":\"https\",\"Metrics\":{\"conns\":{\"count\":0,\"gauge\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0},\"http\":{\"count\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0}},\"Session\":{\"Logger\":{}},\"Config\":{\"name\":\"command_line\",\"inspect\":true,\"addr\":\"localhost:61590\",\"Subdomain\":\"\",\"Hostname\":\"\",\"Auth\":\"\",\"HostHeader\":\"\",\"BindTLS\":\"both\"}},\"command_line (http)\":{\"URL\":\"http://ba2c0651.ngrok.io\",\"Proto\":\"http\",\"Metrics\":{\"conns\":{\"count\":0,\"gauge\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0},\"http\":{\"count\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0}},\"Session\":{\"Logger\":{}},\"Config\":{\"name\":\"command_line (http)\",\"inspect\":true,\"addr\":\"localhost:61590\",\"Subdomain\":\"\",\"Hostname\":\"ba2c0651.ngrok.io\",\"Auth\":\"\",\"HostHeader\":\"\",\"BindTLS\":\"false\"}}},\"Metrics\":{\"conns\":{\"count\":0,\"gauge\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0},\"http\":{\"count\":0,\"rate1\":0,\"rate5\":0,\"rate15\":0,\"p50\":0,\"p90\":0,\"p95\":0,\"p99\":0}}},\"Update\":{\"Status\":0,\"Version\":\"\",\"Error\":\"\"}}");
            if (string.IsNullOrEmpty(htmlPart))
            {
                return new List<Url>();
            }

            // \"URL\":\"https://ba2c0651.ngrok.io\"
            var addresses = htmlPart
                .Split(new[] {"\\\"URL\\\""}, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.StartsWith(":\\\"http"))
                .Select(x => x.Substring(":\\\"".Length))
                .Select(x => x.Substring(0, x.IndexOf('\\')).Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            return addresses.ConvertAll(x => new Url(x));
        }

        public Url HttpAddress { get; private set; }
        public Url HttpsAddress { get; private set; }

        private Url ExtractForwarding(string value)
        {
            if (!value.StartsWith("Forwarding"))
            {
                return null;
            }

            // Forwarding                    http://ac681073.ngrok.io -> localhost:5341
            var mapping = value.Substring("Forwarding".Length).Trim();
            var address = mapping.Split(new[] {"->"}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }

            return new Url(address.Trim());
        }

        public void Dispose()
        {
            _ngrok.Kill();
        }
    }
}