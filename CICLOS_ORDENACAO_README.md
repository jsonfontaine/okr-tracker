# 📅 Ciclos com Ordenação por Data — Implementação Completa

## ✨ O que foi implementado

A solução de **ordenação de ciclos por período** foi totalmente implementada no backend e frontend, com suporte a:

1. **Adição de datas**: Novos ciclos podem ter `dataInicio` e `dataFim` (opcionais)
2. **Ordenação automática**: Ciclos são listados por `dataInicio` descendente (mais recentes primeiro)
3. **Fallback para legados**: Ciclos sem data usam `dataCriacao` como fallback
4. **Migração automática**: Ciclos com nomes em padrão (2026-Q1, 2026-S1) têm datas extraídas
5. **Frontend atualizado**: Campos de data na criação/edição e exibição do período

---

## 🔧 Arquivos Modificados

### Backend (C#)

| Arquivo | Mudança |
|---------|---------|
| `src/OkrTracker.Domain/Entities/Ciclo.cs` | Adicionado `DataInicio?` e `DataFim?` |
| `src/OkrTracker.Application/DTOs/CicloDTOs.cs` | Adicionado datas aos DTOs (Request, Response) |
| `src/OkrTracker.Application/Services/ListarCiclosService.cs` | Ordenação por `DataInicio desc` com fallback |
| `src/OkrTracker.Application/Services/CriarCicloService.cs` | Aceita datas, valida consistência |
| `src/OkrTracker.Application/Services/AtualizarCicloService.cs` | Aceita datas, valida consistência |
| `tools/DatabaseInit/LiteDbMigrationCicloDates.cs` | **NOVO**: Migra ciclos legados (parse do nome) |
| `tools/DatabaseInit/Program.cs` | Chama migração automaticamente |

### Frontend (React)

| Arquivo | Mudança |
|---------|---------|
| `frontend/src/pages/CiclosPage.js` | Adicionado campos de data, exibição de período |
| `frontend/src/services/api.js` | Atualizado para aceitar objeto com datas |

---

## 🚀 Como Usar

### 1️⃣ Backend compilado ✅

```bash
dotnet build
# ✅ Build succeeded. 0 Warning(s), 0 Error(s)
```

### 2️⃣ Executar migração (OPCIONAL — já é automática)

Se quiser rodar manualmente:

```bash
cd tools/DatabaseInit
dotnet run
```

**Saída esperada:**
```
🔄 Iniciando migração de datas de ciclos...
✅ Ciclo '2026-Q1': 2026-01-01 a 2026-03-31
✅ Ciclo '2026-Q2': 2026-04-01 a 2026-06-30
⚠️  Ciclo 'Sem padrão': 2026-03-25 a 2026-06-25 (usando DataCriacao)
✨ Migração concluída! 3 ciclos atualizados, 0 com erro.
```

---

## 📝 Exemplos de Uso

### Criar ciclo COM datas

```javascript
// Frontend
const result = await criarCiclo({
  nome: "2026-Q1",
  dataInicio: "2026-01-01T00:00:00Z",
  dataFim: "2026-03-31T23:59:59Z"
});

// API Response
{
  "id": "guid-123",
  "nome": "2026-Q1",
  "dataInicio": "2026-01-01T00:00:00Z",
  "dataFim": "2026-03-31T23:59:59Z",
  "dataCriacao": "2026-03-25T15:30:00Z",
  "ultimaAtualizacao": "2026-03-25T15:30:00Z"
}
```

### Criar ciclo SEM datas (compatibilidade)

```javascript
// Frontend — ainda funciona!
const result = await criarCiclo({ nome: "2026-Q2" });

// Backend usa null para dataInicio e dataFim
// Ordenação: fallback para dataCriacao
```

### Listar ciclos (automaticamente ordenados)

```javascript
const result = await listarCiclos();

// Resposta (ORDENADA por dataInicio desc)
[
  {
    "id": "guid-1",
    "nome": "2026-Q2",
    "dataInicio": "2026-04-01T00:00:00Z",
    "dataFim": "2026-06-30T23:59:59Z",
    ...
  },
  {
    "id": "guid-2",
    "nome": "2026-Q1",
    "dataInicio": "2026-01-01T00:00:00Z",
    "dataFim": "2026-03-31T23:59:59Z",
    ...
  }
]
```

---

## 🎯 Padrões de Nome Suportados

| Padrão | Exemplo | Resultado |
|--------|---------|-----------|
| Quarter | `2026-Q1` | 01/01 a 31/03 |
| Quarter | `2026_Q2` | 01/04 a 30/06 |
| Semester | `2026-S1` | 01/01 a 30/06 |
| Semester | `2026_S2` | 01/07 a 31/12 |
| Sem padrão | `Ciclo Customizado` | Usa `DataCriacao` como fallback |

---

## ⚙️ Validações

### Backend validações automáticas:

✅ `DataInicio` não pode ser maior que `DataFim`

```csharp
if (request.DataInicio.Value > request.DataFim.Value)
    return Erro("A data de início não pode ser posterior à data de término.");
```

✅ Datas são opcionais (backward compatible)

✅ Ordenação usa `DataInicio desc`, fallback para `DataCriacao`

---

## 🔄 Roadmap Futuro (Sugestões)

- [ ] Tornar datas **obrigatórias** na próxima release
- [ ] Gerar nome automaticamente com base na data (ex: input `01/01/2026` → `2026-Q1`)
- [ ] Adicionar filtros por período no dashboard
- [ ] Exibir status do ciclo (Próximo, Ativo, Encerrado) com badge visual

---

## 📋 Checklist de Testes

- [ ] Criar ciclo com datas
- [ ] Criar ciclo sem datas
- [ ] Editar ciclo adicionando datas
- [ ] Listar ciclos e verificar ordenação (descendente por data)
- [ ] Verificar se ciclos legados tiveram datas migradas
- [ ] Testar validação (dataInicio > dataFim deve erro)

---

## 🎉 Pronto para uso!

Você pode agora:
1. **Executar a API**: `dotnet run` na pasta `OkrTracker.Api`
2. **Iniciar o frontend**: `npm start` na pasta `frontend`
3. **Criar ciclos com período** — dashboard mostrará dados bem organizados e ordenados

