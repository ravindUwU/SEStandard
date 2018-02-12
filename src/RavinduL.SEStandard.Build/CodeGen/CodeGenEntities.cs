namespace RavinduL.SEStandard.Build.CodeGen
{
	using System;

	/// <summary>
	/// Specifies the entities for that should be generated.
	/// <para>Note that this enumeration is implemented as a bit field, in which entity dependencies (for example, the generation of <see cref="Classes"/> requires awareness of the available <see cref="Enums"/>) are mapped.</para>
	/// </summary>
	[Flags]
#pragma warning disable CA2217 // Do not mark enums with FlagsAttribute
	public enum CodeGenEntities
#pragma warning restore CA2217 // Do not mark enums with FlagsAttribute
	{
		Enums = 1,
		Classes = 2 | Enums,
		Methods = 4 | Enums,
		EnumConversionTests = 8 | Enums,

		None = 0,
		All = Enums | Classes | Methods | EnumConversionTests,
	}
}
