namespace RavinduL.SEStandard.Http
{
	/// <summary>
	/// A response to an HTTP request made by a <see cref="IHttpRequester"/>.
	/// </summary>
	public interface IHttpResponse
	{
		/// <summary>
		/// Gets the content of the HTTP response.
		/// </summary>
		string Content { get; }

		/// <summary>
		/// Gets the status codeof the HTTP response.
		/// </summary>
		int StatusCode { get; }

		/// <summary>
		/// Gets a value indicating if the <see cref="StatusCode"/> is indicative of the request completing successfully.
		/// </summary>
		bool IsSuccessStatusCode { get; }
	}
}
