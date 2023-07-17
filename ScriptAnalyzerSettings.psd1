﻿# The PowerShell Script Analyzer will generate a warning
# diagnostic record for this file due to a bug -
# https://github.com/PowerShell/PSScriptAnalyzer/issues/472
@{
    # Only diagnostic records of the specified severity will be generated.
    # Uncomment the following line if you only want Errors and Warnings but
    # not Information diagnostic records.

    # Severity = @('Error','Warning')

    # Analyze **only** the following rules. Use IncludeRules when you want
    # to invoke only a small subset of the defualt rules.

    # IncludeRules = @('PSAvoidDefaultValueSwitchParameter',
    #                  'PSMissingModuleManifestField',
    #                  'PSReservedCmdletChar',
    #                  'PSReservedParams',
    #                  'PSShouldProcess',
    #                  'PSUseApprovedVerbs',
    #                  'PSUseDeclaredVarsMoreThanAssigments')

    # Do not analyze the following rules. Use ExcludeRules when you have
    # commented out the IncludeRules settings above and want to include all
    # the default rules except for those you exclude below.
    # Note: if a rule is in both IncludeRules and ExcludeRules, the rule
    # will be excluded.

    # ExcludeRules = @('')
}