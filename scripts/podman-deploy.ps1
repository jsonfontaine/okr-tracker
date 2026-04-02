param(
  [switch]$NoBuildFrontend,
  [switch]$NoBuildImage,
  [string]$Tag = "latest",
  [switch]$WithSeq
)

<#
  .SYNOPSIS
    Realiza o build completo e o redeploy do OKR Tracker no Podman.

  .DESCRIPTION
    Pipeline completo:
      1. Build do frontend (npm run build)
      2. Build da imagem de container (podman build)
      3. Para o container em execucao (graceful stop)
      4. Sobe o novo container (podman run)

  .PARAMETER NoBuildFrontend
    Pula o build do frontend.

  .PARAMETER NoBuildImage
    Pula o build da imagem (implica -NoBuildFrontend).

  .PARAMETER Tag
    Tag da imagem a ser gerada. Default: latest.

  .PARAMETER WithSeq
    Inclui o container do Seq no deploy.

  .EXAMPLE
    .\scripts\podman-deploy.ps1
    .\scripts\podman-deploy.ps1 -NoBuildImage
    .\scripts\podman-deploy.ps1 -Tag "v1.2.0"
    .\scripts\podman-deploy.ps1 -WithSeq
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

# ─── Ler configuracoes do .env ────────────────────────────────────────────────
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

$envVars     = Read-EnvFile (Join-Path $repoRoot ".env")
$dbPath      = $envVars["OKR_DB_HOST_PATH"] ?? "C:/PersonalTools/Appdata/OKRTracker"
$appPort     = $envVars["OKR_APP_PORT"]     ?? "5430"
$seqPort     = $envVars["OKR_SEQ_PORT"]    ?? "5341"
$imageName   = "localhost/okr-tracker:$Tag"
$networkName = "okr-net"

Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║         OKR Tracker — Build & Deploy         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "  Imagem : $imageName"
Write-Host "  Porta  : $appPort"
Write-Host "  Banco  : $dbPath"
Write-Host ""

# ─── ETAPA 1: Build do Frontend ───────────────────────────────────────────────
if (-not $NoBuildFrontend -and -not $NoBuildImage) {
  Write-Host "▶ [1/3] Build do frontend..." -ForegroundColor Cyan
  Set-Location "$repoRoot\frontend"
  if (-not (Test-Path "node_modules")) {
    Write-Host "  Instalando dependencias npm..." -ForegroundColor Yellow
    npm install
    if ($LASTEXITCODE -ne 0) { throw "npm install falhou." }
  }
  npm run build
  if ($LASTEXITCODE -ne 0) { throw "Build do frontend falhou." }
  Set-Location $repoRoot
  Write-Host "  Frontend compilado." -ForegroundColor Green
} else {
  Write-Host "▶ [1/3] Build do frontend... PULADO" -ForegroundColor Yellow
}

# ─── ETAPA 2: Build da Imagem ─────────────────────────────────────────────────
if (-not $NoBuildImage) {
  Write-Host ""
  Write-Host "▶ [2/3] Build da imagem de container..." -ForegroundColor Cyan
  podman build -t $imageName -f Containerfile .
  if ($LASTEXITCODE -ne 0) { throw "Build da imagem falhou." }
  Write-Host "  Imagem '$imageName' gerada." -ForegroundColor Green
} else {
  Write-Host "▶ [2/3] Build da imagem... PULADO" -ForegroundColor Yellow
}

# ─── ETAPA 3: Stop + Deploy ───────────────────────────────────────────────────
Write-Host ""
Write-Host "▶ [3/3] Redeploy dos containers..." -ForegroundColor Cyan

# Garantir rede
podman network exists $networkName 2>$null
if ($LASTEXITCODE -ne 0) {
  podman network create $networkName | Out-Null
  Write-Host "  Rede '$networkName' criada." -ForegroundColor DarkGray
}

# Parar e remover container antigo
podman rm -f okr-tracker 2>$null | Out-Null
Write-Host "  Container antigo removido."

# Garantir diretorio do banco
if (-not (Test-Path $dbPath)) {
  New-Item -ItemType Directory -Path $dbPath -Force | Out-Null
  Write-Host "  Diretorio do banco criado: $dbPath" -ForegroundColor Yellow
}
ra
# Subir novo container
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

# Seq (opcional)
if ($WithSeq) {
  podman rm -f okr-tracker-seq 2>$null | Out-Null
  podman volume exists seq-data 2>$null
  if ($LASTEXITCODE -ne 0) { podman volume create seq-data | Out-Null }

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
}

# ─── Status final ─────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  Deploy concluido com sucesso!               ║" -ForegroundColor Green
Write-Host "╠══════════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  App : http://localhost:$appPort                 ║" -ForegroundColor Green
if ($WithSeq) {
  Write-Host "║  Seq : http://localhost:$seqPort                 ║" -ForegroundColor Green
}
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
podman ps --filter "name=okr-tracker" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

