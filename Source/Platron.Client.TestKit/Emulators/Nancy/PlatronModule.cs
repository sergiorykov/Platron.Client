using System;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using Nancy;

namespace Platron.Client.TestKit.Emulators.Nancy
{
    public sealed class PlatronModule : NancyModule
    {
        public const string ResultUrlRoute = "/platron/result";
        private static readonly ISubject<ServerRequestContext> requests = new Subject<ServerRequestContext>();

        public PlatronModule()
        {
            Get[ResultUrlRoute] = _ =>
            {
                var context = new ServerRequestContext(Request.Url);

                requests.OnNext(context);
                context.WaitForResponse(TimeSpan.FromMinutes(5));
                return AsXml(context.Response);
            };

            Post[ResultUrlRoute] = _ =>
            {
                // we could expect if incoming request was in POST, that their callback request will be POST too. But it is not.
                // Platron always sends response thru get.
                throw new InvalidOperationException("Not expected: callback as POST");
            };
        }

        public static IObservable<ServerRequestContext> Requests => requests;

        private Response AsXml(string xml)
        {
            return new Response
                   {
                       ContentType = "application/xml; charset:utf-8",
                       Contents = stream =>
                       {
                           var data = Encoding.UTF8.GetBytes(xml);
                           stream.Write(data, 0, data.Length);
                       },
                       StatusCode = (HttpStatusCode) System.Net.HttpStatusCode.OK
                   };
        }

        public static ServerRequestContext WaitForRequest(TimeSpan timeout)
        {
            ServerRequestContext context = null;
            var timeToStop = new ManualResetEvent(false);

            using (Requests.Subscribe(
                x =>
                {
                    context = x;
                    timeToStop.Set();
                },
                () => timeToStop.Set()))
            {
                var requestReceived = timeToStop.WaitOne(timeout);
                if (!requestReceived)
                {
                    throw new InvalidOperationException("Request from platron didn't received");
                }

                return context;
            }
        }
    }
}