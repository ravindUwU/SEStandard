namespace RavinduL.SEStandard.Build.CodeGen.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Newtonsoft.Json;

	public class CodeGenMethod
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("returnType")]
		public string ReturnType { get; set; }

		[JsonProperty("post")]
		public bool Post { get; set; }

		[JsonProperty("isPaged")]
		public bool IsPaged { get; set; }

		[JsonProperty("isDated")]
		public bool IsDated { get; set; }

		[JsonProperty("isNetworkMethod")]
		public bool IsNetworkMethod { get; set; }

		[JsonProperty("isPreviewable")]
		public bool IsPreviewable { get; set; }

		[JsonProperty("sort")]
		public string Sort { get; set; }

		[JsonProperty("auth")]
		public bool Auth { get; set; }

		[JsonProperty("scopes")]
		public IEnumerable<CodeGenMethodScope> Scopes { get; set; }

		[JsonProperty("paths")]
		public IEnumerable<CodeGenMethodPath> Paths { get; set; }

		[JsonProperty("queries")]
		public List<CodeGenMethodQuery> Queries { get; set; }

		[JsonIgnore]
		public bool HasDescriptionOrUrl => !String.IsNullOrWhiteSpace(Description) || !String.IsNullOrWhiteSpace(Url);

		/// <summary>
		/// All paths, and queries without default values, each in the form of $"{Type} {Identifier}".
		/// </summary>
		[JsonIgnore]
		public IEnumerable<string> ParametersWithoutDefaults
			=> Paths
			.Select((p) => $"{p.Type} {p.EscapedName}")
			.Concat
			(
				Queries
					.Where((q) => String.IsNullOrWhiteSpace(q.Default))
					.Select((q) => $"{q.Type} {q.EscapedName}")
			);

		/// <summary>
		/// All queries with default values, each in the form of <c>"{Type} {Identifier} = {Value}"</c>.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<string> ParametersWithDefaults
			=> Queries
				.Where((q) => !String.IsNullOrWhiteSpace(q.Default))
				.Select((q) => $"{q.Type} {q.EscapedName} = {q.Default}");

		/// <summary>
		/// <see cref="ParametersWithoutDefaults"/> followed by <see cref="ParametersWithDefaults"/>, as they should be in method signatures.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<string> Parameters
			=> ParametersWithoutDefaults.Concat(ParametersWithDefaults);

		[JsonIgnore]
		public IEnumerable<string> ParametersThatShouldntBeNull
			=> Paths
			.Where((p) => p.ShouldntBeNull)
			.Select((p) => p.EscapedName)
			.Concat
			(
				Queries
					.Where((q) => q.ShouldntBeNull)
					.Select((q) => q.EscapedName)
			);

		/// <summary>
		/// Paths and queries that have descriptions, each in the form of an object containing their name and description.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<object> ParametersWithDescriptions
			=> Paths
			.Where((p) => !String.IsNullOrWhiteSpace(p.Description))
			.Select((p) => new { Name = p.Name, Description = p.Description, })
			.Concat
			(
				Queries
					.Where((q) => !String.IsNullOrWhiteSpace(q.Description))
					.Select((q) => new { Name = q.Name, Description = q.Description, })
			);
	}
}
