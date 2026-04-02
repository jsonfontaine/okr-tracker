param(
  [switch]$WithSeq,
  [switch]$NoBuild
)

<#
  .SYNOPSIS
    Sobe o OKR Tracker no Podman (sem dependencia de docker-compose ou podman-compose).

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

# ─── Ler configuracoes do .env (se existir) ───────────────────────────────────
function Read-EnvFile {
  param([string]$Path)
  $vars = @{}
  if (Test-Path $Path) {
    Get-Content $Path | Where-Object { $_ -match "^\s*[^#]\w*=" } | ForEach-Object {
      $parts = $_ -split "=", 2
      $vars[$parts[0].Trim()] = $parts[1].Trim().Trim('"').Trim("'")
    }
  }
  return $vars
}

$envVars   = Read-EnvFile (Join-Path $repoRoot ".env")
$dbPath    = $envVars["OKR_DB_HOST_PATH"] ?? "C:/PersonalTools/Appdata/OKRTracker"
$appPort   = $envVars["OKR_APP_PORT"]     ?? "5430"
$seqPort   = $envVars["OKR_SEQ_PORT"]    ?? "5341"
$imageName = "localhost/okr-tracker:latest"
$networkName = "okr-net"

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
  podman build -t $imageName -f Containerfile .
  if ($LASTEXITCODE -ne 0) { throw "Build da imagem falhou." }
} else {
  Write-Host "  -NoBuild: pulando build do frontend e da imagem." -ForegroundColor Yellow
}

# ─── Rede ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "=== Preparando rede '$networkName' ===" -ForegroundColor Cyan
podman network exists $networkName 2>$null
if ($LASTEXITCODE -ne 0) {
  podman network create $networkName | Out-Null
  Write-Host "  Rede '$networkName' criada." -ForegroundColor Green
} else {
  Write-Host "  Rede '$networkName' ja existe." -ForegroundColor DarkGray
}

# ─── Subir OKR Tracker ────────────────────────────────────────────────────────
Write-Host ""
Write-Host "=== Subindo okr-tracker ===" -ForegroundColor Cyan

# Remover container antigo se existir
podman rm -f okr-tracker 2>$null | Out-Null

# Garantir que o diretorio do banco existe no host
if (-not (Test-Path $dbPath)) {
  New-Item -ItemType Directory -Path $dbPath -Force | Out-Null
  Write-Host "  Diretorio do banco criado: $dbPath" -ForegroundColor Yellow
}

podman run -d `
  --name okr-tracker `
  --network $networkName `
  -p "${appPort}:80" `
  -v "${dbPath}:/data:z" `
  -e DOTNET_ROLL_FORWARD=LatestPatch `
  -e ASPNETCORE_URLS="http://+:80" `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e "Seq__ServerUrl=http://okr-tracker-seq:80" `
  --restart unless-stopped `
  $imageName

if ($LASTEXITCODE -ne 0) { throw "Falha ao iniciar okr-tracker." }
Write-Host "  okr-tracker iniciado." -ForegroundColor Green

# ─── Subir Seq (opcional) ─────────────────────────────────────────────────────
if ($WithSeq) {
  Write-Host ""
  Write-Host "=== Subindo Seq ===" -ForegroundColor Cyan

  podman rm -f okr-tracker-seq 2>$null | Out-Null

  # Garantir volume para dados do Seq
  podman volume exists seq-data 2>$null
  if ($LASTEXITCODE -ne 0) {
    podman volume create seq-data | Out-Null
  }

  podman run -d `
    --name okr-tracker-seq `
    --network $networkName `
    -p "${seqPort}:80" `
    -v "seq-data:/data" `
    -e ACCEPT_EULA=Y `
    -e SEQ_FIRSTRUN_NOAUTHENTICATION=true `
    --restart unless-stopped `
    datalust/seq:2024.3

  if ($LASTEXITCODE -ne 0) { throw "Falha ao iniciar Seq." }
  Write-Host "  Seq iniciado." -ForegroundColor Green

  # Configurar retencao de 24h
  Write-Host ""
  Write-Host "=== Configurando retencao de logs do Seq (24h) ===" -ForegroundColor Cyan
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
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host " OKR Tracker disponivel em: http://localhost:$appPort" -ForegroundColor Green
if ($WithSeq) {
  Write-Host " Seq disponivel em:         http://localhost:$seqPort" -ForegroundColor Green
}
Write-Host "============================================" -ForegroundColor Green
