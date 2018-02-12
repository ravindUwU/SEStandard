namespace RavinduL.SEStandard.Build.CodeGen.Tasks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Cake.Common;
	using Cake.Common.Diagnostics;
	using Cake.Common.Solution;
	using Cake.Core.IO;
	using Cake.Frosting;
	using HandlebarsDotNet;

	/// <summary>
	/// The first step of code generation. 
	/// <para>Entities specified via the command line are parsed, and the output directories of each, and the Handlebars.Net compiler, are configured.</para>
	/// </summary>
	/// <seealso cref="FrostingTask{T}" />
	[TaskName("CodeGen/Prepare")]
	public sealed class PrepareTask : FrostingTask<Context>
	{
		/// <summary>
		/// A <see cref="Dictionary{TKey, TValue}"/> specifying <see cref="CodeGenEntities"/> to be generated, and the corresponding <see cref="SolutionProject"/>s to which the generated content of each should be output.
		/// </summary>
		public static Dictionary<CodeGenEntities, DirectoryPath> Outputs { get; private set; }

		public override void Run(Context context)
		{
			var entitiesArgument = context.Argument("Entities", "");

			context.Verbose($"Parsing `-Entities` argument: \"{entitiesArgument}\"");

			// Selects all valid items out of those that are specified, discarding invalid items.
			var specifiedEntities = entitiesArgument
				.Split(',')
				.Select((s) => Enum.TryParse<CodeGenEntities>(s.Trim(), out var e) ? e as CodeGenEntities? : null)
				.Where((e) => e != null && e != CodeGenEntities.None)
				.Cast<CodeGenEntities>()
				.ToHashSet();

			if (specifiedEntities.Contains(CodeGenEntities.All))
			{
				specifiedEntities = Enum.GetValues(typeof(CodeGenEntities))
					.Cast<CodeGenEntities>()
					.Except(new[] { CodeGenEntities.None, CodeGenEntities.All })
					.ToHashSet();
			}

			if (specifiedEntities.Any())
			{
				context.Verbose($"Entities specified to be generated are: {String.Join(", ", specifiedEntities)}");

				var dictionary = new Dictionary<CodeGenEntities, SolutionProject>
				{
					[CodeGenEntities.Enums] = context.MainProject,
					[CodeGenEntities.Classes] = context.MainProject,
					[CodeGenEntities.Methods] = context.MainProject,
					[CodeGenEntities.EnumConversionTests] = context.TestProject,
				};

				Outputs = dictionary
					.Where((kvp) => specifiedEntities.Contains(kvp.Key))
					.ToDictionary
					(
						(kvp) => kvp.Key,
						(kvp) => kvp.Value.Path.GetDirectory().Combine(CodeGenHelpers.CodeGenFolderName).Combine(kvp.Key.ToString())
					);

				// Handlebars configuration.

				Handlebars.Configuration.TextEncoder = new PlainTextEncoder();

				Handlebars.RegisterHelper("ifIsInt", (writer, options, handlebarsContext, arguments) =>
				{
					if (arguments.Length == 1)
					{
						(arguments[0] is int ? options.Template : options.Inverse)(writer, (object)handlebarsContext);
					}
					else
					{
						throw new ArgumentException("The ifIsInt helper takes exactly one argument.");
					}
				});

				Handlebars.RegisterHelper("newline", (writer, handlebarsContext, arguments) =>
				{
					writer.WriteLine("");
				});
			}
			else
			{
				var validValues = Enum.GetValues(typeof(CodeGenEntities))
					.Cast<CodeGenEntities>()
					.Except(new[] { CodeGenEntities.None });

				throw new Exception($"Please specify the entities to be generated via the `-Entities` argument. Valid values are {String.Join(", ", validValues)} (individually, or as a comma-delimited list)");
			}
		}
	}
}
