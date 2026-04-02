param(
  [switch]$WithSeq
)

<#
  .SYNOPSIS
    Para e remove os containers do OKR Tracker via compose.yml.

  .PARAMETER WithSeq
    Inclui o container do Seq na operacao de parada.

  .EXAMPLE
    .\scripts\podman-down.ps1
    .\scripts\podman-down.ps1 -WithSeq
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

function Invoke-Compose {
  param([string[]]$Args)
  podman compose version 2>&1 | Out-Null
  if ($LASTEXITCODE -eq 0) { podman compose @Args; return $LASTEXITCODE }
  if (Get-Command "docker" -ErrorAction SilentlyContinue) {
    docker compose version 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) { docker compose @Args; return $LASTEXITCODE }
  }
  if (Get-Command "podman-compose" -ErrorAction SilentlyContinue) {
    podman-compose @Args; return $LASTEXITCODE
  }
  throw "Compose nao disponivel. Execute: pip install podman-compose"
}

Write-Host "Parando containers..." -ForegroundColor Yellow

$downArgs = @()
if ($WithSeq) { $downArgs += @("--profile", "observability") }
$downArgs += "down"

Invoke-Compose $downArgs | Out-Null

Write-Host "Containers parados." -ForegroundColor Green
