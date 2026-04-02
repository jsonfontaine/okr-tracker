param(
  [switch]$NoBuildFrontend,
  [switch]$NoBuildImage,
  [string]$Tag = "latest",
  [switch]$WithSeq
)

<#
  .SYNOPSIS
    Build completo e redeploy do OKR Tracker no Podman.
    Usa compose.yml se disponivel, senao usa podman run direto.
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

function Read-EnvFile {
  $vars = @{}
  $envFile = Join-Path $repoRoot ".env"
  if (Test-Path $envFile) {
    Get-Content $envFile | Where-Object { $_ -match "^\s*[^#]\w*=" } | ForEach-Object {
      $parts = $_ -split "=", 2
      $vars[$parts[0].Trim()] = $parts[1].Trim().Trim('"').Trim("'")
    }
  }
  return $vars
}

$composeProvider = $null
if (Test-NativeCommand "podman" @("compose", "version"))        { $composeProvider = "podman" }
elseif ((Get-Command "docker" -ErrorAction SilentlyContinue) -and
        (Test-NativeCommand "docker" @("compose", "version")))  { $composeProvider = "docker" }
elseif (Get-Command "podman-compose" -ErrorAction SilentlyContinue) { $composeProvider = "podman-compose" }

$imageName = "localhost/okr-tracker:$Tag"

Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║         OKR Tracker — Build & Deploy         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "  Imagem   : $imageName"
Write-Host "  Provider : $(if ($composeProvider) { $composeProvider } else { 'podman run (fallback)' })"
Write-Host ""

# ─── ETAPA 1: Build do Frontend ───────────────────────────────────────────────
if (-not $NoBuildFrontend -and -not $NoBuildImage) {
  Write-Host "▶ [1/3] Build do frontend..." -ForegroundColor Cyan
  Set-Location "$repoRoot\frontend"
  if (-not (Test-Path "node_modules")) {
    Write-Host "  Instalando dependencias npm..." -ForegroundColor Yellow
    npm install; if ($LASTEXITCODE -ne 0) { throw "npm install falhou." }
  }
  npm run build; if ($LASTEXITCODE -ne 0) { throw "Build do frontend falhou." }
  Set-Location $repoRoot
  Write-Host "  Frontend compilado." -ForegroundColor Green
} else {
  Write-Host "▶ [1/3] Build do frontend... PULADO" -ForegroundColor Yellow
}

# ─── ETAPA 2: Build da Imagem ─────────────────────────────────────────────────
if (-not $NoBuildImage) {
  Write-Host ""
  Write-Host "▶ [2/3] Build da imagem..." -ForegroundColor Cyan
  podman build -t $imageName -f Containerfile .
  if ($LASTEXITCODE -ne 0) { throw "Build da imagem falhou." }
  Write-Host "  Imagem '$imageName' gerada." -ForegroundColor Green
} else {
  Write-Host "▶ [2/3] Build da imagem... PULADO" -ForegroundColor Yellow
}

# ─── ETAPA 3: Redeploy ────────────────────────────────────────────────────────
Write-Host ""
Write-Host "▶ [3/3] Redeploy..." -ForegroundColor Cyan
$profileArgs = @(); if ($WithSeq) { $profileArgs = @("--profile", "observability") }

if ($composeProvider) {
  Write-Host "  Parando containers (compose down)..."
  switch ($composeProvider) {
    "podman"         { podman compose @($profileArgs + @("down")) 2>&1 | Out-Null }
    "docker"         { docker compose @($profileArgs + @("down")) 2>&1 | Out-Null }
    "podman-compose" { podman-compose @($profileArgs + @("down")) 2>&1 | Out-Null }
  }
  Write-Host "  Subindo containers (compose up)..."
  $exitCode = switch ($composeProvider) {
    "podman"         { podman compose @($profileArgs + @("up", "-d")); $LASTEXITCODE }
    "docker"         { docker compose @($profileArgs + @("up", "-d")); $LASTEXITCODE }
    "podman-compose" { podman-compose @($profileArgs + @("up", "-d")); $LASTEXITCODE }
  }
  if ($exitCode -ne 0) { throw "Falha ao subir containers." }
} else {
  # Fallback: podman run direto
  $env = Read-EnvFile
  $dbPath  = $env["OKR_DB_HOST_PATH"] ?? "C:/PersonalTools/Appdata/OKRTracker"
  $appPort = $env["OKR_APP_PORT"]     ?? "5430"
  $seqPort = $env["OKR_SEQ_PORT"]    ?? "5341"
  $network = "okr-net"

  if (-not (Test-NativeCommand "podman" @("network", "exists", $network))) {
    podman network create $network | Out-Null
  }
  if (-not (Test-Path $dbPath)) { New-Item -ItemType Directory -Path $dbPath -Force | Out-Null }

  $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
  podman rm -f okr-tracker 2>&1 | Out-Null
  $ErrorActionPreference = $prev

  Write-Host "  Iniciando okr-tracker..."
  podman run -d --name okr-tracker --network $network `
    -p "${appPort}:80" -v "${dbPath}:/data:z" `
    -e DOTNET_ROLL_FORWARD=LatestPatch -e ASPNETCORE_URLS="http://+:80" `
    -e ASPNETCORE_ENVIRONMENT=Production -e "Seq__ServerUrl=http://okr-tracker-seq:80" `
    --restart unless-stopped $imageName
  if ($LASTEXITCODE -ne 0) { throw "Falha ao iniciar okr-tracker." }

  if ($WithSeq) {
    $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
    podman rm -f okr-tracker-seq 2>&1 | Out-Null; $ErrorActionPreference = $prev
    if (-not (Test-NativeCommand "podman" @("volume", "exists", "seq-data"))) { podman volume create seq-data | Out-Null }
    podman run -d --name okr-tracker-seq --network $network `
      -p "${seqPort}:80" -v "seq-data:/data" `
      -e ACCEPT_EULA=Y -e SEQ_FIRSTRUN_NOAUTHENTICATION=true `
      --restart unless-stopped datalust/seq:2024.3
    if ($LASTEXITCODE -ne 0) { throw "Falha ao iniciar Seq." }
  }
}

# ─── Status final ─────────────────────────────────────────────────────────────
$env = Read-EnvFile
$appPort = $env["OKR_APP_PORT"] ?? "5430"
Write-Host ""
Write-Host "╔══════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  Deploy concluido com sucesso!               ║" -ForegroundColor Green
Write-Host "╠══════════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  App : http://localhost:$appPort                 ║" -ForegroundColor Green
if ($WithSeq) { Write-Host "║  Seq : http://localhost:$($env['OKR_SEQ_PORT'] ?? '5341')                 ║" -ForegroundColor Green }
Write-Host "╚══════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
podman ps --filter "name=okr-tracker" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
