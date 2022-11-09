using namespace System.IO
using namespace System.Text
using namespace System.Management.Automation
using namespace System.Management.Automation.Language
using namespace System.Collections
using namespace System.Collections.Generic
using namespace System.IO.Compression

# All Credits to jborean93 on the EncodingTransformation Class
# Source: https://gist.github.com/jborean93/50a517a8105338b28256ff0ea27ab2c8#file-get-extendedattribute-ps1

class EncodingTransformation : ArgumentTransformationAttribute {
    [object] Transform([EngineIntrinsics] $EngineIntrinsics, [object] $InputData) {
        $outputData = switch($InputData) {
            { $_ -is [Encoding] } { $_ }

            { $_ -is [string] } {
                switch ($_) {
                    ASCII { [ASCIIEncoding]::new() }
                    BigEndianUnicode { [UnicodeEncoding]::new($true, $true) }
                    BigEndianUTF32 { [UTF32Encoding]::new($true, $true) }
                    ANSI {
                        $raw = Add-Type -Namespace Encoding -Name Native -PassThru -MemberDefinition '
                            [DllImport("Kernel32.dll")]
                            public static extern Int32 GetACP();
                        '
                        [Encoding]::GetEncoding($raw::GetACP())
                    }
                    OEM { [Console]::OutputEncoding }
                    Unicode { [UnicodeEncoding]::new() }
                    UTF8 { [UTF8Encoding]::new($false) }
                    UTF8BOM { [UTF8Encoding]::new($true) }
                    UTF8NoBOM { [UTF8Encoding]::new($false) }
                    UTF32 { [UTF32Encoding]::new() }
                    default { [Encoding]::GetEncoding($_) }
                }
            }

            { $_ -is [int] } { [Encoding]::GetEncoding($_) }

            default {
                throw [ArgumentTransformationMetadataException]::new(
                    "Could not convert input '$_' to a valid Encoding object."
                )
            }
        }

        return $outputData
    }
}

class EncodingCompleter : IArgumentCompleter {
    [string[]] $EncodingSet = @(
        'ascii'
        'bigendianutf32'
        'unicode'
        'utf8'
        'utf8NoBOM'
        'bigendianunicode'
        'oem'
        'utf7'
        'utf8BOM'
        'utf32'
        'ansi'
    )

    [IEnumerable[CompletionResult]] CompleteArgument (
        [string] $commandName,
        [string] $ParameterName,
        [string] $wordToComplete,
        [CommandAst] $commandAst,
        [IDictionary] $fakeBoundParameters
    ) {
        [CompletionResult[]] $arguments = foreach($enc in $this.EncodingSet) {
            if($enc.StartsWith($wordToComplete)) {
                [CompletionResult]::new($enc)
            }
        }
        return $arguments
    }
}