namespace RavinduL.SEStandard.Http
{
	using System.Net.Http;
	using System.Threading.Tasks;

	/// <summary>
	/// A response to an HTTP request made by a <see cref="HttpClientRequester"/>.
	/// </summary>
	/// <seealso cref="IHttpResponse" />
	public sealed class HttpClientResponse : IHttpResponse
	{
		/// <summary>
		/// Gets the content of the HTTP response.
		/// </summary>
		public string Content { get; private set; }

		/// <summary>
		/// Gets the status codeof the HTTP response.
		/// </summary>
		public int StatusCode { get; private set; }

		/// <summary>
		/// Gets a value indicating if the <see cref="StatusCode" /> is indicative of the request completing successfully.
		/// </summary>
		public bool IsSuccessStatusCode { get; private set; }

		private HttpClientResponse()
		{
		}

		/// <summary>
		/// Asynchronously genrates an <see cref="HttpClientResponse"/> from the specified <see cref="HttpResponseMessage"/>.
		/// </summary>
		public static async Task<HttpClientResponse> FromHttpResponseMessageAsync(HttpResponseMessage responseMessage)
		{
			return new HttpClientResponse
			{
				Content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false),
				StatusCode = (int)responseMessage.StatusCode,
				IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
			};
		}
	}
}
