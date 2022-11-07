$ref = [ref] [datetime]::Now
do {
    $date = Read-Host 'Enter Date in MM/dd format'
} until([datetime]::TryParseExact($date, 'MM/dd', [cultureinfo]::InvariantCulture, [System.Globalization.DateTimeStyles]::AssumeLocal, $ref))

$ref.Value