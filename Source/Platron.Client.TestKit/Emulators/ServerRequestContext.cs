using System;
using System.Threading;
using Nancy;

namespace Platron.Client.TestKit.Emulators
{
    public sealed class ServerRequestContext
    {
        public Url Url { get; set; }
        private readonly ManualResetEvent _awaitResponse = new ManualResetEvent(false);

        public ServerRequestContext(Url url)
        {
            Url = url;
        }

        public string Response { get; private set; }

        public void SendResponse(string response)
        {
            Response = response;
            _awaitResponse.Set();
        }

        public void WaitForResponse(TimeSpan timeout)
        {
            bool signalReceived = _awaitResponse.WaitOne(timeout);
            if (!signalReceived)
            {
                throw new InvalidOperationException("Response to platron timeout");
            }
        }
    }
}