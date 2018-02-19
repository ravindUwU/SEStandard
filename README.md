<div align="center">

<img src="images/logo.png" height="100">

# SEStandard<sup>†</sup>

**A C# library for querying the Stack Exchange API.**

[![NuGet Package](https://img.shields.io/nuget/v/RavinduL.SEStandard.svg?style=flat-square)](https://www.nuget.org/packages/RavinduL.SEStandard)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RavinduL.SEStandard.svg?style=flat-square)](https://www.nuget.org/packages/RavinduL.SEStandard)
[![MIT License](https://img.shields.io/github/license/RavinduL/SEStandard.svg?style=flat-square)](https://github.com/RavinduL/SEStandard/blob/master/LICENSE)
[![StackApps Post](https://img.shields.io/badge/StackApps-%237777-000000.svg?style=flat-square)](https://stackapps.com/q/7777/36931)

[Features](#features) |
[Installation](#installation) |
[Usage](#usage) |
[Contributing](#contributing) |
[Credits](#credits)

</div>

<br>

## Features

- Supports [all endpoints](https://api.stackexchange.com/docs?tab=category#docs) of the Stack Exchange API version `2.2`.
- [.NET Standard `1.1`](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.1.md) compliant.
- `Task<T>` based; asynchronous.
- Provides helper methods for [implicit authentication](https://api.stackexchange.com/docs/authentication).

<br>

## Installation

To install this library via **NuGet** ([package](https://www.nuget.org/packages/RavinduL.SEStandard)),

```Text
	PM> Install-Package RavinduL.SEStandard -Version 1.0.0-rc
```

To install the NuGet package via the **.NET Core CLI**,

```Text
	PS> dotnet add package RavinduL.SEStandard --version 1.0.0-rc
```

Should you prefer to manually build the library from its source instead, please refer to the [Contributing](#contributing) section for instructions on how to do so.

<br>

## Usage

Sub-topics:
[Basic Usage](#basic-usage) |
[Implicit Authentication](#implicit-authentication) |
[Concerns](#concerns) |
[Customization](#customization)

<br>

### Basic Usage

<dl>

<dt>

Step 1: Create a new instance of the `StackExchangeClient` class,

</dt>

<dd>

```CSharp
using RavinduL.SEStandard;
using Scopes = RavinduL.SEStandard.Models.Scopes;

// Create an anonymous client to access endpoints of the API that don't require authentication.
var client = new StackExchangeClient();

// -- or --

// Create an non-anonymous client to be able to access all endpoints of the API.
var client = new StackExchangeClient
(
	clientId: 1234,
	key: "key",
	scopes: Scopes.ReadInbox | Scopes.PrivateAccess | Scopes.WriteAccess,

	// (optional)
	// If you've got an custom access token stored away, that you'd like to re-use,
	accessToken: "accessToken"
	// Ensure that the specified scopes correspond to the custom access token.
);
```

</dd>

<dt>

Step 2: Use the newly created `StackExchangeClient` object to access data on the Stack Exchange API,

</dt>

<dd>

```CSharp
// To retrieve and list the 10 highest voted questions on Stack Overflow,

using Order = RavinduL.SEStandard.Models.Order;
using QuestionSort = RavinduL.SEStandard.Models.QuestionSort;

var query = await client.Questions.GetAsync
(
	site: "stackoverflow",
	pagesize: 10,
	order: Order.Descending,
	sort: QuestionSort.Votes
);

foreach (var question in query.Items)
{
	Console.WriteLine($"{question.Title} (score: {question.Score})");
}
```

</dd>

</dl>

<br>

### Implicit Authentication

To participate in the [implicit authentication flow](https://api.stackexchange.com/docs/authentication), a non-anonymous `StackExchangeClient` object is required.

In this contrived example, the `WebBrowser` class represents a web browser that, upon navigation or redirection to a new webpage, fires its `event NavigationDelegate Navigated` whose arguments contain the new URL (as a property identified as `NewUrl`). The `StackExchangeClient` object is identified as `client`.

<dl>

<dt>

Step 1: Retrieve the implicit authentication URL, and direct the user to it,

</dt>

<dd>

```CSharp
var url = client.GetImplicitAuthenticationUrl();

// Direct the user to uri
WebBrowser.Navigate(url);
```

The user will be presented with a webpage with options to login to their Stack Exchange account. During the process of logging in, the user will be redirected multiple times.

</dd>

<dt>

Step 2: Handle redirections from the implicit authentication URL until authentication succeeds or fails.

</dt>

<dd>

```CSharp
NavigationDelegate WebBrowser_Navigated = null;
WebBrowser_Navigated = (sender, eventArgs) =>
{
	var result = client.TryImplicitAuthentication(eventArgs.NewUrl);

	if (result.State == ImplicitAuthenticationState.Successful)
	{
		// The client is now authenticated, and has a valid access token.

		Console.WriteLine($"Access Token: {result.AccessToken}");
		Console.WriteLine(client.HasAccessToken); // True

		// Unsubscribe from the event.
		WebBrowser.Navigated -= WebBrowser_Navigated;
	}
	// Do not use else here, as ImplicitAuthenticationState.Ignored should be ignored.
	else if (result.State == ImplicitAuthenticationState.Failed)
	{
		Console.WriteLine("Authentication failed");

		// Unsubscribe from the event.
		WebBrowser.Navigated -= WebBrowser_Navigated;
	}
};

WebBrowser.Navigated += WebBrowser_Navigated;
```

</dd>

</dl>

<br>

### Concerns

-	**Rate Limiting**  
	Read about the request throttling policies of the Stack Exchange API at [api.stackexchange.com/docs/throttle](https://api.stackexchange.com/docs/throttle).

	It is the reponsibility of all applications that use this library, to respect the rate limits imposed by the Stack Exchange API when making requests to it, themselves, in lieu of this library not doing so. The `Wrapper<T>` object ([Stack Exchange documentation](https://api.stackexchange.com/docs/wrapper)) in which all API responses are wrapped contains the properties `BackOff`, `QuotaMax`, and `QuotaRemaining`, that are relevant in this regard.

-	**Custom Access Tokens**  
	When a non-anonymous `StackExchangeClient` object is constructed with a custom access token specified, ensure that it is valid. The `StackExchangeClient` object does not perform validation of the access token.

-	**`min` and `max` Parameters**  
	The `min` and `max` parameters that specify minimum and maximum values that requests are sorted by, are `object`s. To use them, make sure to assign data of the appropriate type (i.e. `DateTime`s for dates, `int` for numeric values, etc.)

<br>

### Customization

The functionality of the `StackExchangeClient` object can be tweaked in numerous ways by sub-classing it.

<dl>

<dt>

#### Customizing JSON Deserialization

</dt>

<dd>

By default, this library uses the [Json.NET](https://www.newtonsoft.com/json) library for deserializing the JSON responses from the Stack Exchange API. Should you wish to deserialize JSON some other way,

```CSharp
using RavinduL.SEStandard;
using RavinduL.SEStandard.Serialization;

// Sub-class the IJsonDeserializer class.
public class CustomJsonDeserializer : IJsonDeserializer
{
	public T Deserialize<T>(string json)
	{
		// Add logic to deserialize JSON to an object of type T here.
	}
}

// Sub-class the StackExchangeClient class, and override the CreateJsonDeserializer method.
public class CustomStackExchangeClient : StackExchangeClient
{
	public override IJsonDeserializer CreateJsonDeserializer()
	{
		return new CustomJsonDeserializer();
	}
}
```

</dd>

<dt>

#### Using a custom `HttpClient`

</dt>

<dd>

By default, every instance of the `StackExchangeClient` class utilizes its own [`HttpClient`](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) object to make HTTP requests. The `HttpClient` class is robust, and thread-safe -- a single instance of which should ideally be used by the entire application.

To provide a custom `HttpClient` (the instance used by your application, for example) for the `StackExchangeClient` to utilize,

```CSharp
using RavinduL.SEStandard;
using RavinduL.SEStandard.Http;

// Sub-class the StackExchangeClient class, and override the CreateHttpRequester method.
public class CustomStackExchangeClient : StackExchangeClient
{
	public override IHttpRequester CreateHttpRequester()
	{
		var httpClient = /* custom HttpClient object */;
		return new HttpClientRequester(httpClient);
	}
}
```

</dd>

</dl>

<dt>

#### Customizing HTTP requests

</dt>

<dd>

To completely customize the means by which HTTP requests are made, create an implementation of the `IHttpRequester` interface. You may also want to create your own corresponding implementation of the `IHttpResponse` interface.

```CSharp
using RavinduL.SEStandard.Http;

// Optionally, create an implementation of the IHttpResponse interface.
public class CustomHttpResponse : IHttpResponse
{
	// ...
}

// Create an implementation of the IHttpRequester interface.
public class CustomHttpRequester : IHttpRequester
{
	public async Task<IHttpResponse> PerformRequestAsync(string url, Dictionary<string, string> data, HttpRequestMethod requestMethod)
	{
		// Perform HTTP request here.

		CustomHttpResponse response = /* Initialize an IHttpResponse to return */;
		return response;
	}
}

// Sub-class the StackExchangeClient class, and override the CreateHttpRequester method.
public class CustomStackExchangeClient : StackExchangeClient
{
	public override IHttpRequester CreateHttpRequester()
	{
		return new CustomHttpRequester();
	}
}
```

</dd>

<br>

## Contributing

Please refer to [CONTRIBUTING.md](CONTRIBUTING.md) for information on setting up this project for development, repository layout, etc.

<br>

## Credits

Massive thanks to the developers and contributors of the following incredible open-source projects,

- [Json.NET](https://www.newtonsoft.com/json)
- [Flurl](https://tmenier.github.io/Flurl/)
- [Handlebars.Net](https://github.com/rexm/Handlebars.Net)
- [Cake](https://cakebuild.net/), and [Frosting](https://github.com/cake-build/frosting)
- [xUnit.net](https://xunit.github.io/)

<br>

---

`†` **This is not an official product of Stack Exchange Inc.** The name was inspired by that of the [`NETStandard.Library`](https://www.nuget.org/packages/NETStandard.Library/) NuGet package.
