namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public class CodeGenClass
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("suffix")]
		public string Suffix { get; set; }

		[JsonProperty("properties")]
		public IEnumerable<CodeGenClassProperty> Properties { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonIgnore]
		public bool HasDescriptionOrUrl => !String.IsNullOrWhiteSpace(Description) || !String.IsNullOrWhiteSpace(Url);
	}
}
