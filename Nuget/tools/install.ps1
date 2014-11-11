# Runs every time a package is installed in a project

param($installPath, $toolsPath, $package, $project)

# $installPath is the path to the folder where the package is installed.
# $toolsPath is the path to the tools directory in the folder where the package is installed.
# $package is a reference to the package object.
# $project is a reference to the project the package was installed to.

$dteProject = "dte:/solution/projects/$($project.ProjectName)"
ls $dteProject -Recurse `
    | Where { $_.Name -match "\.resx$" } `
    | ForEach-Object {
        $toolName = "I18NReactive"
        $ct = "$($_.PSPath)\ItemProperties\CustomTool"
        if((get-item $ct).Value -eq "ResXFileCodeGenerator")
        {
            Write-Host "Set CustomTool of '$($_.PSPath)' to '$toolName'"
            set-item -Path $ct -Value $toolName
        }
        else
        {
            if((get-item $ct).Value -eq $toolName)
            {
                Write-Host "CustomTool of '$($_.PSPath)' was already set to '$toolName'"
            }
            else
            {
                Write-Host "CustomTool of '$($_.PSPath)' is '$((get-item $ct).Value)' and will not be changed"
            }
        }
    }