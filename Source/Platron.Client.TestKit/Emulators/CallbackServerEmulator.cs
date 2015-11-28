using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Owin.Hosting;
using Platron.Client.TestKit.Emulators.Nancy;
using Platron.Client.TestKit.Emulators.Tunnels;

namespace Platron.Client.TestKit.Emulators
{
    public sealed class CallbackServerEmulator : IDisposable
    {
        private IDisposable _app;
        private IDisposable _tunnel;

        public Uri LocalAddress { get; private set; }
        public Uri ExternalAddress { get; private set; }
        public int Port { get; private set; }

        public void Start()
        {
            var port = FreeTcpPort();
            Start(port);
        }

        public void Start(int port)
        {
            _app = WebApp.Start<Startup>($"http://+:{port}");

            // doesn't require license to run single instance with generated domain
            var ngrok = new NgrokTunnel(port, TimeSpan.FromSeconds(2));
            _tunnel = ngrok;

            LocalAddress = new Uri($"http://localhost:{port}");
            ExternalAddress = ngrok.HttpsAddress;
            Port = port;
        }

        public void Stop()
        {
            if (_tunnel != null)
            {
                _tunnel.Dispose();
                _tunnel = null;
            }

            if (_app != null)
            {
                _app.Dispose();
                _app = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private static int FreeTcpPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint) tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
    }
}