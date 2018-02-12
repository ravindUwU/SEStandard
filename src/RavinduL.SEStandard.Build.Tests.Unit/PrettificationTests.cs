namespace RavinduL.SEStandard.Build.Tests.Unit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using RavinduL.SEStandard.Build.CodeGen;
	using Xunit;

	public class PrettificationTests
	{
		[Fact]
		public void Prettify_CapitalizesFirstLetter()
		{
			var actual = CodeGenHelpers.Prettify("prettify");

			Assert.True(actual[0] == 'P');
		}

		[Fact]
		public void Prettify_RemovesUnderscores()
		{
			var actual = CodeGenHelpers.Prettify("prettify_this_string");

			Assert.True(!actual.Any((c) => c == '_'));
		}

		[Fact]
		public void Prettify_CapitalizesLettersFollowingUnderscores()
		{
			var s = "prettify_this_string";

			var indexes = new HashSet<int>();
			var underscoreCount = 0;

			for (int i = 0; i < s.Length - 1; i++)
			{
				if (s[i] == '_')
				{
					if (Char.IsLower(s[i + 1]))
					{
						indexes.Add(i - underscoreCount);
					}

					++underscoreCount;
				}
			}

			var prettifiedString = CodeGenHelpers.Prettify(s);

			var actual = prettifiedString.Skip(1).Where((c) => Char.IsUpper(c));
			var expected = indexes.Select((index) => prettifiedString[index]);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Prettify_PreservesCapitalizationOfAlreadyCapitalizedLetters()
		{
			var s = "pReTtIfY";

			var expected = s.Where((c) => Char.IsUpper(c));
			var actual = CodeGenHelpers.Prettify(s).Skip(1).Where((c) => Char.IsUpper(c));

			Assert.Equal(expected, actual);
		}
	}
}
