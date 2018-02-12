namespace RavinduL.SEStandard
{
	using Newtonsoft.Json;

	/// <summary>
	/// Converts an enum to its <see cref="string"/> representation and vice-versa.
	/// </summary>
	/// <seealso cref="JsonConverter" />
	public abstract class EnumConverter : JsonConverter
	{
		protected EnumConverter()
		{
		}

		/// <summary>
		/// Gets the string representation of the specfied enum.
		/// </summary>
		public abstract string GetString(object o);

		/// <summary>
		/// Gets the enum representation of the specified string.
		/// </summary>
		public abstract object GetEnum(string s);
	}
}