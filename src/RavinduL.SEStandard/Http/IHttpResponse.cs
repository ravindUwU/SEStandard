namespace RavinduL.SEStandard.Http
{
	/// <summary>
	/// A response to a HTTP request made by an <see cref="IHttpRequester"/>.
	/// </summary>
	public interface IHttpResponse
	{
		/// <summary>
		/// Gets the content of the HTTP response.
		/// </summary>
		string Content { get; }

		/// <summary>
		/// Gets the status code of the HTTP response.
		/// </summary>
		int StatusCode { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="StatusCode"/> is indicative of the request completing successfully.
		/// </summary>
		bool IsSuccessStatusCode { get; }
	}
}
