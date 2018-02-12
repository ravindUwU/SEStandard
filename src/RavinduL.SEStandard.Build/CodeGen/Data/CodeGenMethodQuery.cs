namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using System;
	using Newtonsoft.Json;

	public class CodeGenMethodQuery
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("default")]
		public string Default { get; set; }

		[JsonIgnore]
		public string EscapedName => CodeGenHelpers.EscapeName(Name);

		/// <summary>
		/// A query should be null checked if it's of a nullable type and it doesn't have a default (queries with defaults are optional).
		/// </summary>
		[JsonIgnore]
		public bool ShouldntBeNull => String.IsNullOrWhiteSpace(Default) && !CodeGenHelpers.NonNullableTypes.Contains(Type);
	}
}
