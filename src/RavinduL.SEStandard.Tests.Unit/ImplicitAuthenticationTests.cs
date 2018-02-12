namespace RavinduL.SEStandard.Tests.Unit
{
	using System;
	using Xunit;

	public class ImplicitAuthenticationTests
	{
		[Fact]
		public void GetImplicitAuthenticationUrl_ThrowsIfAnonymous()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var client = new StackExchangeClient();
				client.GetImplicitAuthenticationUrl();
			});
		}

		[Fact]
		public void TryImplicitAuthentication_ThrowsIfAnonymous()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var client = new StackExchangeClient();
				client.TryImplicitAuthentication(new Uri("https://www.example.com/"));
			});
		}
	}
}
