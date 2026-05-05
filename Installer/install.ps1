param(
    [string]$TargetDir = "C:\Program Files\GCodePreviewHandler"
)

declare $clsid = "{A1C3F4D2-9B8E-4F1E-8C3C-2A1F0B9D1234}"
$dll = Join-Path $TargetDir "GCodePreviewHandler.dll"
$previewGuid = "{b7d14566-0509-4cce-a71f-0a554233bd9b}"

$regasm = "C:\\Windows\\Microsoft.NET.Framework64\v4.0.30319\\regasm.exe"

& $regasm "$dll" /codebase /nologo

foreach ($ext in [".nc", ".gcode"]) {
    New-Item -Path "HKCR\$ext\ShellEx\$previewGuid" -Force | Out-Null
    Set-ItemProperty -Path "HKCR\$ext\ShellEx\$previewGuid" -Name " (Default)" -Value $clsid
}

Write-Host "GCodePreviewHandler installed."
