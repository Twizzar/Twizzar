function RenameInText {
    param (
        $include,
        $replaceValues
    )
    
    $files = Get-ChildItem -Path $root -Recurse -include $include

    $stmtsToReplace = @()
    foreach ($key in $replaceValues.Keys) 
    { 
        $stmtsToReplace += $null
    }
    $replaceValues.Keys.CopyTo($stmtsToReplace, 0)
    
    foreach ($file in $files)
    {
        $path = [IO.Path]::Combine($file.DirectoryName, $file.Name)
    
        $sel = Select-String -Pattern $stmtsToReplace -Path $path -List
    
        if ($null -ne $sel)
        {
            Write-Output "Modifying file $path" 
    
            (Get-Content -Encoding Ascii $path) | 
            ForEach-Object {
                $containsStmt = $false
                foreach ($key in $replaceValues.Keys) 
                {
                    if ($_.Contains($key))
                    { 
                        $_.Replace($key, $replaceValues[$key])
                        $containsStmt = $true
                        break
                    }
                }
                if (!$containsStmt) { $_ } 
            } |
            Set-Content -Encoding Ascii $path
        }
    }
}

# Source\Twizzar
$root = Resolve-Path $PSScriptRoot\..\..
Set-Location $root

# Remove all test project and the DemoApplication
# Remove-Item -Path $root\*.Test -Recurse
# Remove-Item -Path $root\Twizzar.TestCommon -Recurse
# Remove-Item -Path $root\Twizzar.Fixture.Design.Ui.DemoApplication -Recurse

# Replace namespaces in code
$replaceValuesCode = 
@{
# 'clr-namespace:Twizzar'         = 'clr-namespace:TwizzarInternal';
'namespace Twizzar' = 'namespace TwizzarInternal';
'Twizzar.'          = 'TwizzarInternal.';
}

RenameInText -include *.cs,*.xaml -replaceValues $replaceValuesCode

# Visual Studio command
$replaceValuesCode = 
@{
'<ButtonText>Twizzar</ButtonText>' = '<ButtonText>TwizzarInternal</ButtonText>';
}

RenameInText -include *.vsct, -replaceValues $replaceValuesCode

# replace assembly names in project files
$replaceValuesProjectFiles = 
@{
    'Twizzar' = 'TwizzarInternal';
}

RenameInText -include *.csproj,*.sln,*.vsixmanifest, *.targets, *.nuspec -replaceValues $replaceValuesProjectFiles

# replace assembly names in project files
$replaceValuesProjectFiles = 
@{
    'twizzar' = 'TwizzarInternal';
}

RenameInText -include *.csproj,*.sln -replaceValues $replaceValuesProjectFiles

# rename project files
$files = Get-ChildItem -Path $root -Recurse |
            Where-Object {$_.Name -Match 'Twizzar'} |
            Sort-Object @{expression = {$_.FullName.length}} -descending

foreach ($file in $files) {
    $NewName = $file.Name -replace 'Twizzar', 'TwizzarInternal'
    Write-Host "replace file ${file.FullName} with $NewName"
    Rename-Item -Path $file.FullName -NewName $NewName
}