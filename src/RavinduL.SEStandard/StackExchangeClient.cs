namespace RavinduL.SEStandard
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Flurl;
	using RavinduL.SEStandard.Http;
	using RavinduL.SEStandard.Serialization;

	/// <summary>
	/// Provides methods to access data on the Stack Exchange API.
	/// </summary>
	/// <seealso cref="IDisposable" />
	public partial class StackExchangeClient : IDisposable
	{
		/// <summary>
		/// The URL to which all API requests would be made.
		/// </summary>
		public const string ApiBaseUrl = "https://api.stackexchange.com/2.2";

		/// <summary>
		/// The URL at which the user will be able to log in to their Stack Exchange account, for implicit authentication.
		/// </summary>
		public const string ImplicitAuthenticationPromptUrl = "https://stackexchange.com/oauth/dialog";

		/// <summary>
		/// The URL to which the user will be redirected after implicit authentication.
		/// </summary>
		public const string ImplicitAuthenticationRedirectUrl = "https://stackexchange.com/oauth/login_success";

		/// <summary>
		/// Gets a value indicating whether this <see cref="StackExchangeClient"/> object is anonymous (refer to the constructors for more information).
		/// </summary>
		public bool IsAnonymous { get; } = true;

		/// <summary>
		/// Gets a value indicating whether this <see cref="StackExchangeClient"/> object has an access token. This doesn't imply its validity.
		/// <para>Access tokens can be specified via a constructor, or can be obtained via implicit authentication.</para>
		/// </summary>
		public bool HasAccessToken => !String.IsNullOrWhiteSpace(accessToken);

		public int ClientId { get; }
		public string Key { get; }

		private string accessToken;
		private Models.Scopes _scopes;
		private string scopesString = null;

		/// <summary>
		/// Gets the scopes (permissions) that this <see cref="StackExchangeClient"/> object has, to data on the Stack Exchange network.
		/// <para>Visit https://api.stackexchange.com/docs/authentication#scope for more information about scopes.</para>
		/// </summary>
		public Models.Scopes Scopes
		{
			get { return _scopes; }
			private set
			{
				if (_scopes != value)
				{
					_scopes = value;

					var converter = new Models.ScopesConverter();

					scopesString = String.Join
					(
						" ",
						Enum.GetValues(typeof(Models.Scopes))
							.Cast<Models.Scopes>()
							.Except(new[] { Models.Scopes.None, })
							.Select((s) => converter.GetString(s))
					);
				}
			}
		}

		private IHttpRequester httpRequester;
		private IJsonDeserializer jsonDeserializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="StackExchangeClient"/> class, that is anonymous to the Stack Exchange API (i.e. unidentifiable without the client id, key, and access token).
		/// </summary>
		public StackExchangeClient()
		{
			PostConstructor();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StackExchangeClient" /> class, that the Stack Exchange API can identify.
		/// <para>Visit https://api.stackexchange.com/docs/authentication for more information about the authentication process, and its perks.</para>
		/// </summary>
		/// <param name="clientId">The client id of the application, as indicated on its edit page on StackApps.</param>
		/// <param name="key">The request key of the application, as indicated on its edit page on StackApps.</param>
		/// <param name="scopes">The permissions that this <see cref="StackExchangeClient" /> object has, to data on the Stack Exchange network.</param>
		/// <param name="accessToken">A custom access token.</param>
		/// <exception cref="ArgumentException">Thrown if the specified key is <c>null</c> or consists of only white space.</exception>
		public StackExchangeClient(int clientId, string key, Models.Scopes scopes, string accessToken = null)
		{
			ClientId = clientId;
			Key = !String.IsNullOrWhiteSpace(key) ? key.Trim() : throw new ArgumentException(nameof(key));
			Scopes = scopes;

			this.accessToken = accessToken;
			IsAnonymous = false;

			PostConstructor();
		}

		/// <summary>
		/// Initializes the <see cref="IHttpRequester"/> and <see cref="IJsonDeserializer"/> used by this <see cref="StackExchangeClient"/> object.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if the <see cref="IHttpRequester"/> or the <see cref="IJsonDeserializer"/> is null.</exception>
		private void PostConstructor()
		{
			httpRequester = CreateHttpRequester() ?? throw new ArgumentNullException($"A non-null {nameof(IHttpRequester)} is required.");
			jsonDeserializer = CreateJsonDeserializer()?? throw new ArgumentNullException($"A non-null {nameof(IJsonDeserializer)} is required.");
		}

		/// <summary>
		/// Creates the <see cref="IHttpRequester"/> used by this <see cref="StackExchangeClient"/> object, to perform HTTP requests to the Stack Exchange API.
		/// </summary>
		public virtual IHttpRequester CreateHttpRequester()
		{
			var httpClient = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip,
			});

			httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
			httpClient.DefaultRequestHeaders.Add("User-Agent", "RavinduL.SEStandard");

			return new HttpClientRequester(httpClient);
		}


		/// <summary>
		/// Creates the <see cref="IJsonDeserializer"/> used by this <see cref="StackExchangeClient"/> object, to deserialize JSON to objects.
		/// </summary>
		public virtual IJsonDeserializer CreateJsonDeserializer()
		{
			return new JsonDeserializer();
		}

		/// <summary>
		/// Performs an asynchronous HTTP request to the Stack Exchange API, with the specified data, and request method.
		/// </summary>
		/// <typeparam name="T">The type implementing <see cref="IStackExchangeModel"/>, to which the response gets deserialized to.</typeparam>
		/// <param name="path">The path relative to <see cref="ApiBaseUrl"/> to which the request should be made.</param>
		/// <param name="data">The data to be sent along with the HTTP request.</param>
		/// <param name="requestMethod">The HTTP method used for the request.</param>
		public async Task<Models.Wrapper<T>> PerformRequestAsync<T>(string path, Dictionary<string, string> data, HttpRequestMethod requestMethod)
			where T : IStackExchangeModel
		{
			if (data == null)
			{
				data = new Dictionary<string, string>();
			}

			if (!IsAnonymous)
			{
				data.Add("key", Key);

				if (HasAccessToken)
				{
					data.Add("access_token", accessToken);
				}
			}

			var url = ApiBaseUrl.AppendPathSegment(path);

			var response = await httpRequester.PerformRequestAsync(url, data, requestMethod).ConfigureAwait(false);

			return jsonDeserializer.Deserialize<Models.Wrapper<T>>(response.Content);
		}

		/// <summary>
		/// Releases managed and unmanaged resources used by this <see cref="StackExchangeClient"/> object.
		/// </summary>
		public void Dispose()
		{
			httpRequester.Dispose();
		}

		private void ThrowIfAnonymous()
		{
			if (IsAnonymous)
			{
				throw new InvalidOperationException($"The operation cannot be performed with an anonymous {nameof(StackExchangeClient)} object.");
			}
		}

		/// <summary>
		/// Gets the URL to which the user should be directed to, to authenticate this <see cref="StackExchangeClient"/> object via implicit authentication.
		/// <para>Visit https://api.stackexchange.com/docs/authentication for more information.</para>
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if this <see cref="StackExchangeClient"/> object is anonymous.</exception>
		public Uri GetImplicitAuthenticationUrl()
		{
			ThrowIfAnonymous();

			return new Uri(ImplicitAuthenticationPromptUrl.SetQueryParams(new Dictionary<string, string>
			{
				["client_id"] = ClientId.ToString(),
				["scope"] = scopesString,
				["redirect_uri"] = ImplicitAuthenticationRedirectUrl,
			}));
		}

		/// <summary>
		/// Attempts to complete the implicit authentication flow with the specified URL.
		/// <para>If the app was successfully authenticated, it will acquire an access token and the <see cref="HasAccessToken"/> method will return <c>true</c>.</para>
		/// </summary>
		public ImplicitAuthenticationResult TryImplicitAuthentication(Uri uri)
		{
			ThrowIfAnonymous();

			var url = new Url(uri.ToString());

			if (url.Path == ImplicitAuthenticationRedirectUrl || url.Path == ImplicitAuthenticationRedirectUrl + "/")
			{
				// The URL should be of the form $"{ImplicitAuthenticationDestinationUrl}#key=value&key=value"

				if (!String.IsNullOrWhiteSpace(url.Fragment))
				{
					// Replaces the '#' that separates the fragment of the URL with a '?', converting the data contained within the fragment into query parameters.
					var arr = url.ToString().ToCharArray();
					arr[url.ToString().IndexOf('#')] = '?';
					url = new Url(new String(arr));

					const string accessTokenKey = "access_token";
					const string expiresKey = "expires";
					const string errorKey = "error";
					const string errorDescriptionKey = "error_description";

					if (url.QueryParams.ContainsKey(accessTokenKey))
					{
						accessToken = url.QueryParams[accessTokenKey].ToString();

						int? expires = null;

						if (url.QueryParams.ContainsKey(expiresKey))
						{
							Int32.TryParse(url.QueryParams[expiresKey].ToString(), out var _expires);
							expires = _expires;
						}

						return ImplicitAuthenticationResult.ForSuccess(accessToken, expires);
					}
					else if (url.QueryParams.ContainsKey(errorKey))
					{
						var error = url.QueryParams[errorKey].ToString();
						var description = url.QueryParams.ContainsKey(errorDescriptionKey) ? url.QueryParams[errorDescriptionKey].ToString() : null;

						return ImplicitAuthenticationResult.ForFailure(error, description);
					}
				}
			}

			return ImplicitAuthenticationResult.ForIgnore();
		}
	}
}
