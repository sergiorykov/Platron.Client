using System;
using System.Threading;

namespace Platron.Client.TestKit.Emulators
{
    public sealed class ServerRequestContext
    {
        private readonly ManualResetEvent _awaitResponse = new ManualResetEvent(false);

        public ServerRequestContext(Uri uri)
        {
            Uri = uri;
        }

        public Uri Uri { get; set; }

        public string Response { get; private set; }

        public void SendResponse(string response)
        {
            Response = response;
            _awaitResponse.Set();

            // to allow nancy send response back
            Thread.Sleep(250);
        }

        public void WaitForResponse(TimeSpan timeout)
        {
            var signalReceived = _awaitResponse.WaitOne(timeout);
            if (!signalReceived)
            {
                throw new InvalidOperationException("Response to platron timeout");
            }
        }
    }
}