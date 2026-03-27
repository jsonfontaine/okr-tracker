param(
  [switch]$Follow,
  [switch]$WithSeq,
  [string]$Service = ""
)

<#
  .SYNOPSIS
    Exibe os logs dos containers do OKR Tracker.

  .PARAMETER Follow
    Fica monitorando os logs em tempo real (equivalente a -f).

  .PARAMETER WithSeq
    Inclui o servico Seq na saida de logs (requer que o Seq esteja rodando).

  .PARAMETER Service
    Nome do servico especifico (okr-tracker ou seq). Padrao: todos.

  .EXAMPLE
    .\scripts\podman-logs.ps1
    .\scripts\podman-logs.ps1 -Follow
    .\scripts\podman-logs.ps1 -WithSeq -Follow
    .\scripts\podman-logs.ps1 -Service seq -Follow
#>

$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

# --profile e uma flag global do compose: deve vir ANTES do subcomando
$logsArgs = @("compose")
if ($WithSeq) {
  $logsArgs += @("--profile", "observability")
}
$logsArgs += "logs"
if ($Follow) { $logsArgs += "-f" }
if ($Service -ne "") { $logsArgs += $Service }

podman @logsArgs

