namespace RavinduL.SEStandard.Build.CodeGen
{
	using HandlebarsDotNet;

	/// <summary>
	/// An <see cref="ITextEncoder"/> that doesn't transform the text in any way.
	/// <para>This is necessary because the default <see cref="ITextEncoder"/> used by Handlebars.Net is suited for HTML transformations, and escapes characters such as '&lt;' and '&gt;'.</para>
	/// </summary>
	/// <seealso cref="ITextEncoder" />
	public class PlainTextEncoder : ITextEncoder
	{
		public string Encode(string value)
		{
			return value;
		}
	}
}
