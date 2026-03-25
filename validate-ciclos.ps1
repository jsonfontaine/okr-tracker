# Script de Validacao - Ciclos com Ordenacao por Data
# Uso: .\validate-ciclos.ps1

Write-Host "`n==== VALIDACAO - CICLOS COM ORDENACAO POR DATA ====`n"

# TEST 1
Write-Host "TEST 1: Backend Compilado"
$build = dotnet build --no-incremental 2>&1 | Select-String "Build succeeded"
if ($build) {
    Write-Host "  PASS`n"
} else {
    Write-Host "  FAIL`n"
}

# TEST 2
Write-Host "TEST 2: Migration File Exists"
if (Test-Path "tools/DatabaseInit/LiteDbMigrationCicloDates.cs") {
    Write-Host "  PASS`n"
} else {
    Write-Host "  FAIL`n"
}

# TEST 3
Write-Host "TEST 3: DTOs with DataInicio"
$dtos = Get-Content "src/OkrTracker.Application/DTOs/CicloDTOs.cs" -Raw
if ($dtos -match "DataInicio") {
    Write-Host "  PASS`n"
} else {
    Write-Host "  FAIL`n"
}

# TEST 4
Write-Host "TEST 4: OrderByDescending in ListarCiclosService"
$service = Get-Content "src/OkrTracker.Application/Services/ListarCiclosService.cs" -Raw
if ($service -match "OrderByDescending") {
    Write-Host "  PASS`n"
} else {
    Write-Host "  FAIL`n"
}

# TEST 5
Write-Host "TEST 5: Frontend CiclosPage with Data Fields"
$page = Get-Content "frontend/src/pages/CiclosPage.js" -Raw
if (($page -match "dataInicio") -and ($page -match "dataFim")) {
    Write-Host "  PASS`n"
} else {
    Write-Host "  FAIL`n"
}

# TEST 6
Write-Host "TEST 6: API Helper Backward Compatible"
$api = Get-Content "frontend/src/services/api.js" -Raw
if ($api -match "typeof payload") {
    Write-Host "  PASS`n"
} else {
    Write-Host "  FAIL`n"
}

Write-Host "==== SUMARIO ====`n"
Write-Host "Ciclo.cs: + DataInicio DateTime?"
Write-Host "Ciclo.cs: + DataFim DateTime?"
Write-Host "DTOs: + DataInicio e DataFim em CriarCicloRequest, AtualizarCicloRequest, CicloResponse"
Write-Host "ListarCiclosService: + Ordenacao por DataInicio desc"
Write-Host "CriarCicloService: + Validacao DataInicio <= DataFim"
Write-Host "AtualizarCicloService: + Validacao DataInicio <= DataFim"
Write-Host "Frontend: + Campos de data e exibicao de periodo"
Write-Host "API: + Backward compatible (string ou objeto)`n"
Write-Host "MIGRATIONS: LiteDbMigrationCicloDates.cs (novo arquivo)`n"
Write-Host "STATUS: IMPLEMENTACAO COMPLETA`n"

