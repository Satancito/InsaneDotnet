[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet("Major", "Minor", "Build", "Revision")]
    [String]
    $BuildValueType = "Build"
)
    
$ErrorActionPreference = "Stop"
Import-Module -Name "$(Get-Item "./Z-PsCoreFxs.ps1")" -Force -NoClobber


    
Clear-Host 
    
Write-Host
[System.String] $project = Get-Item "./*.csproj"
Write-PrettyKeyValue "███ Publish NugetPackage for project" "`"$project`""



Write-Host
Write-InfoBlue "█ Update - Project Version"
$newVersion = Update-Version -ProjectFileName $project -ValueType $BuildValueType
Write-PrettyKeyValue "New version generated" "$newVersion"

Write-Host
Write-InfoBlue "█ Build - Project"
dotnet build --configuration Release

Write-Host
Write-InfoBlue "█ Build - Project Pack"
Remove-Item "./bin/Release/*.nupkg"
dotnet pack --configuration Release
$nugetPackage = (Get-Item "./bin/Release/*.nupkg")

Write-Host
Write-InfoBlue "█ Copy - Project Pack"
Write-PrettyKeyValue "From" "$($nugetPackage.FullName)"
Write-PrettyKeyValue  "To" "$($sourcePath)"
Copy-Item $nugetPackage.FullName  -Destination $sourcePath

Write-Host
Write-InfoBlue "█ Push - Git changes"
try {
    Push-Location "$sourcePath"
    git add -A
    git status
    git commit -m "New Version $newVersion"
    Test-LastExitCode
    git push
    Test-LastExitCode
}
catch {
}
finally {
    Pop-Location
}

Write-Host
Write-Host
Write-InfoMagenta "███ Finished all actions"

