param(
  [switch]$WithSeq,
  [switch]$NoBuild
)

<#
  .SYNOPSIS
    Sobe o OKR Tracker no Podman usando o compose.yml.

  .PARAMETER WithSeq
    Inicia tambem o container do Seq (observabilidade de logs).

  .PARAMETER NoBuild
    Pula o build do frontend e da imagem. Usa a imagem ja existente.

  .EXAMPLE
    .\scripts\podman-up.ps1
    .\scripts\podman-up.ps1 -WithSeq
    .\scripts\podman-up.ps1 -NoBuild
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

# ─── Resolver provider de compose ─────────────────────────────────────────────
function Test-Command {
  param([string]$Cmd, [string[]]$TestArgs = @("version"))
  try {
    $prev = $ErrorActionPreference
    $ErrorActionPreference = "SilentlyContinue"
    & $Cmd @TestArgs 2>&1 | Out-Null
    $ok = $LASTEXITCODE -eq 0
    $ErrorActionPreference = $prev
    return $ok
  } catch {
    $ErrorActionPreference = $prev
    return $false
  }
}

function Invoke-Compose {
  param([string[]]$Args)

  # 1. podman compose (built-in)
  if (Test-Command "podman" @("compose", "version")) {
    podman compose @Args; return $LASTEXITCODE
  }

  # 2. docker compose v2 (Docker Desktop CLI plugin)
  if ((Get-Command "docker" -ErrorAction SilentlyContinue) -and (Test-Command "docker" @("compose", "version"))) {
    docker compose @Args; return $LASTEXITCODE
  }

  # 3. podman-compose standalone (pip)
  if (Get-Command "podman-compose" -ErrorAction SilentlyContinue) {
    podman-compose @Args; return $LASTEXITCODE
  }

  # 4. Tentar instalar podman-compose via pip
  Write-Host "  Nenhum compose provider encontrado. Instalando podman-compose via pip..." -ForegroundColor Yellow
  $pipCmd = @("pip", "pip3") | Where-Object { Get-Command $_ -ErrorAction SilentlyContinue } | Select-Object -First 1
  if ($pipCmd) {
    & $pipCmd install podman-compose -q
    if ($LASTEXITCODE -eq 0 -and (Get-Command "podman-compose" -ErrorAction SilentlyContinue)) {
      podman-compose @Args; return $LASTEXITCODE
    }
  }

  throw "Compose nao disponivel. Execute: pip install podman-compose"
}

# ─── Build ────────────────────────────────────────────────────────────────────
if (-not $NoBuild) {
  Write-Host ""
  Write-Host "=== [1/2] Build do frontend ===" -ForegroundColor Cyan
  Set-Location "$repoRoot\frontend"
  if (-not (Test-Path "node_modules")) {
    Write-Host "  Instalando dependencias npm..."
    npm install
    if ($LASTEXITCODE -ne 0) { throw "npm install falhou." }
  }
  npm run build
  if ($LASTEXITCODE -ne 0) { throw "Build do frontend falhou." }
  Set-Location $repoRoot

  Write-Host ""
  Write-Host "=== [2/2] Build da imagem ===" -ForegroundColor Cyan
  podman build -t localhost/okr-tracker:latest -f Containerfile .
  if ($LASTEXITCODE -ne 0) { throw "Build da imagem falhou." }
} else {
  Write-Host "  -NoBuild: pulando build do frontend e da imagem." -ForegroundColor Yellow
}

# ─── Subir containers via compose.yml ─────────────────────────────────────────
Write-Host ""
Write-Host "=== Subindo containers ===" -ForegroundColor Cyan

$upArgs = @()
if ($WithSeq) {
  $upArgs += @("--profile", "observability")
  Write-Host "  Seq habilitado."
}
$upArgs += @("up", "-d")

$exitCode = Invoke-Compose $upArgs
if ($exitCode -ne 0) { throw "Falha ao subir containers." }

# ─── Configurar retencao de 24h no Seq ───────────────────────────────────────
if ($WithSeq) {
  Write-Host ""
  Write-Host "=== Configurando retencao de logs do Seq (24h) ===" -ForegroundColor Cyan
  $envFile = Join-Path $repoRoot ".env"
  $seqPort = "5341"
  if (Test-Path $envFile) {
    $line = Get-Content $envFile | Where-Object { $_ -match "^OKR_SEQ_PORT=" }
    if ($line) { $seqPort = $line -replace "^OKR_SEQ_PORT=", "" }
  }
  $seqUrl = "http://localhost:$seqPort"
  $maxWait = 60; $waited = 0
  Write-Host "  Aguardando Seq ($seqUrl)..." -NoNewline
  while ($waited -lt $maxWait) {
    try {
      $r = Invoke-WebRequest -Uri "$seqUrl/api/" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
      if ($r.StatusCode -lt 400) { break }
    } catch { }
    Start-Sleep -Seconds 2; $waited += 2; Write-Host "." -NoNewline
  }
  Write-Host ""
  if ($waited -ge $maxWait) {
    Write-Warning "  Seq nao respondeu em ${maxWait}s. Retencao nao configurada."
  } else {
    try {
      $existing = Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies" -Method Get -UseBasicParsing -ErrorAction Stop
      if ($existing -and $existing.Count -gt 0) {
        $policyId = $existing[0].Id
        $body = @{ Id = $policyId; RetentionTime = "1.00:00:00" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies/$policyId" -Method Put -Body $body -ContentType "application/json" -UseBasicParsing | Out-Null
      } else {
        $body = @{ RetentionTime = "1.00:00:00" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies" -Method Post -Body $body -ContentType "application/json" -UseBasicParsing | Out-Null
      }
      Write-Host "  Retencao configurada: logs mantidos por 24h." -ForegroundColor Green
    } catch {
      Write-Warning "  Falha ao configurar retencao: $_"
    }
  }
}

# ─── Resultado ────────────────────────────────────────────────────────────────
$envFile = Join-Path $repoRoot ".env"
$appPort = "5430"
if (Test-Path $envFile) {
  $line = Get-Content $envFile | Where-Object { $_ -match "^OKR_APP_PORT=" }
  if ($line) { $appPort = $line -replace "^OKR_APP_PORT=", "" }
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host " OKR Tracker disponivel em: http://localhost:$appPort" -ForegroundColor Green
if ($WithSeq) {
  Write-Host " Seq disponivel em:         http://localhost:$seqPort" -ForegroundColor Green
}
Write-Host "============================================" -ForegroundColor Green
