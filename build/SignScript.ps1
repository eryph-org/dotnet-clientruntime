
param(
    $FilePath

)

$cert = (gci cert:\CurrentUser\My -codesigning | sort NotAfter | Select -First 2 | Select -Last 1)
Set-AuthenticodeSignature $FilePath -Certificate $cert -TimestampServer http://timestamp.verisign.com/scripts/timstamp.dll -HashAlgorithm sha256

