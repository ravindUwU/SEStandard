namespace RavinduL.SEStandard
{
	/// <summary>
	/// Represents the state of an attempt to complete the implicit authentication flow via <see cref="StackExchangeClient.TryImplicitAuthentication(System.Uri)"/>.
	/// </summary>
	public enum ImplicitAuthenticationState
	{
		/// <summary>
		/// The attempt to authenticate was successful, and an access token has been acquired.
		/// </summary>
		Successful,

		/// <summary>
		/// The attempt to authenticate failed.
		/// </summary>
		Failed,

		/// <summary>
		/// The state of the attempt to authenticate couldn't be determined.
		/// </summary>
		Ignore,
	}
}
