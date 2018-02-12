namespace RavinduL.SEStandard.Serialization
{
	using Newtonsoft.Json;

	/// <summary>
	/// Uses Json.NET to deserialize a JSON string to an object.
	/// </summary>
	/// <seealso cref="IJsonDeserializer" />
	public sealed class JsonDeserializer : IJsonDeserializer
	{
		/// <summary>
		/// Deserializes the specified string of JSON into an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the specified JSON to.</typeparam>
		/// <param name="json">The JSON to be deserialized.</param>
		public T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
