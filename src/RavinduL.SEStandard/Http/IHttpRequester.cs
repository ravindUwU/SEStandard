namespace RavinduL.SEStandard.Http
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// Provides methods for performing HTTP requests.
	/// </summary>
	/// <seealso cref="IDisposable" />
	public interface IHttpRequester : IDisposable
	{
		/// <summary>
		/// Performs an asynchronous HTTP request.
		/// </summary>
		/// <param name="url">The URL to which the HTTP request should be made.</param>
		/// <param name="data">The data to be submitted with the HTTP request.</param>
		/// <param name="requestMethod">The HTTP method to use.</param>
		Task<IHttpResponse> PerformRequestAsync(string url, Dictionary<string, string> data, HttpRequestMethod requestMethod);
	}
}
