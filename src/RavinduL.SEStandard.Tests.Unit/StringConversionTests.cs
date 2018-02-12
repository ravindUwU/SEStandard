namespace RavinduL.SEStandard.Tests.Unit
{
	using System;
	using RavinduL.SEStandard.Endpoints;
	using Xunit;

	public partial class StringConversionTests
	{
		private class CustomEndpoint : Endpoint
		{
			public CustomEndpoint() : base(new StackExchangeClient())
			{
			}

			public new string ConvertToString(object o)
			{
				return base.ConvertToString(o);
			}
		}

		private CustomEndpoint endpoint = new CustomEndpoint();

		[Fact]
		public void ConvertToString_ReturnsNulls()
		{
			var actual = endpoint.ConvertToString(null);

			Assert.Null(actual);
		}

		[Fact]
		public void ConvertToString_ReturnsStrings()
		{
			const string s = "a string";

			var actual = endpoint.ConvertToString(s);
			var expected = s;

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ConvertToString_TrimsStringsUponReturning()
		{
			const string s = "\r\n\t whitespace on either side \r\n\t";

			var actual = endpoint.ConvertToString(s);
			var expected = "whitespace on either side";

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ConvertToString_ReturnsNullOnEmptyOrWhitespaceString()
		{
			Assert.Null(endpoint.ConvertToString(""));
			Assert.Null(endpoint.ConvertToString(" \r\n\t"));
		}

		[Fact]
		public void ConvertToString_ConvertsIntsToStrings()
		{
			foreach (var i in new[] { -1, 1 })
			{
				Assert.Equal(i.ToString(), endpoint.ConvertToString(i));
			}
		}

		[Fact]
		public void ConvertToString_ConvertsDateTimesToUnixTime()
		{
			// 0, the Unix Epoch.
			Assert.Equal("0", endpoint.ConvertToString(new DateTime(1970, 1, 1)));

			// A random date: 28th of September, 2000
			Assert.Equal("970099200", endpoint.ConvertToString(new DateTime(2000, 9, 28)));
		}

		[Fact]
		public void ConvertToString_ConvertsBoolsToTitleCasedTrueOrFalse()
		{
			Assert.Equal("True", endpoint.ConvertToString(true));
			Assert.Equal("False", endpoint.ConvertToString(false));
		}

		[Fact]
		public void ConvertToString_ConvertsGuidsToStrings()
		{
			var guid = new Guid();

			var expected = "00000000-0000-0000-0000-000000000000";
			var actual = endpoint.ConvertToString(guid);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ConvertToString_ConvertsAlphabeticalCharactersInGuidsToUppercase()
		{
			var guid = new Guid(new String('a', 32));

			var expected = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA";
			var actual = endpoint.ConvertToString(guid);

			Assert.Equal(expected, actual);
		}
	}
}
