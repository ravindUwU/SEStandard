namespace RavinduL.SEStandard
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Specifies the <see cref="EnumConverter"/> used for converting <c>enum</c> values to their <see cref="string"/> representations and vice-versa.
	/// </summary>
	/// <seealso cref="Attribute" />
	public sealed class EnumConverterAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumConverterAttribute"/> class.
		/// </summary>
		/// <param name="converterType">Type of the <see cref="EnumConverter"/>.</param>
		/// <exception cref="ArgumentException">Thrown if the specified type doesn't derive from the <see cref="EnumConverter"/> class.</exception>
		public EnumConverterAttribute(Type converterType)
		{
			if (converterType?.GetTypeInfo().IsSubclassOf(typeof(EnumConverter)) ?? false)
			{
				ConverterType = converterType;
			}
			else
			{
				throw new ArgumentException($"The specified type should derive from the {nameof(EnumConverter)} class.");
			}
		}

		/// <summary>
		/// Gets the type of the <see cref="EnumConverter"/> used for converting enums to their <see cref="string"/> representations and vice-versa.
		/// </summary>
		public Type ConverterType { get; }

		/// <summary>
		/// Attempts to get the <see cref="EnumConverterAttribute"/> associated with the specified <see cref="object"/>.
		/// </summary>
		/// <param name="o">The <see cref="object"/> to attempt to retrieve the <see cref="EnumConverterAttribute"/> of.</param>
		/// <param name="attribute">The <c>out</c> variable that the <see cref="EnumConverterAttribute"/> associated with the specified object will be assigned to.</param>
		/// <returns><c>true</c> if the specified object has an <see cref="EnumConverterAttribute"/> associated with it, otherwise, <c>false</c>.</returns>
		public static bool TryGetForObject(object o, out EnumConverterAttribute attribute)
		{
			attribute = o.GetType().GetTypeInfo().GetCustomAttribute<EnumConverterAttribute>();
			return attribute != null;
		}
	}
}
