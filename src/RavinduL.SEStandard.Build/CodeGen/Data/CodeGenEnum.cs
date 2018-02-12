namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public class CodeGenEnum
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("flags")]
		public bool Flags { get; set; }

		[JsonProperty("values")]
		public IEnumerable<CodeGenEnumValue> Values { get; set; }
	}
}
