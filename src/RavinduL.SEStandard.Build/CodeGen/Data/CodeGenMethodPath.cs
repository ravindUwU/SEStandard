namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using Newtonsoft.Json;

	public class CodeGenMethodPath
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonIgnore]
		public string EscapedName => CodeGenHelpers.EscapeName(Name);

		/// <summary>
		/// A path should be null-checked if it's of a nullable type.
		/// </summary>
		[JsonIgnore]
		public bool ShouldntBeNull => !CodeGenHelpers.NonNullableTypes.Contains(Type);
	}
}
