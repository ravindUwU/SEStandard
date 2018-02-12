namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using Newtonsoft.Json;
	public class CodeGenClassProperty
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("jsonName")]
		public string JsonName { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonIgnore]
		public bool IsEnum { get; set; }
	}
}
