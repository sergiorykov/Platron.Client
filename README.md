
# Platron.Client [![NuGet](https://img.shields.io/nuget/v/Platron.Client.svg)](https://www.nuget.org/packages/Platron.Client) [![AppVeyor](https://img.shields.io/appveyor/ci/sergiorykov/platron-client.svg)](https://ci.appveyor.com/project/sergiorykov/platron-client)

A Platron API client library for .NET. 
Based on API **version 3.5** ([EN](http://www.platron.ru/integration/Merchant_Platron_API_EN.pdf "Merchant_Platron_API_EN.pdf") | [RU](http://www.platron.ru/integration/Merchant_Platron_API_RU.pdf "Merchant_Platron_API_RU.pdf"))

# Documentation
[http://platronclient.readthedocs.org/en/latest](http://platronclient.readthedocs.org/en/latest)

# Getting started

We will show how to process payment for sample order `#1234567890`. 

Client sends to server request to proceed order payment. Server initiates payment sending request to Platron. Platron can return html page to let server show it by itself or redirect url. We will use redirect url. 

If server knows user's phone number (and/or email) it can be added to request in order to simplify payment process for user.

```csharp
var credentials = new Credentials("0000", "asdffsasdfasdfasdf");
var client = new PlatronClient(credentials);

// ensure that your server listens on that address and accepts GET request
var resultUrl = new Uri("https://my.server.com/platron/result");

var request = new InitPaymentRequest(1.Rub(), "Order payment")
                    {
                        OrderId = "#1234567890",
                        UserPhone = "+79990001112",
                        ResultUrl = resultUrl
                    };

InitPaymentResponse response = await client.InitPaymentAsync(request).ConfigureAwait(false);
await SendToUserAsync(response.RedirectUrl).ConfigureAwait(false);
```

User (or your client) opens link and proceed payment in browser. Platron sends request to ResultUrl GET with details of payment and awaits completing the order. 

We will show how to work with server side processing using simple but complete server on [Nancy](http://nancyfx.org). 

We will not cover hosting and bootstrapping details, the only important thing here is base address of our server:

```csharp
WebApp.Start<Startup>($"https://my.server.com");
```

Server accepts ResultUrl and can return OK, Error or Rejected response back to Platron. There is ResultUrl processing module:

```csharp

public sealed class PlatronModule : NancyModule
{
    private readonly PlatronClient _platronClient;

    public PlatronModule(PlatronClient platronClient)
    {
        // IoC container will make us super-duper happy and gives us a client.
        _platronClient = platronClient;

        Get["/platron/result", true] = async (_, ct) =>
        {
            CallbackResponse response = await CompleteOrderAsync(Request.Url);
            return AsXml(response);
        };
    }

    private async Task<CallbackResponse> CompleteOrderAsync(Uri resultUrl)
    {
        ResultUrlRequest request = _platronClient.ResultUrl.Parse(resultUrl);
        CallbackResponse response = _platronClient.ResultUrl.ReturnOk(request, "Order completed");
        return await Task.FromResult(response);
    }

    private Response AsXml(CallbackResponse response)
    {
        return new Response
        {
            ContentType = "application/xml; charset:utf-8",
            Contents = stream =>
            {
                var data = Encoding.UTF8.GetBytes(response.Content);
                stream.Write(data, 0, data.Length);
            },
            StatusCode = HttpStatusCode.OK
        };
    }    
}

```

# Logging

Logging is implemented by integrating [LibLog](https://www.nuget.org/packages/LibLog/) inside library. See details in post: [http://sergiorykov.github.io/Adding-Logging-To-A-Library-Using-LibLog](http://sergiorykov.github.io/Adding-Logging-To-A-Library-Using-LibLog/). 

# Testing

Tests separated into several categories. 

`Integration` and `Manual` tests requires real credentials. You need to set them in environment variables:

	PLATRON_TEST_MERCHANTID=1234
	PLATRON_TEST_SECRETKEY=erwer87werwer8wer6
	PLATRON_TEST_PHONENUMBER=+79990001112

`Manual` tests ignored by default - they require interraption of test to fulfill - guess what :) - manual action like proceeding real payment for 1 ruble using your preferred payment system and then rejecting it by emulator of shop server. Platron has testing payment systems but you cann't complete scenario with it and receive confirmation ResultUrl callback from Platron.

# Integration Testing [![NuGet](https://img.shields.io/nuget/v/Platron.Client.TestKit.svg?label=Platron.Client.TestKit)](https://www.nuget.org/packages/Platron.Client.TestKit)

You can drastically simplify integration testing using package [Platron.Client.TestKit](https://www.nuget.org/packages/Platron.Client.TestKit). It requires a lot of dependencies like Nansy, Owin, RX but it's made to be able write integration test in a few lines of code (we used XUnit):

```csharp
public sealed class CallbackIntegrationTests : IClassFixture<CallbackServerEmulator>
{
    private readonly CallbackServerEmulator _server;
    private readonly ITestOutputHelper _output;

    public CallbackIntegrationTests(CallbackServerEmulator server, ITestOutputHelper output)
    {
        server.Start();

        _server = server;
        _output = output;
    }

    [Fact]
    public async Task FullPayment_ManualPaymentThruBrowser_Succeeds()
    {
        var connection = new Connection(PlatronClient.PlatronUrl, SettingsStorage.Credentials,
            HttpRequestEncodingType.PostWithQueryString);

        var client = new PlatronClient(connection);

        var initPaymentRequest = new InitPaymentRequest(1.01.Rub(), "verifying resulturl")
                                    {
                                        ResultUrl = _server.ResultUrl,
                                        UserPhone = SettingsStorage.PhoneNumber,
                                        OrderId = Guid.NewGuid().ToString("N"),
                                        NeedUserPhoneNotification = true
                                    };

        // enables only test systems
        //initPaymentRequest.InTestMode();

        var response = await client.InitPaymentAsync(initPaymentRequest);

        // open browser = selenium can be here ^)
        Assert.NotNull(response);
        Assert.NotNull(response.RedirectUrl);
        Browser.Open(response.RedirectUrl);

        // we have some time to manually finish payment.
        var request = _server.WaitForRequest(TimeSpan.FromMinutes(3));
        _output.WriteLine(request.Uri.AbsoluteUri);

        var resultUrl = client.ResultUrl.Parse(request.Uri);

        // to return money back - it's enough to reject payment
        // and hope that your payment service supports it.
        var resultUrlResponse = client.ResultUrl.TryReturnReject(resultUrl, "sorry, my bad...");
        _output.WriteLine(resultUrlResponse.Content);

        request.SendResponse(resultUrlResponse.Content);
    }
}
```

You can [grab sources](https://github.com/sergiorykov/Platron.Client/blob/master/Source/Platron.Client.Tests/Integration/CallbackIntegrationTests.cs) and try it by yourself. You will need real credentials and magic sauce to make your local server available to Platron. 

To make it you will need tunnel service like https://forwardhq.com, https://ngrok.com or any similar. We choose ngrok - it has free of charge version working on a single address.  

You will need to [download ngrok](https://ngrok.com/download) and make it available in PATH. Register free account and save API key. Everything else will  do automagically by `CallbackServerEmulator`:

```csharp
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
    
    /// Other methods
}
```


# Building

 * Restore NuGet dependencies. If you don't have installed VS extension NuGet Package Manager, then install it or just execute `restore.cmd`. 

 * Open solution `Source\Platron.sln` and build it. 

# Release

To issue release `X.Y.Z` execute 

    release.cmd X.Y.Z
and take packages from `out\Release\Packages`. It's that simple. 

# Build server [![AppVeyor](https://img.shields.io/appveyor/ci/sergiorykov/platron-client.svg)](https://ci.appveyor.com/project/sergiorykov/platron-client)

Configured [base CI](https://github.com/sergiorykov/Platron.Client/blob/master/appveyor.yml) on [AppVeyor](https://ci.appveyor.com/project/sergiorykov/platron-client) to build every commit. Not ready to finish release pipeline.  

# License

[MIT](https://github.com/sergiorykov/Platron.Client/blob/master/LICENSE)