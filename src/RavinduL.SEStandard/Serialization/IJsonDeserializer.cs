namespace RavinduL.SEStandard.Serialization
{
	/// <summary>
	/// Provides methods for deserializing a string of JSON to an object.
	/// </summary>
	public interface IJsonDeserializer
	{
		/// <summary>
		/// Deserializes the specified string of JSON into an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the specified JSON to.</typeparam>
		/// <param name="json">The JSON to be deserialized.</param>
		T Deserialize<T>(string json);
	}
}