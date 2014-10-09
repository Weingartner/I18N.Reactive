# Runs the first time a package is installed in a solution, and every time the solution is opened.

param($installPath, $toolsPath, $package, $project)
# $installPath is the path to the folder where the package is installed.
# $toolsPath is the path to the tools directory in the folder where the package is installed.
# $package is a reference to the package object.
# $project is null in init.ps1

$extensionManager = [Microsoft.VisualStudio.Shell.Package]::GetGlobalService([Microsoft.VisualStudio.ExtensionManager.SVsExtensionManager])
$VSInstallDir = Split-Path $DTE.FullName
$vsixInstallerPath = Join-Path $VSInstallDir VSIXInstaller.exe

function Vsix-Get-Installed {
    param(
        [string]
        [parameter(Mandatory = $true)]
        $VsixId
    )
    
    $vsix = $null
    if($extensionManager.TryGetInstalledExtension($VsixId, [ref]$vsix)) {
        $vsix
    }
    else {
        $null
    }
}

function Vsix-Get-Manifest-Info {
    param([string]$path)

    $dummy = [Reflection.Assembly]::Load("System.IO.Compression.FileSystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
    $zipContens = [System.IO.Compression.ZipFile]::OpenRead($path);
    $manifest = $zipContens.Entries | Where { $_.Name -match "extension.vsixmanifest" } | Select -First 1
    
    $reader = New-Object System.IO.StreamReader($manifest.Open())
    $xml = new-object System.Xml.XmlDocument
    $xml.load($reader)

    $nsm =  New-Object system.xml.XmlNamespaceManager($xml.NameTable)
    $nsm.AddNamespace("p", "http://schemas.microsoft.com/developer/vsx-schema/2010")
    
    @{
        version = $xml.SelectSingleNode('//p:Version', $nsm).InnerXml
        identifier = $xml.SelectSingleNode('//p:Identifier/@Id', $nsm).Value
        name = $xml.SelectSingleNode('//p:Name', $nsm).InnerXml
    }
}

function Vsix-Install-If-Newer{
    param([String]$path)

    $info = Vsix-Get-Manifest-Info($path)
    $vsix = Vsix-Get-Installed $info.identifier

    # If the new version of the VSIX is different to the currently installed
    # version then we replace the old VSIX with the new one.

    $installedVersion = ""
    if ($vsix) {
        $installedVersion = $vsix.Header.Version.ToString()
        Write-Host "VSIX: Found $($info.name).$installedVersion"
    }
    Write-Host "VSIX: Old version: $installedVersion, New version: $($info.version)"
    if (!$info.version.Equals($installedVersion)) {
        if ($vsix) {
            Write-Host "VSIX: Deleting old version of $($info.name).$installedVersion"
            Start-Process $vsixInstallerPath -ArgumentList /uninstall:$info.identifier -Wait
        }
        Write-Host "VSIX: Installing new version of $($info.name).$($info.version)"
        Start-Process $vsixInstallerPath -ArgumentList $path -Wait
    }
}

$path = "$toolsPath/I18N.Reactive.vsix"

Vsix-Install-If-Newer($path)
