namespace RavinduL.SEStandard.Build
{
	using Cake.Common.Solution;
	using Cake.Core;
	using Cake.Frosting;

	public class Context : FrostingContext
	{
		public Context(ICakeContext context) : base(context)
		{
		}

		public SolutionProject MainProject { get; set; }
		public SolutionProject BuildProject { get; set; }
		public SolutionProject TestProject { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if the effects of tasks are displayed, instead of the tasks being executed.
		/// </summary>
		public bool WhatIf { get; set; }
	}
}
