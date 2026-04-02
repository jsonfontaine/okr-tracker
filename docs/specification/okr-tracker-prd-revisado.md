# PRD — OKR Tracker (Versão Revisada)

Versão: 2.0  
Autor: Jason Fontaine  
Status: Validado  
Data: 13/03/2026

---

## 1. Visão Geral

### 1.1. Contexto
O sistema OKR Tracker evoluiu para atender a um gestor operacional multifrentes, agora com foco ampliado para projetos (antigo conceito de "Time"). O acompanhamento de OKRs, fatos relevantes e riscos é centralizado, com interface web interativa, regras visuais aprimoradas e exportação para Adaptive Card.

### 1.2. Problema
- Acompanhamento disperso dos OKRs, sem fonte única.
- Dificuldade de consolidar OKRs por projeto e ciclo.
- Comunicação executiva não padronizada.
- Falta de histórico estruturado de fatos relevantes, decisões e riscos.
- KRs nem sempre mensuráveis numericamente.

### 1.3. Objetivo do Produto
- Registrar, organizar e atualizar OKRs (Objetivos + KRs).
- Suportar KRs quantitativos, qualitativos e de projeto.
- Visualizar OKRs por projeto e ciclo em tela web interativa.
- Registrar notas, fatos relevantes e riscos.
- Exportar OKRs de um projeto para Adaptive Card.

### 1.4. Valor Esperado
- Redução de retrabalho.
- Clareza rápida sobre estado dos projetos.
- Histórico estruturado.
- Comunicação executiva fluida.

### 1.5. Público-Alvo
- Gestor Operacional Multifrentes (usuário único).

---

## 2. Escopo (IN / OUT)

### 2.1. Escopo IN
- Cadastro de Objetivos (com Projeto, Ciclo, Prioridade).
- Cadastro de KRs (Quantitativo, Qualitativo, Projeto).
- Cadastro de requisitos para KRs de Projeto.
- Registro de progresso, status, check-ins, fatos relevantes e riscos.
- Visualização em lista, filtros por ciclo e projeto.
- Exportação para Adaptive Card.
- Regras visuais: cores de fundo e borda para status e farol.
- Comportamento de comentários, fatos relevantes e riscos com quebra de linha e formatação preservada.
- Borda consistente em cards e botões de ação.

### 2.2. Escopo OUT
- Multiusuário.
- Integrações externas.
- Kanban.
- Notificações automáticas.
- Dashboards avançados.

---

## 3. Principais Mudanças e Decisões
- "Time" renomeado para "Projeto" em todo o sistema.
- Regras de cor: Objetivos e KRs concluídos com fundo verde claro; KRs com farol vermelho com fundo vermelho claro.
- Borda consistente em cards e botões de ação.
- Comportamento de comentários, fatos relevantes e riscos: Shift+Enter para nova linha, Enter para enviar; quebra de linha preservada na listagem.
- Exportação de OKRs por projeto/ciclo para Adaptive Card.
- Banco de dados atualizado: propriedade ProjetoId substitui TimeId.
- Avaliação de segurança periódica nas dependências.

---

## 4. Requisitos Funcionais
- Cadastro, edição e visualização de Objetivos e KRs.
- Cadastro de requisitos para KRs de Projeto.
- Registro de progresso, status, check-ins, fatos relevantes e riscos.
- Visualização em lista, filtros por ciclo e projeto.
- Exportação para Adaptive Card.
- Regras visuais e de borda.
- Comportamento de comentários, fatos relevantes e riscos.

---

## 5. Requisitos Não Funcionais
- Preservação total do histórico.
- Integridade dos dados.
- Simplicidade de uso.
- Acesso imediato.
- Performance básica.
- Portabilidade.
- Compatibilidade com Adaptive Cards.
- Retenção mínima de 2 anos.
- Segurança: verificação periódica de vulnerabilidades.

---

## 6. Fluxos de Usuário
- Criar, editar e visualizar Objetivos e KRs.
- Adicionar requisitos a KRs de Projeto.
- Registrar check-ins, fatos relevantes e riscos.
- Visualizar OKRs por projeto e ciclo.
- Exportar OKRs para Adaptive Card.

---

## 7. Critérios de Aceitação
- Cadastro e visualização de Objetivos e KRs.
- KRs de Projeto funcionais.
- Status de Objetivo refletindo progresso dos KRs.
- Registro e visualização de notas, fatos relevantes e riscos.
- Exportação funcional para Adaptive Card.
- Preservação de histórico.

---

## 8. Métricas de Sucesso
- Redução de retrabalho.
- Clareza rápida do estado dos projetos.
- Uso consistente de check-ins, fatos relevantes e riscos.
- Zero perda de histórico.

---

## 9. Pressupostos & Dependências
- Uso single-user.
- Interface web.
- Exportação via copiar/colar para Outlook.
- Compatibilidade com Adaptive Cards.

---

## 10. Riscos & Mitigações
- Perda de histórico: backups e retenção.
- Volume de dados: separação por ciclo.
- Inconsistência de tipos de KRs: validação.
- Alterações futuras no Adaptive Card: aderência ao schema oficial.
- Visual poluído: visão em lista resumida.

---

## 11. Anexos / Observações
- Exemplos de Adaptive Cards, prints de tela, etc.

---

## Anexo A — Feature: Edição e Exclusão de Objetivo

**Data:** 02/04/2026  
**Status:** Implementado

### A.1. Contexto

Durante a fase de planejamento de OKRs (ex.: Q2 2026), identificou-se a necessidade de editar e excluir objetivos diretamente na interface, sem precisar recriar ou usar fluxos alternativos.

### A.2. Funcionalidades

#### Excluir Objetivo
- O usuário pode excluir um objetivo clicando no botão 🗑️ localizado no cabeçalho do card.
- Uma confirmação via diálogo nativo é exibida antes de prosseguir.
- A exclusão é permanente e em cascata: remove o objetivo, todos os seus KRs, e todos os eventos associados (comentários, fatos relevantes e riscos — tanto do objetivo quanto dos KRs).

#### Editar Descrição e Valor do Objetivo
- A descrição e o valor para o negócio são exibidos em modo **somente leitura** por padrão.
- Um ícone de lápis ✏️ exibido antes do ícone "📝 Descrição" ativa o modo de edição ao ser clicado.
- Em modo de edição, ambos os campos (descrição e valor) se tornam editáveis via textarea.
- Botões **Salvar** e **Cancelar** permitem confirmar ou descartar as alterações.

### A.3. Regras de Negócio
- A exclusão exige confirmação explícita do usuário.
- A edição respeita as validações existentes do backend (descrição e valor obrigatórios).
- Não é possível excluir um objetivo inexistente (retorna erro 400).

### A.4. Fora do Escopo deste Anexo
- Edição/exclusão de KRs (tratado em anexo futuro).
- Histórico de edições de descrição/valor.

---

_Fim do PRD — OKR Tracker (Versão Revisada)._
