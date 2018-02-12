<#

.SYNOPSIS
Lists all the data types of the parameters of methods of RavinduL.SEStandard that interact with the Stack Exchange API.

.DESCRIPTION
Lists all the data types of the parameters of methods of RavinduL.SEStandard that interact with the Stack Exchange API.

It does so using regular expressions to extract data types of method parameters defined in ./Data/Sets/Methods.json.

#>

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

$content = Get-Content "$PSScriptRoot/CodeGen/Data/Sets/Methods.json"

$matches = [System.Text.RegularExpressions.Regex]::Matches($content, '"type": "(?<type>[^"]+)"')

$matches | ForEach-Object {
	$_.Groups["type"].Value
} | Select-Object $_ -Unique
