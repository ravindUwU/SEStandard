<#

.SYNOPSIS
This is a Powershell script to bootstrap a Cake.Frosting build.

.DESCRIPTION
This Powershell script will download NuGet if missing, restore NuGet tools (including Cake) and execute your Cake build script with the parameters you provide.

.PARAMETER Target
The build script target to run.

.PARAMETER Configuration
The build configuration to use.

.PARAMETER Verbosity
Specifies the amount of information to be displayed.

.PARAMETER WhatIf
Performs a dry run of the build script. No tasks will be executed.

.PARAMETER ScriptArgs
Remaining arguments are added here.

.LINK
https://github.com/cake-build/frosting

#>

[CmdletBinding()]
Param(
	[string] $Target = "Default",

	[ValidateSet("Release", "Debug")]
	[string] $Configuration = "Release",

	[ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
	[string] $Verbosity = "Verbose",

	[switch] $WhatIf,

	[Parameter(Position=0, Mandatory=$false, ValueFromRemainingArguments=$true)]
	[string[]] $ScriptArgs,

	[string] $Entities
)

# Continues only if the .NET CLI is installed.
if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
	throw "The .NET Core CLI (dotnet) is required to run the build project."
}

$ProjectName = "RavinduL.SEStandard.Build"
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

#region The tools folder and NuGet binary aren't necessary right now.

#$NugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

## Ensures that the tools folder exists.
#$ToolPath = Join-Path $PSScriptRoot "tools"
#if (!(Test-Path $ToolPath)) {
#	Write-Verbose "Creating tools directory..."
#	New-Item -Path $ToolPath -Type Directory | Out-Null
#}

## Ensures that the NuGet binary exists.
#$NugetPath = Join-Path $ToolPath "nuget.exe"
#if (!(Test-Path $NugetPath)) {
#	Write-Host "Downloading NuGet binary"
#	(New-Object System.Net.WebClient).DownloadFile($NugetUrl, $NugetPath)
#}

#endregion

# Builds the argument list.
$Arguments = @{
	Target = $Target
	Configuration = $Configuration
	Verbosity = $Verbosity
	WhatIf = $WhatIf
	Entities = $Entities
}.GetEnumerator() | ForEach-Object { "--{0}=`"{1}`"" -f $_.key, $_.value }

try {
	Push-Location
	Set-Location "$PSScriptRoot/src/$ProjectName"

	Write-Host "Restoring packages..."
	Invoke-Expression "dotnet restore"

	if ($LASTEXITCODE -eq 0) {
		Write-Output "Compiling build..."
		Invoke-Expression "dotnet publish -c Debug /v:q /nologo"

		if ($LASTEXITCODE -eq 0) {
			Write-Output "Running build..."
			Invoke-Expression "dotnet bin/Debug/netcoreapp2.0/publish/$ProjectName.dll $Arguments"
		}
	}
} finally {
	Pop-Location
	exit $LASTEXITCODE
}
