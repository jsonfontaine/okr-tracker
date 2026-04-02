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

function Test-Command {
  param([string]$Cmd, [string[]]$TestArgs = @("version"))
  try {
    $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
    & $Cmd @TestArgs 2>&1 | Out-Null
    $ok = $LASTEXITCODE -eq 0
    $ErrorActionPreference = $prev; return $ok
  } catch { $ErrorActionPreference = $prev; return $false }
}

function Invoke-Compose {
  param([string[]]$Args)
  if (Test-Command "podman" @("compose", "version")) { podman compose @Args; return $LASTEXITCODE }
  if ((Get-Command "docker" -ErrorAction SilentlyContinue) -and (Test-Command "docker" @("compose", "version"))) {
    docker compose @Args; return $LASTEXITCODE
  }
  if (Get-Command "podman-compose" -ErrorAction SilentlyContinue) { podman-compose @Args; return $LASTEXITCODE }
  throw "Compose nao disponivel. Execute: pip install podman-compose"
}

Write-Host "Parando containers..." -ForegroundColor Yellow

$downArgs = @()
if ($WithSeq) { $downArgs += @("--profile", "observability") }
$downArgs += "down"

Invoke-Compose $downArgs | Out-Null

Write-Host "Containers parados." -ForegroundColor Green
