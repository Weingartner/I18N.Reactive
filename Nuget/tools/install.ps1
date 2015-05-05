# Runs every time a package is installed in a project

param($installPath, $toolsPath, $package, $project)

# $installPath is the path to the folder where the package is installed.
# $toolsPath is the path to the tools directory in the folder where the package is installed.
# $package is a reference to the package object.
# $project is a reference to the project the package was installed to.

function GetProjectItems() {
    param($parent)
    $children = new-object "System.Collections.Generic.List[EnvDTE.ProjectItem]"
    foreach($child in $parent) {
        $children += $child
        foreach($grandChild in GetProjectItems($child.ProjectItems)) {
            $children += $grandChild
        }
    }
    return $children
}

$toolName = "I18NReactive"
$project = $dte.Solution.Projects | ? { $_.ProjectName -eq "ConsoleApplication3" } | select -first 1
GetProjectItems($project.ProjectItems) |
? { $_.Name -match "\.resx" } |
% {
    $customTool = $_.Properties | ? { $_.Name -eq "CustomTool" } | select -first 1
    if($customTool.Value -eq "ResXFileCodeGenerator")
    {
        Write-Host "Set CustomTool of '$($_.Name)' to '$toolName'"
        $customTool.Value = $toolName
    }
    elseif($customTool.Value -eq $toolName)
    {
        Write-Host "CustomTool of '$($_.Name)' was already set to '$toolName'"
    }
    else
    {
        Write-Host "CustomTool of '$($_.Name)' is '$($customTool.Value)' and will not be changed"
    }
}