namespace RavinduL.SEStandard.Build.CodeGen.Tasks
{
	using Cake.Core.Diagnostics;
	using Cake.Common.Diagnostics;
	using Cake.Common.IO;
	using Cake.Frosting;

	/// <summary>
	/// The second step of code generation.
	/// <para>The output directories of the entities to be generated are cleaned.</para>
	/// </summary>
	/// <seealso cref="FrostingTask{T}" />
	/// <seealso cref="PrepareTask" />
	[TaskName("CodeGen/Clean")]
	[Dependency(typeof(PrepareTask))]
	public sealed class CleanTask : FrostingTask<Context>
	{
		public override void Run(Context context)
		{
			foreach (var (entity, path) in PrepareTask.Outputs)
			{
				if (context.WhatIf)
				{
					// The context.CleanDirectory method logs message to the console.
					context.Information($"Cleaning directory: {path}");
				}
				else
				{
					context.CleanDirectory(path);
				}
			}
		}
	}
}
