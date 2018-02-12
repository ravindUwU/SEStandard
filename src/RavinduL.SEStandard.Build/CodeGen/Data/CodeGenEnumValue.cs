namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using Newtonsoft.Json;

	public class CodeGenEnumValue
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("jsonName")]
		public string JsonName { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("value")]
		public int? Value { get; set; }
	}
}
