namespace RavinduL.SEStandard.Build
{
	using Cake.Core;
	using Cake.Frosting;

	public class Program : IFrostingStartup
	{
		public static int Main(string[] args)
		{
			return new CakeHostBuilder()
				.WithArguments(args)
				.UseStartup<Program>()
				.Build()
				.Run();
		}

		public void Configure(ICakeServices services)
		{
			services.UseContext<Context>();
			services.UseLifetime<Lifetime>();
			services.UseWorkingDirectory(".");
		}
	}
}
