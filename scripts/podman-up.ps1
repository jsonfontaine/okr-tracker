param(
  [switch]$WithSeq,
  [switch]$NoBuild
)

<#
  .SYNOPSIS
    Sobe o OKR Tracker no Podman.

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

# Garantir que variaveis de sessao nao sobrescrevam o .env
Remove-Item Env:OKR_DB_HOST_PATH  -ErrorAction SilentlyContinue
Remove-Item Env:OKR_APP_PORT      -ErrorAction SilentlyContinue
Remove-Item Env:OKR_SEQ_PORT      -ErrorAction SilentlyContinue
Remove-Item Env:OKR_CERT_PASSWORD -ErrorAction SilentlyContinue
Remove-Item Env:REACT_APP_BASE_PATH -ErrorAction SilentlyContinue
Remove-Item Env:REACT_APP_SEQ_URL   -ErrorAction SilentlyContinue
Remove-Item Env:PUBLIC_URL          -ErrorAction SilentlyContinue

Set-Location $repoRoot

if (-not $NoBuild) {
  Write-Host ""
  Write-Host "=== [1/2] Build do frontend ===" -ForegroundColor Cyan
  Set-Location "$repoRoot\frontend"

  if (-not (Test-Path "node_modules")) {
    Write-Host "  Instalando dependencias npm..."
    npm install
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

Write-Host ""
Write-Host "=== Subindo containers ===" -ForegroundColor Cyan

# --profile e uma flag global do compose: deve vir ANTES do subcomando
$upArgs = @("compose")
if ($WithSeq) {
  $upArgs += @("--profile", "observability")
  Write-Host "  Seq habilitado na porta 5341"
}
$upArgs += @("up", "-d")

podman @upArgs
if ($LASTEXITCODE -ne 0) { throw "Falha ao subir containers." }

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host " OKR Tracker disponivel em: http://localhost:5430" -ForegroundColor Green
if ($WithSeq) {
  Write-Host " Seq disponivel em:         http://localhost:5341" -ForegroundColor Green
}
Write-Host "============================================" -ForegroundColor Green

# --- Configurar retencao de 24h no Seq ---
if ($WithSeq) {
  Write-Host ""
  Write-Host "=== Configurando retencao de logs do Seq (24h) ===" -ForegroundColor Cyan

  $seqPort = $env:OKR_SEQ_PORT
  if (-not $seqPort) {
    # Tentar ler do .env
    $envFile = Join-Path $repoRoot ".env"
    if (Test-Path $envFile) {
      $seqPort = (Get-Content $envFile | Where-Object { $_ -match "^OKR_SEQ_PORT=" }) -replace "^OKR_SEQ_PORT=",""
    }
    if (-not $seqPort) { $seqPort = "5341" }
  }
  $seqUrl = "http://localhost:$seqPort"

  # Aguardar Seq ficar disponivel (ate 60s)
  $maxWait = 60
  $waited  = 0
  Write-Host "  Aguardando Seq ($seqUrl)..." -NoNewline
  while ($waited -lt $maxWait) {
    try {
      $r = Invoke-WebRequest -Uri "$seqUrl/api/" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
      if ($r.StatusCode -lt 400) { break }
    } catch { }
    Start-Sleep -Seconds 2
    $waited += 2
    Write-Host "." -NoNewline
  }
  Write-Host ""

  if ($waited -ge $maxWait) {
    Write-Warning "  Seq nao respondeu em ${maxWait}s. Retencao nao configurada."
  } else {
    # Verificar se ja existe uma politica de retencao
    try {
      $existing = Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies" -Method Get -UseBasicParsing -ErrorAction Stop
      if ($existing -and $existing.Count -gt 0) {
        Write-Host "  Politica de retencao ja existe. Atualizando para 24h..." -ForegroundColor Yellow
        $policyId = $existing[0].Id
        $body = @{ Id = $policyId; RetentionTime = "1.00:00:00" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies/$policyId" `
          -Method Put -Body $body -ContentType "application/json" -UseBasicParsing | Out-Null
      } else {
        Write-Host "  Criando politica de retencao de 24h..." -ForegroundColor Yellow
        $body = @{ RetentionTime = "1.00:00:00" } | ConvertTo-Json
        Invoke-RestMethod -Uri "$seqUrl/api/retentionpolicies" `
          -Method Post -Body $body -ContentType "application/json" -UseBasicParsing | Out-Null
      }
      Write-Host "  Retencao configurada: logs mantidos por 24h." -ForegroundColor Green
    } catch {
      Write-Warning "  Falha ao configurar retencao: $_"
    }
  }
}

