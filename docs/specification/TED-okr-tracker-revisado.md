# TED — Technical Execution Document (Versão Revisada)

OKR Tracker — Sistema Pessoal de Gestão de OKRs
Data: 13/03/2026

---

## 1. Descrição da demanda
Sistema pessoal para gestão de OKRs, focado em projetos (antigo "Time"), com interface web, regras visuais aprimoradas, exportação para Adaptive Card e banco LiteDB.

---

## 2. Arquitetura
- Monolito leve: frontend (React) + backend (ASP.NET Core).
- Banco LiteDB (.db externo, versionável em git).
- Executado localmente (localhost) ou em container Docker.

---

## 3. Modelagem de Dados
### 3.1. Entidades
- Projeto: Id, Nome, Descrição, DataCriacao, UltimaAtualizacao.
- Ciclo: Id, Nome, DataCriacao, UltimaAtualizacao.
- Objetivo: Id, Titulo, Descricao, CicloId, ProjetoId, Prioridade, Progresso, Status, Farol, Intruder, DescobertaTardia, DataCriacao, UltimaAtualizacao.
- KR: Id, ObjetivoId, Titulo, Descricao, Tipo, Progresso, Status, Farol, Intruder, DescobertaTardia, DataCriacao, UltimaAtualizacao.
- Requisito: Id, KRId, Titulo, Descricao, Critério de Aceite, Status, DataCriacao, UltimaAtualizacao.
- Comentário, Fato Relevante, Risco: Id, Texto/Descricao, ObjetivoId/KrId, Impacto (Risco), DataCriacao.

### 3.2. Coleções LiteDB
- projetos, ciclos, objetivos, krs, requisitos, comentarios, fatosRelevantes, riscos

---

## 4. APIs
- CRUD de projetos, ciclos, objetivos, KRs, requisitos.
- Registro de comentários, fatos relevantes e riscos.
- Exportação de OKRs por projeto/ciclo para Adaptive Card.
- Filtros obrigatórios por ciclo e projeto.

---

## 5. Regras de Negócio
- Objetivo deve ter pelo menos 1 KR.
- KR de Projeto pode ser decomposto em requisitos.
- Progresso de KR Projeto calculado automaticamente.
- Regras de cor: fundo verde claro para concluído, vermelho claro para farol vermelho.
- Borda consistente em cards e botões.
- Comentários, fatos relevantes e riscos: Shift+Enter para nova linha, Enter para enviar; quebra de linha preservada.

---

## 6. Segurança
- Aplicação roda localmente.
- Sem autenticação.
- Verificação periódica de vulnerabilidades nas dependências.
- Validação de entradas (caminho do .db, valores numéricos).
- Não usar dangerouslySetInnerHTML no frontend.

---

## 7. Performance e Resiliência
- Índices em LiteDB para consultas rápidas.
- try/catch ao abrir arquivo .db.
- Mensagens claras ao usuário em caso de erro.
- Regras impedindo Objetivo sem KR, exclusão de projeto/ciclo com objetivos.

---

## 8. Observabilidade
- Logging padrão ASP.NET Core.
- Log de tentativas de configuração, erros de validação, exceções LiteDB.

---

## 9. Escalabilidade
- Arquitetura single-user, single-instance.
- Sem cache distribuído.
- Suporte a múltiplos anos de OKRs.

---

## 10. Cenários de Teste
- Configuração da base.
- CRUD de projetos, ciclos, objetivos, KRs, requisitos.
- Registro e visualização de comentários, fatos relevantes e riscos.
- Exportação para Adaptive Card.
- Validação de regras de cor e borda.
- Preservação de histórico.

---

_Fim do TED — OKR Tracker (Versão Revisada)._
  