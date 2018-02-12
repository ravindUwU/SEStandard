# Contributing

Topics:
[Code of Conduct](#code-of-conduct) |
[Repository Layout](#repository-layout) |
[The Build Project](#the-build-project) |
[Code Generation](#code-generation) |
[Building the Library](#building-the-library)

<br>

## Code of Conduct

All participants and contributors to this project are expected to adhere to the guidelines of the code of conduct located [here](https://github.com/RavinduL/Meta/blob/master/CODE_OF_CONDUCT.md).

<br>

## Repository Layout

-	The following C# projects are located within the `src` folder of the root of the repository,

	Project                                | Description
	-------------------------------------- | -----------
	`RavinduL.SEStandard`                  | The 'main project', which compiles to the SEStandard library.
	`RavinduL.SEStandard.Tests.Unit`       | The 'test' project' that contains unit tests for the main project.
	`RavinduL.SEStandard.Build`            | The 'build project', which uses [Frosting][frosting] to perform tasks such as code generation, for the projects above.
	`RavinduL.SEStandard.Tests.Build.Unit` | Unit tests for the build project.
	`Sandbox`                              | A .NET Core console application for the purpose of running snippets of code.

- The `RavinduL.SEStandard.sln` Visual Studio solution located at the root of the repository references all C# projects mentioned above.

- The [`Cake.ps1`][f_cakeps1] build script located at the root of the repository can be used to easily build and run the build project.

<br>

## The Build Project

The build project is a .NET Core console application that utilizes [`Frosting`][frosting] to execute [`Cake`](https://cakebuild.net/) tasks (that perform code generation, etc.).

The [`Cake.ps1`][f_cakeps1] script located at the root of the repository eases the process of running the build project.

```Text
PS> Cake.ps1 [-Target <String>] [-WhatIf] ...
```

- `-Target`: The name of the `Frosting` task to invoke.

- `-WhatIf`: Displays the effects of the task instead of executing them.

<br>

## Code Generation

### Entities

The build project uses the [Handlebars.Net](https://github.com/rexm/Handlebars.Net) library to generate code via [Handlebars](http://handlebarsjs.com/) templates. The entities that are generated are listed below,

- Main project,

	-	`Enums`  
		Enumerated types and corresponding [`EnumConverter`](https://github.com/RavinduL/SEStandard/blob/master/src/RavinduL.SEStandard/EnumConverter.cs)s.

	-	`Classes`  
		Model classes that responses from the Stack Exchange API are deserialized to.

	-	`Methods`  
		Classes that represent endpoints of the Stack Exchange API, and contain corresponding methods.

- Test project,

	-	`EnumConversionTests`  
		Unit tests that test how the `Endpoint.ConvertToString` method converts the enums generated as part of the `Enums` entity, to their `string` representations.

The entities are represented by the [`CodeGenEntities`](https://github.com/RavinduL/SEStandard/blob/master/src/RavinduL.SEStandard.Build/CodeGen/CodeGenEntities.cs) enumeration, which is implemented as a [bit field](https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute) in order to map dependencies between entities (e.g. generating `Classes` depends on the awareness of the `Enums` entity).

### Data Sets

Entities are generated using JSON datasets that are named the same as the entity, and are located in the `CodeGen/Data/Sets` folder relative to that of the build project. JSON schemas for the aforementioned data sets are located in the same folder, with the `.schema.json` extension.

### Outputs

Each entity gets generated into a folder names the same as the entity, within the `CodeGenOutput` folder, relative to the `.csproj` file that corresponds to the project in to which it is generated (for example, `Enums` will be generated into the `src/RavinduL.SEStandard/CodeGenOutput/Enums` folder).

The `CodeGenOutput` folder is gitignored, and therefore, artefacts of code generation will not be commited to the repository.

### Generation

Generating entities is a three-step process which entails the execution of the following tasks (in order),

1.	**Prepare** (`-Target CodeGen/Prepare`, [source](https://github.com/RavinduL/SEStandard/blob/master/src/RavinduL.SEStandard.Build/CodeGen/Tasks/PrepareTask.cs))  
	The entities specified to be generated are parsed, and the output directories of each are configured.  
	The `Handlebars.Net` library is also configured by registering text encoders, helpers, etc.

2.	**Clean** (`-Target CodeGen/Clean`, [source](https://github.com/RavinduL/SEStandard/blob/master/src/RavinduL.SEStandard.Build/CodeGen/Tasks/CleanTask.cs))  
	The output directories of the entities are cleaned by removing all of their contents.

3. **Generate** (`-Target CodeGen/Generate`, [source](https://github.com/RavinduL/SEStandard/blob/master/src/RavinduL.SEStandard.Build/CodeGen/Tasks/GenerateTask.cs))  
	The data sets are read, and the files that correspond to the specified entities are generated.

Each task in the list above is dependant on the task above it. Invoking the `CodeGen/Generate` task will therefore automatically invoke the `CodeGen/Clean` task, and so on.

To generate a particular entity, execute the [`Cake.ps1`][f_cakeps1] script specifying the `CodeGen/Generate` target, specifying the entity you wish to generate via the `-Entities` parameter,

```Text
PS> # To generate only enums,
PS> .\Cake.ps1 -Target CodeGen/Generate -Entities Classes

PS> # To generate classes and methods,
PS> .\Cake.ps1 -Target CodeGen/Generate -Entities "Classes,Methods"

PS> # To generate all entities,
PS> .\Cake.ps1 -Target CodeGen/Generate -Entities All

```

<br>

## Building the Library

### Prerequistes

The tools listed below are required to build SEStandard.

1. [.NET Core SDK](https://www.microsoft.com/net/download/), for compiling the source code. The `dotnet` executable must be in the `PATH`.
2. (Recommended) [Visual Studio](https://www.visualstudio.com/) or [Visual Studio Code](https://code.visualstudio.com/), for editing the source code.
3. [PowerShell](https://microsoft.com/powershell) for executing the build script.

All tools mentioned above, with the exception of Visual Studio, are avaiable for Windows, macOS, and Linux. Visual Studio is available for Windows and macOS.

### Building

1.	Clone the repository to your computer.

	```Text
	PS> git clone https://github.com/RavinduL/SEStandard.git
	Cloning into 'SEStandard'... done.

	PS> cd SEStandard
	```

2.	Generate all entities with the `Cake.ps1` script.

	```Text
	PS> .\Cake.ps1 -Target CodeGen/Generate -Entities All
	```

3. Open the Visual Studio solution.

	```Text
	PS> .\RavinduL.SEStandard.sln
	```

[f_cakeps1]: https://github.com/RavinduL/SEStandard/blob/master/Cake.ps1
[frosting]: https://github.com/cake-build/frosting