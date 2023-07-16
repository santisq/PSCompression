$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'PSVersionHelper Class' {
    It 'Should have the same value as $IsCoreCLR' {
        $property = [PSCompression.ZipEntryBase].Assembly.
            GetType('PSCompression.PSVersionHelper').
            GetProperty(
                'IsCoreCLR',
                [System.Reflection.BindingFlags] 'NonPublic, Static')

        $property.GetValue($property) | Should -BeExactly ([bool] $IsCoreCLR)
    }
}
