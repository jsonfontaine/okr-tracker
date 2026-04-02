param(
  [switch]$WithSeq
)

<#
  .SYNOPSIS
    Para e remove os containers do OKR Tracker.
    Usa compose.yml se disponivel, senao usa podman rm direto.
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

function Test-NativeCommand {
  param([string]$Cmd, [string[]]$CmdArgs)
  try {
    $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
    & $Cmd @CmdArgs 2>&1 | Out-Null
    $ok = $LASTEXITCODE -eq 0
    $ErrorActionPreference = $prev; return $ok
  } catch { $ErrorActionPreference = $prev; return $false }
}

$composeProvider = $null
if (Test-NativeCommand "podman" @("compose", "version"))        { $composeProvider = "podman" }
elseif ((Get-Command "docker" -ErrorAction SilentlyContinue) -and
        (Test-NativeCommand "docker" @("compose", "version")))  { $composeProvider = "docker" }
elseif (Get-Command "podman-compose" -ErrorAction SilentlyContinue) { $composeProvider = "podman-compose" }

Write-Host "Parando containers..." -ForegroundColor Yellow

if ($composeProvider) {
  $downArgs = @()
  if ($WithSeq) { $downArgs += @("--profile", "observability") }
  $downArgs += "down"
  switch ($composeProvider) {
    "podman"         { podman compose @downArgs }
    "docker"         { docker compose @downArgs }
    "podman-compose" { podman-compose @downArgs }
  }
} else {
  $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
  podman rm -f okr-tracker 2>&1 | Out-Null
  Write-Host "  okr-tracker parado." -ForegroundColor Green
  if ($WithSeq) {
    podman rm -f okr-tracker-seq 2>&1 | Out-Null
    Write-Host "  okr-tracker-seq parado." -ForegroundColor Green
  }
  $ErrorActionPreference = $prev
}

Write-Host "Containers parados." -ForegroundColor Green
