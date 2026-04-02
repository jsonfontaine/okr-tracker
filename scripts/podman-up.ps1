param(
  [switch]$WithSeq,
  [switch]$NoBuild
)

<#
  .SYNOPSIS
    Sobe o OKR Tracker no Podman usando o compose.yml.
    Fallback automatico para podman run caso nenhum compose provider esteja disponivel.
#>

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

# ─── Helpers ──────────────────────────────────────────────────────────────────
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

# ─── Detectar provider de compose ─────────────────────────────────────────────
$composeProvider = $null
if (Test-NativeCommand "podman" @("compose", "version"))        { $composeProvider = "podman" }
elseif ((Get-Command "docker" -ErrorAction SilentlyContinue) -and
        (Test-NativeCommand "docker" @("compose", "version")))  { $composeProvider = "docker" }
elseif (Get-Command "podman-compose" -ErrorAction SilentlyContinue) { $composeProvider = "podman-compose" }

if ($composeProvider) {
  Write-Host "  Compose provider: $composeProvider" -ForegroundColor DarkGray
} else {
  Write-Host "  Compose provider nao encontrado. Usando podman run direto." -ForegroundColor Yellow
}

function Invoke-Compose {
  param([string[]]$ComposeArgs)
  switch ($composeProvider) {
    "podman"         { podman compose @ComposeArgs; return $LASTEXITCODE }
    "docker"         { docker compose @ComposeArgs; return $LASTEXITCODE }
    "podman-compose" { podman-compose @ComposeArgs; return $LASTEXITCODE }
  }
  return 1  # sinaliza "sem provider"
}

# ─── Fallback: podman run direto (replica compose.yml) ────────────────────────
function Start-ContainersDirect {
  param([bool]$IncludeSeq)
  $env = Read-EnvFile
  $dbPath  = $env["OKR_DB_HOST_PATH"] ?? "C:/PersonalTools/Appdata/OKRTracker"
  $appPort = $env["OKR_APP_PORT"]     ?? "5430"
  $seqPort = $env["OKR_SEQ_PORT"]    ?? "5341"
  $network = "okr-net"

  # Garantir rede
  Test-NativeCommand "podman" @("network", "exists", $network) | Out-Null
  if (-not (Test-NativeCommand "podman" @("network", "exists", $network))) {
    podman network create $network | Out-Null
    Write-Host "  Rede '$network' criada." -ForegroundColor DarkGray
  }

  # Garantir diretorio do banco
  if (-not (Test-Path $dbPath)) {
    New-Item -ItemType Directory -Path $dbPath -Force | Out-Null
    Write-Host "  Diretorio criado: $dbPath" -ForegroundColor Yellow
  }

  # Parar container existente
  $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
  podman rm -f okr-tracker 2>&1 | Out-Null
  $ErrorActionPreference = $prev

  Write-Host "  Iniciando okr-tracker..."
  podman run -d `
    --name okr-tracker `
    --network $network `
    -p "${appPort}:80" `
    -v "${dbPath}:/data:z" `
    -e DOTNET_ROLL_FORWARD=LatestPatch `
    -e ASPNETCORE_URLS="http://+:80" `
    -e ASPNETCORE_ENVIRONMENT=Production `
    -e "Seq__ServerUrl=http://okr-tracker-seq:80" `
    --restart unless-stopped `
    localhost/okr-tracker:latest
  if ($LASTEXITCODE -ne 0) { throw "Falha ao iniciar okr-tracker." }

  if ($IncludeSeq) {
    $prev = $ErrorActionPreference; $ErrorActionPreference = "SilentlyContinue"
    podman rm -f okr-tracker-seq 2>&1 | Out-Null
    $ErrorActionPreference = $prev

    Test-NativeCommand "podman" @("volume", "exists", "seq-data") | Out-Null
    if (-not (Test-NativeCommand "podman" @("volume", "exists", "seq-data"))) {
      podman volume create seq-data | Out-Null
    }

    Write-Host "  Iniciando okr-tracker-seq..."
    podman run -d `
      --name okr-tracker-seq `
      --network $network `
      -p "${seqPort}:80" `
      -v "seq-data:/data" `
      -e ACCEPT_EULA=Y `
      -e SEQ_FIRSTRUN_NOAUTHENTICATION=true `
      --restart unless-stopped `
      datalust/seq:2024.3
    if ($LASTEXITCODE -ne 0) { throw "Falha ao iniciar Seq." }
  }
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
  Write-Host "  -NoBuild: pulando build." -ForegroundColor Yellow
}

# ─── Subir containers ─────────────────────────────────────────────────────────
Write-Host ""
Write-Host "=== Subindo containers ===" -ForegroundColor Cyan

if ($composeProvider) {
  $upArgs = @()
  if ($WithSeq) { $upArgs += @("--profile", "observability"); Write-Host "  Seq habilitado." }
  $upArgs += @("up", "-d")
  $exitCode = Invoke-Compose $upArgs
  if ($exitCode -ne 0) { throw "Falha ao subir containers." }
} else {
  Start-ContainersDirect -IncludeSeq:$WithSeq
}

# ─── Configurar retencao de 24h no Seq ───────────────────────────────────────
if ($WithSeq) {
  Write-Host ""
  Write-Host "=== Configurando retencao Seq (24h) ===" -ForegroundColor Cyan
  $env = Read-EnvFile
  $seqPort = $env["OKR_SEQ_PORT"] ?? "5341"
  $seqUrl  = "http://localhost:$seqPort"
  $maxWait = 60; $waited = 0
  Write-Host "  Aguardando Seq..." -NoNewline
  while ($waited -lt $maxWait) {
    try { $r = Invoke-WebRequest -Uri "$seqUrl/api/" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop; if ($r.StatusCode -lt 400) { break } } catch { }
    Start-Sleep 2; $waited += 2; Write-Host "." -NoNewline
  }
  Write-Host ""
  if ($waited -lt $maxWait) {
    try {
      $existing = Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies" -Method Get -UseBasicParsing -ErrorAction Stop
      if ($existing -and $existing.Count -gt 0) {
        $body = @{ Id = $existing[0].Id; RetentionTime = "1.00:00:00" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies/$($existing[0].Id)" -Method Put -Body $body -ContentType "application/json" -UseBasicParsing | Out-Null
      } else {
        $body = @{ RetentionTime = "1.00:00:00" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies" -Method Post -Body $body -ContentType "application/json" -UseBasicParsing | Out-Null
      }
      Write-Host "  Retencao configurada: 24h." -ForegroundColor Green
    } catch { Write-Warning "  Falha ao configurar retencao: $_" }
  } else { Write-Warning "  Seq nao respondeu em ${maxWait}s." }
}

# ─── Resultado ────────────────────────────────────────────────────────────────
$env = Read-EnvFile
$appPort = $env["OKR_APP_PORT"] ?? "5430"
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host " OKR Tracker disponivel em: http://localhost:$appPort" -ForegroundColor Green
if ($WithSeq) { Write-Host " Seq disponivel em: http://localhost:$($env['OKR_SEQ_PORT'] ?? '5341')" -ForegroundColor Green }
Write-Host "============================================" -ForegroundColor Green
