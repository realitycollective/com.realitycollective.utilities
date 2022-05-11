# Reality Toolkit UPM project generator
#
# To use:
# * Use the Template repository to clone a new repository (ensure to select the option to "copy ALL branches")
# * Clone the project locally
# * Run powershell in the cloned folder
# * Run this template initialization script with the suffix of the project being generated
# 	E.G.
#	    ./InitialiseTemplate.ps1 WindowsXR
# * Once run, delete the InitializeTemplate script
# * Copy the cloned project into the RealityToolkit.dev project in the packages folder
# * Open Unity to generate the required meta files for the solution
# * Resolve any issues in the Unity project
# * Push the generated project back to the repository
# * Once the build has successfully built, ONLY THEN, deleted the cloned folder and add it as a submodule to the "RealityToolkit.dev" project
# 
# This will generate the "RealityToolkit.WindowsXR" UPM package


 param (
        [Parameter(Mandatory = $true)]
        [string[]]$ProjectName
    )

$templateVar = 'UPMTEMPLATE'
$templateVarGUID = 'UPMTEMPLATEGUID'
$lowerProjectName = $ProjectName.ToLower().Replace('-','_')
$upperProjectName = $ProjectName.Toupper().Replace('-','_').Replace(".","_")
$newProjectGuid = [guid]::NewGuid()

if ( -not (Test-Path ".\Documentation~\com.realitytoolkit.$templateVar.md")) 
{
    echo "Unable to process, no template files / folders found.  Has the processor already been run for this project?"
    exit 1
}

if (Test-Path ".\Documentation\com.realitytoolkit.$ProjectName.md") 
{
    echo "Unable to process, files exist for the new template name already"
    exit 1
}


Get-ChildItem ".\*.json" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace $templateVar, $lowerProjectName | Set-Content $_
}
Get-ChildItem ".\*.cs" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace $templateVarGUID, $newProjectGuid | Set-Content $_
}
Get-ChildItem ".\*.cs" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace "REALITYTOOLKIT_$templateVar", "REALITYTOOLKIT_$upperProjectName" | Set-Content $_
}
Get-ChildItem ".\*.cs" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace $templateVar, $lowerProjectName | Set-Content $_
}
Get-ChildItem ".\*.md" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace $templateVar, $lowerProjectName | Set-Content $_
}
Get-ChildItem ".\*.json" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace $templateVar, $lowerProjectName | Set-Content $_
}
Get-ChildItem ".\*.asmdef" -Recurse | ForEach-Object -Process {
    (Get-Content $_) -Replace $templateVar, $lowerProjectName | Set-Content $_
}

#Rename files#
ChildItem .\ -Recurse | Where-Object { $_.name -like "*$templateVar*"} | Rename-Item -NewName { $_.name -replace $templateVar, $lowerProjectName}

echo "package name: $ProjectName"