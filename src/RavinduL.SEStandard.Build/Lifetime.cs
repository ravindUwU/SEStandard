namespace RavinduL.SEStandard.Build
{
	using System.Linq;
	using Cake.Common;
	using Cake.Common.Solution;
	using Cake.Frosting;

	public sealed class Lifetime : FrostingLifetime<Context>
	{
		/// <summary>
		/// The unique identifier of [.NET Core projects].
		/// <para>Sourced from: https://github.com/dotnet/project-system/blob/7b41c695866c16e121cc87914d26feadad139f3f/src/Microsoft.VisualStudio.ProjectSystem.CSharp.VS/Packaging/CSharpProjectSystemPackage.cs#L38 </para>
		/// </summary>
		private const string ProjectTypeGuid = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";

		/// <summary>
		/// This method is executed before any tasks are run. If setup fails, no tasks will be executed but teardown will be performed.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Setup(Context context)
		{
			var solutionPath = context.Environment.WorkingDirectory.Combine("../..").Collapse().CombineWithFilePath("RavinduL.SEStandard.sln");

			var projects = context.ParseSolution(solutionPath).Projects
				.Where((p) => p.Type == "{" + ProjectTypeGuid + "}");

			context.MainProject = projects.Where((p) => p.Name == "RavinduL.SEStandard").First();
			context.BuildProject = projects.Where((p) => p.Name == "RavinduL.SEStandard.Build").First();
			context.TestProject = projects.Where((p) => p.Name == "RavinduL.SEStandard.Tests.Unit").First();

			context.WhatIf = context.Argument("WhatIf", false);
		}
	}
}
