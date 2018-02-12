namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public class CodeGenEndpoint
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("methods")]
		public IEnumerable<CodeGenMethod> Methods { get; set; }
	}
}
