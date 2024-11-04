param(
    [Parameter(Position=0, Mandatory=$true)]
    [string]$Version,

    [Parameter()]
    [bool]$IsInternal = $false,
    
    [Parameter()]
    [string]$VsVersion
)

Write-Host "Set version: $version"

$FullPath = $null
if($isInternal)
{
    $FullPath = Resolve-Path $PSScriptRoot\..\..\Design\Client\TwizzarInternal.VsAddin$VsVersion\source.extension.vsixmanifest
}
else
{
    $FullPath = Resolve-Path $PSScriptRoot\..\..\Design\Client\Twizzar.VsAddin$VsVersion\source.extension.vsixmanifest
}

Write-Host $FullPath
[xml]$content = Get-Content $FullPath
$content.PackageManifest.Metadata.Identity.Version = $Version
$content.Save($FullPath)