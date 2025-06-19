[CmdletBinding()] param (
    [string[]]$docfxArgs
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Push-Location $PSScriptRoot
try {
    # We cannot currently automatically render images in CI for this package since it depends on device drivers
    # ./export-images.ps1 $libPaths
    dotnet docfx metadata
    dotnet docfx build $docfxArgs
} finally {
    Pop-Location
}
