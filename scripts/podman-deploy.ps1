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
    Executa o pipeline completo:
      1. Build do frontend (npm run build)
      2. Build da imagem de container (podman build)
      3. Para o container em execucao (graceful stop)
      4. Sobe o novo container (podman compose up -d)

  .PARAMETER NoBuildFrontend
    Pula o build do frontend. Usa o conteudo ja existente em frontend/build/.

  .PARAMETER NoBuildImage
    Pula o build da imagem. Implica -NoBuildFrontend.
    Util para redeploy rapido sem alteracoes de codigo.

  .PARAMETER Tag
    Tag da imagem a ser gerada. Default: latest.

  .PARAMETER WithSeq
    Inclui o container do Seq (observabilidade) no deploy.

  .EXAMPLE
    # Build completo + deploy
    .\scripts\podman-deploy.ps1

    # Apenas redeploy (sem rebuild)
    .\scripts\podman-deploy.ps1 -NoBuildImage

    # Build frontend, mas pula rebuild da imagem
    .\scripts\podman-deploy.ps1 -NoBuildImage

    # Build completo com tag especifica
    .\scripts\podman-deploy.ps1 -Tag "v1.2.0"

    # Com Seq habilitado
    .\scripts\podman-deploy.ps1 -WithSeq
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$imageName = "localhost/okr-tracker:$Tag"

Set-Location $repoRoot

# Limpar variaveis de sessao para nao sobrescrever o .env
foreach ($envVar in @("OKR_DB_HOST_PATH","OKR_APP_PORT","OKR_SEQ_PORT","OKR_CERT_PASSWORD")) {
  Remove-Item "Env:$envVar" -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║         OKR Tracker — Build & Deploy         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "  Imagem : $imageName"
Write-Host "  Raiz   : $repoRoot"
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
  Write-Host "  Frontend compilado com sucesso." -ForegroundColor Green
} else {
  Write-Host "▶ [1/3] Build do frontend... PULADO" -ForegroundColor Yellow
}

# ─── ETAPA 2: Build da Imagem ─────────────────────────────────────────────────
if (-not $NoBuildImage) {
  Write-Host ""
  Write-Host "▶ [2/3] Build da imagem de container..." -ForegroundColor Cyan

  podman build -t $imageName -f Containerfile .
  if ($LASTEXITCODE -ne 0) { throw "Build da imagem falhou." }

  Write-Host "  Imagem '$imageName' gerada com sucesso." -ForegroundColor Green
} else {
  Write-Host "▶ [2/3] Build da imagem... PULADO" -ForegroundColor Yellow
}

# ─── ETAPA 3: Stop + Deploy ───────────────────────────────────────────────────
Write-Host ""
Write-Host "▶ [3/3] Deploy dos containers..." -ForegroundColor Cyan

# Parar containers em execucao (graceful, ignorando erro se nao estiver rodando)
$downArgs = @("compose")
if ($WithSeq) { $downArgs += @("--profile", "observability") }
$downArgs += "down"

Write-Host "  Parando containers existentes..."
podman @downArgs 2>&1 | Out-Null

# Subir nova versao
$upArgs = @("compose")
if ($WithSeq) {
  $upArgs += @("--profile", "observability")
  Write-Host "  Seq habilitado."
}
$upArgs += @("up", "-d")

podman @upArgs
if ($LASTEXITCODE -ne 0) { throw "Falha ao subir containers." }

# ─── Status final ─────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  Deploy concluido com sucesso!               ║" -ForegroundColor Green
Write-Host "╠══════════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  App : http://localhost:5430                 ║" -ForegroundColor Green
if ($WithSeq) {
  Write-Host "║  Seq : http://localhost:5341                 ║" -ForegroundColor Green
}
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Green

Write-Host ""
Write-Host "  Containers em execucao:" -ForegroundColor DarkGray
podman ps --filter "name=okr-tracker" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

