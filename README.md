
# Platron.Client
A Platron API client library for .NET.

Based on API **version 3.5** ([EN](http://www.platron.ru/integration/Merchant_Platron_API_EN.pdf "Merchant_Platron_API_EN.pdf") | [RU](http://www.platron.ru/integration/Merchant_Platron_API_RU.pdf "Merchant_Platron_API_RU.pdf"))

# Documentation
[http://platronclient.readthedocs.org/en/latest](http://platronclient.readthedocs.org/en/latest)

# Getting started

We will proceed showing how to process payment for order `#1234567890`. 

Client sends to server request to proceed order payment. Server initiates payment sending request to Platron. Platron can return html page to let server show it by itself or redirect url. We will use redirect url. 

If server knows user's phone number (and/or email) it can be added to request in order to simplify payment process for user.

```csharp
var credentials = new Credentials("0000", "asdffsasdfasdfasdf");
var client = new PlatronClient(credentials);

// ensure that your server listens on that address and accepts GET request
var resultUrl = new Uri("https://my.server.com/platron/result");

var request = new InitPaymentRequest(1.Rur(), "Order payment")
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
                    StatusCode = (HttpStatusCode) System.Net.HttpStatusCode.OK
                };
    }
}
```


# Building

If you don't have installed VS extension NuGet Package Manager, then install it or just execute `restore.cmd`. 

Open solution `Source\Platron.sln` and build it. 

# Testing

Tests separated into several categories. 

`Integration` and `Manual` tests requires real credentials. You need to set them in environment variables:

	PLATRON_MERCHANTID=1234
	PLATRON_SECRETKEY=erwer87werwer8wer6
	PLATRON_PHONENUMBER=+79990001112

`Manual` tests ignored by default - they require interraption of test to fullfill - guess what :) - manual action like proceeding real payment for 1 ruble using your preferred payment system and then rejecting it by emulator of shop server. Platron has testing payment systems but you cann't complete scenario with it and receive confirmation ResultUrl callback from Platron.

# Release

To issue release `vX.Y.Z` execute 

    release.cmd X.Y.Z
and take packages from `out\Release\Packages`. It's that simple. 