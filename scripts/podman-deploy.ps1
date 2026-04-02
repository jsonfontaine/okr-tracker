param(
  [switch]$NoBuildFrontend,
  [switch]$NoBuildImage,
  [string]$Tag = "latest",
  [switch]$WithSeq
)

<#
  .SYNOPSIS
    Realiza o build completo e o redeploy do OKR Tracker no Podman usando o compose.yml.

  .DESCRIPTION
    Pipeline completo:
      1. Build do frontend (npm run build)
      2. Build da imagem de container (podman build)
      3. Redeploy via compose.yml (compose down + compose up -d)

    O compose.yml e a fonte de verdade para portas, volumes, variaveis
    de ambiente e rede do container.

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

# ─── Resolver provider de compose ─────────────────────────────────────────────
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
  param([string[]]$ComposeArgs)

  # 1. podman compose (built-in)
  if (Test-Command "podman" @("compose", "version")) {
    podman compose @ComposeArgs; return $LASTEXITCODE
  }

  # 2. docker compose v2 (Docker Desktop)
  if ((Get-Command "docker" -ErrorAction SilentlyContinue) -and (Test-Command "docker" @("compose", "version"))) {
    docker compose @ComposeArgs; return $LASTEXITCODE
  }

  # 3. podman-compose standalone
  if (Get-Command "podman-compose" -ErrorAction SilentlyContinue) {
    podman-compose @ComposeArgs; return $LASTEXITCODE
  }

  # 4. Tentar instalar via pip
  Write-Host "  Nenhum compose provider encontrado. Instalando podman-compose via pip..." -ForegroundColor Yellow
  $pipCmd = @("pip", "pip3") | Where-Object { Get-Command $_ -ErrorAction SilentlyContinue } | Select-Object -First 1
  if ($pipCmd) {
    & $pipCmd install podman-compose -q
    if ($LASTEXITCODE -eq 0 -and (Get-Command "podman-compose" -ErrorAction SilentlyContinue)) {
      podman-compose @ComposeArgs; return $LASTEXITCODE
    }
  }

  throw "Compose nao disponivel. Execute: pip install podman-compose"
}

# ─── Info ─────────────────────────────────────────────────────────────────────
$imageName = "localhost/okr-tracker:$Tag"
Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║         OKR Tracker — Build & Deploy         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "  Imagem  : $imageName"
Write-Host "  Config  : compose.yml"
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

# ─── ETAPA 3: Redeploy via compose.yml ────────────────────────────────────────
Write-Host ""
Write-Host "▶ [3/3] Redeploy via compose.yml..." -ForegroundColor Cyan

$profileArgs = @()
if ($WithSeq) { $profileArgs = @("--profile", "observability") }

# Down graceful
Write-Host "  Parando containers existentes..."
Invoke-Compose (@() + $profileArgs + @("down")) | Out-Null

# Up com nova imagem
Write-Host "  Subindo containers..."
$exitCode = Invoke-Compose (@() + $profileArgs + @("up", "-d"))
if ($exitCode -ne 0) { throw "Falha ao subir containers." }

# ─── Status final ─────────────────────────────────────────────────────────────
$appPort = "5430"
$envFile = Join-Path $repoRoot ".env"
if (Test-Path $envFile) {
  $line = Get-Content $envFile | Where-Object { $_ -match "^OKR_APP_PORT=" }
  if ($line) { $appPort = $line -replace "^OKR_APP_PORT=", "" }
}

Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  Deploy concluido com sucesso!               ║" -ForegroundColor Green
Write-Host "╠══════════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  App : http://localhost:$appPort                 ║" -ForegroundColor Green
if ($WithSeq) {
  $seqPort = "5341"
  if (Test-Path $envFile) {
    $line = Get-Content $envFile | Where-Object { $_ -match "^OKR_SEQ_PORT=" }
    if ($line) { $seqPort = $line -replace "^OKR_SEQ_PORT=", "" }
  }
  Write-Host "║  Seq : http://localhost:$seqPort                 ║" -ForegroundColor Green
}
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
podman ps --filter "name=okr-tracker" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
