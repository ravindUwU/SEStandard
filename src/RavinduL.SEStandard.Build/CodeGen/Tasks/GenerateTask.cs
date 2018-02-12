namespace RavinduL.SEStandard.Build.CodeGen.Tasks
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Cake.Common.Diagnostics;
	using Cake.Frosting;
	using HandlebarsDotNet;
	using Newtonsoft.Json;
	using RavinduL.SEStandard.Build.CodeGen.Data;

	/// <summary>
	/// The third step of code generation.
	/// <para>Files that correspond to the selected entities are generated.</para>
	/// </summary>
	/// <seealso cref="FrostingTask{T}" />
	/// <seealso cref="CleanTask"/>
	/// <seealso cref="PrepareTask"/>
	[TaskName("CodeGen/Generate")]
	[Dependency(typeof(CleanTask))]
	public sealed class GenerateTask : FrostingTask<Context>
	{
		public override void Run(Context context)
		{
			if (PrepareTask.Outputs?.Count == 0)
			{
				return;
			}

			context.Information($"Generating caches...");
			GenerateCaches(context);

			context.Information($"Generating files...");
			GenerateFiles(context);
		}

		public IEnumerable<CodeGenEnum> EnumCache;
		public IEnumerable<CodeGenClass> ClassCache;
		public IEnumerable<CodeGenEndpoint> MethodCache;

		/// <summary>
		/// Generates caches required to generate files for all <see cref="PrepareTask.Outputs"/>.
		/// </summary>
		private void GenerateCaches(Context context)
		{
			// Aggregates the individual outputs' keys to a single CodeGenEntities bit field.
			var outputEntities = PrepareTask.Outputs
				.Select((kvp) => kvp.Key)
				.Aggregate((current, next) => current |= next);
			
			// Separates the bit field above into an IEnumerable<CodeGenEntities> to be iterated over and cached individually.
			var entities = Enum.GetValues(typeof(CodeGenEntities))
				.Cast<CodeGenEntities>()
				.Where((e) => outputEntities.HasFlag(e))
				.Except(new[] { CodeGenEntities.None, CodeGenEntities.All });

			context.Information($"Caches will be generated for: {String.Join(", ", entities)}");

			foreach (var entity in entities)
			{
				context.Information($"Generating caches for: {entity}");

				var jsonPath = context.BuildProject.Path.GetDirectory().Combine("CodeGen/Data/Sets").CombineWithFilePath($"{entity}.json");
				context.Verbose($"Reading: {jsonPath}");

				var json = new Lazy<string>(() => File.ReadAllText(jsonPath.ToString()));

				void PopulateAndFix<T>(ref IEnumerable<T> field, Action fix)
				{
					if (field == null)
					{
						field = JsonConvert.DeserializeObject<IEnumerable<T>>(json.Value);
						fix();
					}
				}

				switch (entity)
				{
					case CodeGenEntities.Enums:
						{
							PopulateAndFix(ref EnumCache, FixEnums);
							break;
						}

					case CodeGenEntities.Classes:
						{
							PopulateAndFix(ref ClassCache, FixClasses);
							break;
						}

					case CodeGenEntities.Methods:
						{
							PopulateAndFix(ref MethodCache, FixMethods);
							break;
						}
				}
			}
		}

		/// <summary>
		/// Generates files for all <see cref="PrepareTask.Outputs"/>.
		/// </summary>
		private void GenerateFiles(Context context)
		{
			foreach (var (entity, outputPath) in PrepareTask.Outputs)
			{
				context.Information($"Generating: {entity}");

				var templatePath = context.BuildProject.Path.GetDirectory().Combine("CodeGen/Data/Templates").CombineWithFilePath($"{entity}.hbs");
				var template = Handlebars.Compile(File.ReadAllText(templatePath.ToString()));

				void GenerateFromTemplateWithCollection<T>(IEnumerable<T> collection, Func<T, string> fileNameSelector)
				{
					foreach (var item in collection)
					{
						var text = template(item);
						var path = outputPath.CombineWithFilePath($"{fileNameSelector(item)}.cs").ToString().Replace('<', '`').Replace(">", "");

						context.Information($"Writing: {path}");

						if (!context.WhatIf)
						{
							File.WriteAllText(path, text);
						}
					}
				}

				switch (entity)
				{
					case CodeGenEntities.Enums:
						{
							GenerateFromTemplateWithCollection(EnumCache, (e) => e.Name);
							break;
						}

					case CodeGenEntities.Classes:
						{
							GenerateFromTemplateWithCollection(ClassCache, (c) => c.Name);
							break;
						}

					case CodeGenEntities.Methods:
						{
							GenerateFromTemplateWithCollection(MethodCache, (m) => m.Name + "Endpoint");

							var newTemplate = Handlebars.Compile(File.ReadAllText(templatePath.GetDirectory().CombineWithFilePath("InitializeEndpoints.hbs").ToString()));
							var path = outputPath.CombineWithFilePath($"StackExchangeClient.InitializeEndpoints.cs").ToString();

							context.Information($"Writing: {path}");

							if (!context.WhatIf)
							{
								File.WriteAllText(path, newTemplate(MethodCache));
							}

							break;
						}

					case CodeGenEntities.EnumConversionTests:
						{
							var path = outputPath.CombineWithFilePath($"{nameof(CodeGenEntities.EnumConversionTests)}.cs").ToString();
							var text = template(EnumCache);

							context.Information($"Writing: {path}");

							if (!context.WhatIf)
							{
								File.WriteAllText(path, text);
							}
							break;
						}
				}
			}
		}

		private void FixMethods()
		{
			foreach (var g in MethodCache)
			{
				foreach (var m in g.Methods)
				{
					if (!m.Auth)
					{
						m.Auth = m.Scopes?.Count() > 0;
					}

					if (m.Paths == null)
					{
						m.Paths = new CodeGenMethodPath[0];
					}

					if (m.Queries == null)
					{
						m.Queries = new List<CodeGenMethodQuery>();
					}

					if (!m.IsNetworkMethod)
					{
						m.Queries.Add(new CodeGenMethodQuery { Name = "site", Type = "string" });
					}

					if (m.IsPaged)
					{
						m.Queries.AddRange(new[]
						{
							new CodeGenMethodQuery { Name = "page", Type = "int?", Default = "null" },
							new CodeGenMethodQuery { Name = "pagesize", Type = "int?", Default = "null" },
						});
					}

					if (m.IsDated)
					{
						m.Queries.AddRange(new[]
						{
							new CodeGenMethodQuery { Name = "fromdate", Type = "DateTime?", Default = "null" },
							new CodeGenMethodQuery { Name = "todate", Type = "DateTime?", Default = "null" },
						});
					}

					if (!String.IsNullOrWhiteSpace(m.Sort))
					{
						m.Queries.AddRange(new[]
						{
							new CodeGenMethodQuery { Name = "sort", Type = $"{m.Sort}?", Default = "null" },
							new CodeGenMethodQuery { Name = "order", Type = "Order?", Default = "null" },
							new CodeGenMethodQuery { Name = "min", Type = "object", Default = "null" },
							new CodeGenMethodQuery { Name = "max", Type = "object", Default = "null" },
						});
					}

					if (m.IsPreviewable)
					{
						m.Queries.Add(new CodeGenMethodQuery { Name = "preview", Type = "bool?", Default = "null" });
					}

					m.Queries.Add(new CodeGenMethodQuery { Name = "filter", Type = "string", Default = "null" });
				}
			}
		}

		private void FixClasses()
		{
			foreach (var c in ClassCache)
			{
				if (c.Suffix == null)
				{
					c.Suffix = ": IStackExchangeModel";
				}

				foreach (var p in c.Properties)
				{
					if (String.IsNullOrWhiteSpace(p.Name))
					{
						p.Name = CodeGenHelpers.Prettify(p.JsonName);
					}

					p.IsEnum = EnumCache.Any((e) => e.Name == p.Type);
				}
			}
		}

		private void FixEnums()
		{
			foreach (var e in EnumCache)
			{
				foreach (var v in e.Values)
				{
					if (String.IsNullOrEmpty(v.Name))
					{
						v.Name = CodeGenHelpers.Prettify(v.JsonName);
					}
				}

				CodeGenHelpers.NonNullableTypes.Add(e.Name);
			}
		}
	}
}
