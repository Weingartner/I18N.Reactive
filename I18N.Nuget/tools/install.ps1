# Runs every time a package is installed in a project

param($installPath, $toolsPath, $package, $project)

# $installPath is the path to the folder where the package is installed.
# $toolsPath is the path to the tools directory in the folder where the package is installed.
# $package is a reference to the package object.
# $project is a reference to the project the package was installed to.

$dteProject = "dte:/solution/projects/$($project.Name)"
ls $dteProject -Recurse `
    | Where { $_.Name -match "resx" } `
    | ForEach-Object {
        $toolName = "I18NReactive"
        Write-Host "Set CustomTool of $($_.PSPath) to $toolName"
        set-item -Path "$($_.PSPath)\ItemProperties\CustomTool" -Value $toolName
    }