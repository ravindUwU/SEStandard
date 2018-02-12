namespace RavinduL.SEStandard
{
	public class ImplicitAuthenticationResult
	{
		private ImplicitAuthenticationResult()
		{
		}

		/// <summary>
		/// Gets the access token acquired by the <see cref="StackExchangeClient"/> object.
		/// </summary>
		public string AccessToken { get; private set; }

		/// <summary>
		/// Gets the amount of time in which the <see cref="AccessToken"/> expires.
		/// </summary>
		public int? Expires { get; private set; }

		/// <summary>
		/// Gets the error that occured during authentication.
		/// </summary>
		public Models.ImplicitAuthenticationError? Error { get; private set; }

		/// <summary>
		/// Gets the description of the error.
		/// </summary>
		public string ErrorDescription { get; private set; }

		/// <summary>
		/// Gets the state of the implicit authentication attempt.
		/// </summary>
		public ImplicitAuthenticationState State { get; private set; }

		internal static ImplicitAuthenticationResult ForSuccess(string accessToken, int? expires) => new ImplicitAuthenticationResult
		{
			State = ImplicitAuthenticationState.Successful,
			AccessToken = accessToken,
			Expires = expires,
		};

		internal static ImplicitAuthenticationResult ForIgnore() => new ImplicitAuthenticationResult
		{
			State = ImplicitAuthenticationState.Ignore,
		};

		internal static ImplicitAuthenticationResult ForFailure(string error, string description) => new ImplicitAuthenticationResult
		{
			State = ImplicitAuthenticationState.Failed,
			Error = new Models.ImplicitAuthenticationErrorConverter().GetEnum(error) as Models.ImplicitAuthenticationError?,
			ErrorDescription = description,
		};
	}
}
