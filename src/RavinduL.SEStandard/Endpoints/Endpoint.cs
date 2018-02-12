namespace RavinduL.SEStandard.Endpoints
{
	using System;
	using System.Collections;
	using System.Linq;

	/// <summary>
	/// Groups methods of the Stack Exchange API by shared interest.
	/// </summary>
	public abstract class Endpoint
	{
		protected readonly StackExchangeClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Endpoint"/> class.
		/// </summary>
		/// <param name="client">The <see cref="StackExchangeClient"/> to perform requests via.</param>
		/// <exception cref="ArgumentNullException">client</exception>
		protected Endpoint(StackExchangeClient client)
		{
			this.client = client ?? throw new ArgumentNullException(nameof(client));
		}

		private DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Converts an object to its <see cref="string"/> representation to be sent with requests to the Stack Exchange API.
		/// </summary>
		protected string ConvertToString(object o)
		{
			if (o != null)
			{
				// If the object is an IEnumerable, converts each element of it to its string representation via the `ConvertToString` method, delimiting them with a single comma.
				// Although `(IEnumerable<SomeEnum> as object) is IEnumerable<int>`, its conversion will work because each `SomeEnum` is individually converted.
				// `SomeString is IEnumerable<char>`, and therefore will have to be guarded against, here.
				if (o is IEnumerable ie && !(o is string))
				{
					return String.Join
					(
						",",
						ie.Cast<object>()
							.Select((obj) => ConvertToString(obj))
							.OfType<string>()
							.Distinct()
					);
				}

				switch (o)
				{
					case int i:
						return i.ToString();

					case bool b:
						return b.ToString();

					case string s when !String.IsNullOrWhiteSpace(s):
						return s.Trim();

					case DateTime d:
						// Converts a DateTime to unix time.
						// "Dates are [...] guaranteed to fit in a signed 64-bit integer."
						// https://api.stackexchange.com/docs/numbers
						return ((long)d.Subtract(unixEpoch).TotalSeconds).ToString();

					case Guid g:
						return g.ToString().ToUpper();
				}

				if (EnumConverterAttribute.TryGetForObject(o, out var attribute))
				{
					var converter = (EnumConverter)Activator.CreateInstance(attribute.ConverterType);
					return converter.GetString(o);
				}
			}

			return null;
		}
	}
}
