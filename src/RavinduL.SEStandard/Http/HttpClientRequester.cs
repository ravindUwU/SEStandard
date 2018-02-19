namespace RavinduL.SEStandard.Http
{
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Flurl;

	/// <summary>
	/// Provides a mechanism to perform HTTP requests using a <see cref="HttpClient"/> object.
	/// </summary>
	/// <seealso cref="IHttpRequester" />
	public sealed class HttpClientRequester : IHttpRequester
	{
		private HttpClient httpClient;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpClientRequester"/> class that uses the specified <see cref="HttpClient"/> to perform HTTP requests.
		/// </summary>
		/// <param name="httpClient">The <see cref="HttpClient"/> to use for performing HTTP requests.</param>
		/// <exception cref="ArgumentNullException">Thrown if the specified <see cref="HttpClient"/> is <c>null</c>.</exception>
		public HttpClientRequester(HttpClient httpClient)
		{
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		/// <summary>
		/// Performs an asynchronous HTTP request.
		/// </summary>
		/// <param name="url">The URL to which the HTTP request should be made.</param>
		/// <param name="data">The data to be submitted with the HTTP request.</param>
		/// <param name="requestMethod">The HTTP method to use.</param>
		/// <exception cref="ArgumentException">Thrown if the specified <see cref="HttpRequestMethod"/> is invalid.</exception>
		public async Task<IHttpResponse> PerformRequestAsync(string url, Dictionary<string, string> data, HttpRequestMethod requestMethod)
		{
			HttpResponseMessage response = null;

			switch (requestMethod)
			{
				case HttpRequestMethod.GET:
					response = await httpClient.GetAsync(url.SetQueryParams(data)).ConfigureAwait(false);
					break;

				case HttpRequestMethod.POST:
					response = await httpClient.PostAsync(url, new FormUrlEncodedContent(data)).ConfigureAwait(false);
					break;

				default:
					throw new ArgumentException(nameof(requestMethod));
			}

			return await HttpClientResponse.FromHttpResponseMessageAsync(response).ConfigureAwait(false);
		}

		public void Dispose()
		{
			httpClient.Dispose();
		}
	}
}
