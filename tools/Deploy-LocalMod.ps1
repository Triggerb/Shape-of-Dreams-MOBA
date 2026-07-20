[CmdletBinding()]
param(
    [string]$GameDirectory = 'C:\Program Files (x86)\Steam\steamapps\common\Shape of Dreams',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'

if (Get-Process -Name 'Shape of Dreams' -ErrorAction SilentlyContinue) {
    throw 'Shape of Dreams is running. Exit the game completely before deploying to avoid stale loaded assemblies, Addressables, and UI references.'
}

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectDirectory = Join-Path $repositoryRoot 'src\MasterWu'
$expectedExecutable = Join-Path $GameDirectory 'Shape of Dreams.exe'
$modsDirectory = Join-Path $GameDirectory 'Mods'
$destination = Join-Path $modsDirectory 'MasterWu'
$assemblySource = Join-Path $projectDirectory "bin\$Configuration\netstandard2.1\MasterWu.dll"
$aboutSource = Join-Path $projectDirectory 'about'
$overridesSource = Join-Path $projectDirectory 'overrides'
$templateAbout = Join-Path $modsDirectory 'ModTemplate\about'

$resolvedGameDirectory = [System.IO.Path]::GetFullPath($GameDirectory)
$resolvedDestination = [System.IO.Path]::GetFullPath($destination)
$resolvedModsDirectory = [System.IO.Path]::GetFullPath($modsDirectory)

if (-not (Test-Path -LiteralPath $expectedExecutable -PathType Leaf)) {
    throw "Shape of Dreams executable not found at the expected path: $expectedExecutable"
}
if (-not (Test-Path -LiteralPath $modsDirectory -PathType Container)) {
    throw "Mods directory not found: $modsDirectory"
}
if (-not $resolvedDestination.StartsWith($resolvedModsDirectory + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to deploy outside the game's Mods directory: $resolvedDestination"
}
if (-not (Test-Path -LiteralPath $assemblySource -PathType Leaf)) {
    throw "Build output not found. Run dotnet build first: $assemblySource"
}

$destinationAbout = Join-Path $destination 'about'
$destinationBin = Join-Path $destination "bin\$Configuration\netstandard2.1"
$destinationOverrides = Join-Path $destination 'overrides'
New-Item -ItemType Directory -Force -Path $destinationAbout, $destinationBin, $destinationOverrides | Out-Null

Copy-Item -LiteralPath $assemblySource -Destination (Join-Path $destinationBin 'MasterWu.dll') -Force
Copy-Item -LiteralPath (Join-Path $aboutSource 'metadata.json') -Destination (Join-Path $destinationAbout 'metadata.json') -Force
Copy-Item -LiteralPath (Join-Path $aboutSource 'description.txt') -Destination (Join-Path $destinationAbout 'description.txt') -Force
Copy-Item -LiteralPath (Join-Path $overridesSource 'master-wu-greybox.json') -Destination (Join-Path $destinationOverrides 'master-wu-greybox.json') -Force

foreach ($imageName in @('icon.png', 'preview.png')) {
    $imageSource = Join-Path $templateAbout $imageName
    if (Test-Path -LiteralPath $imageSource -PathType Leaf) {
        Copy-Item -LiteralPath $imageSource -Destination (Join-Path $destinationAbout $imageName) -Force
    }
}

Write-Host "Deployed Master Wu $Configuration build to: $resolvedDestination"
