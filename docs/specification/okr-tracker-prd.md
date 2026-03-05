# PRD — OKR Tracker

Versão: 1.0  
Autor: Jason Fontaine  
Status: Validado  
Data: 04/03/2026

---

## 1. Visão Geral

### 1.1. Contexto

O usuário atua como gestor operacional multifrentes, acompanhando objetivos, metas e entregas de diversos times. O acompanhamento atual de OKRs é feito de forma dispersa (Planner, anotações soltas, conversas, memória pessoal), o que:

- dificulta a visualização clara do progresso por time e ciclo;
- aumenta o retrabalho na hora de consolidar status para apresentações;
- torna o histórico de decisões e fatos relevantes pouco confiável.

A maturidade de métricas na organização ainda é baixa. Em muitos casos, não é possível definir KRs SMART com métricas diretas, sendo necessário trabalhar com **KRs do tipo “projeto”**, cujo objetivo é justamente viabilizar a medição futura.

### 1.2. Problema

- Acompanhamento disperso dos OKRs, sem uma fonte única de verdade.
- Dificuldade de consolidar OKRs por time e por ciclo.
- Formato de comunicação para superintendência não padronizado e com alto retrabalho.
- Falta de histórico estruturado de fatos relevantes, decisões e riscos.
- Parte relevante dos KRs não é mensurável numericamente, o que torna planilhas ou ferramentas padrão pouco aderentes.

### 1.3. Objetivo do Produto

Criar um **sistema pessoal (single user)** para:

- Registrar, organizar e atualizar OKRs (Objetivos + KRs);
- Suportar **KRs quantitativos, qualitativos e de projeto**;
- Registrar requisitos e progresso de KRs de projeto;
- Visualizar OKRs por time e ciclo em uma **tela web interativa em lista**;
- Registrar notas, fatos relevantes e riscos;
- Preservar histórico por pelo menos 2 anos;
- Exportar OKRs de um time em formato adequado para **Adaptive Card**, para uso direto no Outlook em comunicação com superintendência.

### 1.4. Valor Esperado

- Redução significativa de retrabalho na consolidação de OKRs.
- Maior clareza sobre estado dos times no ciclo atual em poucos segundos.
- Histórico estruturado para consultas futuras (retro, análise de evolução, lições aprendidas).
- Comunicação executiva mais fluida, com menos esforço de preparação.
- Adequação à realidade de baixa maturidade de métricas, por meio de KRs de projeto.

### 1.5. Público-Alvo

- Usuário único: **Gestor Operacional Multifrentes** (o próprio autor do sistema).

---

## 2. Escopo (IN / OUT)

### 2.1. Escopo IN – Versão 1

Inclui:

1. **Cadastro de Objetivos**
   - Título (obrigatório)
   - Descrição (obrigatória)
   - Ciclo
   - Time/Frente
   - Prioridade (Alta / Média / Baixa)

2. **Cadastro de KRs**
   - Título (obrigatório)
   - Descrição (obrigatória)
   - Tipo:
     - Quantitativo
     - Qualitativo
     - Projeto

3. **KRs do tipo Projeto**
   - Cadastro de requisitos contendo:
     - Título (obrigatório)
     - Descrição (obrigatória)
     - Critério de aceite (obrigatório)
   - Marcação de requisito como implementado / não implementado
   - Cálculo automático de progresso do KR com base nos requisitos

4. **Atualização de KRs Quantitativos/Qualitativos**
   - Registro de valor atualizado
   - Registro de data da última atualização

5. **Cálculo de Status de Objetivos**
   - Regra simplificada baseada no progresso dos KRs (média simples na V1)

6. **Tela Web Interativa (Lista)**
   - Visualização padrão do ciclo atual
   - Filtros por:
     - Ciclo
     - Time/Frente
   - Expansão de Objetivo → KRs → Requisitos

7. **Registro de Check-ins (Notas)**
   - Em Objetivo
   - Em KR

8. **Registro de Fatos Relevantes**
   - Por Objetivo e por KR
   - Com destaque visual em relação a notas comuns

9. **Registro de Riscos e Pontos de Atenção**
   - Por Objetivo e por KR
   - Com campo de impacto

10. **Histórico de Ciclos Anteriores**
    - Acesso a OKRs de ciclos passados
    - Sem exclusão acidental

11. **Exportação em Formato Adaptive Card**
    - Exportar OKRs de um time/frente para uso no Outlook
    - Estrutura legível e navegável

12. **Retenção de Dados**
    - Preservação de OKRs por no mínimo 2 anos
    - Possibilidade de exclusão após 2 anos, com confirmação explícita

### 2.2. Escopo OUT – Versão 1

Ficam fora da V1:

- Visualização em Kanban.
- Multiusuário, contas, perfis ou permissões.
- Integrações com ferramentas externas (Planner, Jira, Azure DevOps, Google Sheets etc.).
- Automação de coleta de métricas.
- Dashboards gráficos avançados.
- Notificações push ou e-mail automáticas.
- Recursos de Inteligência Artificial.
- Exportação para PDF, Excel, PowerPoint.
- Gestão avançada de iniciativas/projetos além dos requisitos simples de KRs de projeto.

---

## 3. Persona

### 3.1. Persona Principal — Gestor Operacional Multifrentes

- **Responsabilidades**
  - Acompanhar múltiplos times e frentes de trabalho.
  - Consolidar resultados para liderança (superintendência).
  - Planejar e acompanhar ciclos de OKR.

- **Motivações**
  - Reduzir esforço de consolidação.
  - Ganhar visibilidade rápida do progresso.
  - Manter histórico confiável de decisões, riscos e aprendizados.

- **Dores**
  - Retrabalho para juntar informações dispersas.
  - Falta de padronização na comunicação com a liderança.
  - Dificuldade de acompanhar KRs que não são métricos.

- **Definição de Sucesso**
  - Conseguir operar um ciclo completo de OKRs usando só o OKR Tracker.
  - Ao abrir o sistema, em menos de 1 minuto entender a situação de cada time.
  - Conseguir gerar um Adaptive Card funcional para o Outlook quando necessário.

---

## 4. Problemas & Oportunidades

### 4.1. Problemas

1. Acompanhamento de OKRs em múltiplas ferramentas (Planner, anotações, memória).
2. Falta de visão consolidada por time e por ciclo.
3. Ausência de padrão na apresentação de OKRs para a liderança.
4. Baixa rastreabilidade de decisões, riscos e fatos relevantes.
5. Dificuldade em lidar com KRs que não são mensuráveis numericamente.
6. Risco de perda de histórico e descontinuidade de contexto entre ciclos.

### 4.2. Oportunidades

1. Criar uma fonte única de verdade para OKRs pessoais e de times sob responsabilidade do usuário.
2. Estruturar a gestão de KRs de projeto com requisitos e critérios de aceite.
3. Facilitar a comunicação executiva com Adaptive Cards no Outlook.
4. Construir um histórico confiável de aprendizado e evolução.
5. Melhorar a capacidade de planejamento e tomada de decisão com base em dados e registros.

---

## 5. Requisitos Funcionais (Detalhados)

> Notação:  
> RF-XX — [Título]  
> - **Descrição:** …  
> - **Entradas:** …  
> - **Saídas/Comportamento:** …  
> - **Regras de validação:** …  
> - **Erros/mensagens (se aplicável):** …

---

### RF-01 — Criar Objetivo

- **Descrição:**  
  Permitir que o usuário crie um novo Objetivo dentro de um ciclo, associado a um time/frente, com definição de prioridade.

- **Entradas:**
  - Título (texto curto) — **obrigatório**
  - Descrição (texto longo) — **obrigatória**
  - Ciclo (ex.: `2026-Q1`, `2026-S1`) — pode ter valor padrão (ciclo atual)
  - Time/Frente (texto livre ou lista simples)
  - Prioridade (enum: Alta, Média, Baixa) — valor padrão sugerido: Média

- **Saídas/Comportamento:**
  - Criação de um registro de Objetivo com:
    - ID único
    - Progresso inicial = 0%
    - Status inicial = “Não iniciado”
    - Data de criação
  - Exibição do novo Objetivo na tela principal (lista) ao salvar.
  - Permitir edição posterior de todos os campos, exceto ID.

- **Regras de validação:**
  - Bloquear criação se Título estiver vazio.
  - Bloquear criação se Descrição estiver vazia.
  - Se ciclo não informado, utilizar ciclo atual como padrão.
  - Se prioridade não informada, definir como “Média”.

- **Erros/mensagens:**
  - “Título do objetivo é obrigatório.”
  - “Descrição do objetivo é obrigatória.”

---

### RF-02 — Criar Key Results (KR) com Tipo

- **Descrição:**  
  Permitir que o usuário crie KRs associados a um Objetivo, com definição de tipo (quantitativo, qualitativo ou projeto).

- **Entradas:**
  - Objetivo de referência (selecionado no contexto)
  - Título (obrigatório)
  - Descrição (obrigatória)
  - Tipo (enum: Quantitativo, Qualitativo, Projeto)
  - Campos adicionais por tipo (opcional na V1, podem ser simples):
    - Quantitativo:
      - Valor alvo (número)
      - Valor inicial (número)
    - Qualitativo:
      - Critério de avaliação (texto, pode ser a própria descrição)
    - Projeto:
      - Sem campos extras obrigatórios além dos requisitos (RF-04)

- **Saídas/Comportamento:**
  - Criação de KR com:
    - ID único
    - Progresso inicial = 0%
    - Status inicial = “Não iniciado”
    - Vinculado ao Objetivo pai
    - Tipo armazenado
  - Exibição do KR na lista de KRs do Objetivo.

- **Regras de validação:**
  - Bloquear criação se Título estiver vazio.
  - Bloquear criação se Descrição estiver vazia.
  - Bloquear criação se Tipo não estiver selecionado.
  - Um Objetivo pode ter 0 ou mais KRs.

- **Erros/mensagens:**
  - “Título do KR é obrigatório.”
  - “Descrição do KR é obrigatória.”
  - “Selecione um tipo para o KR.”

---

### RF-03 — Atualizar Progresso de KR Quantitativo / Qualitativo

- **Descrição:**  
  Permitir atualizar manualmente o progresso de KRs quantitativos ou qualitativos.

- **Entradas:**
  - KR selecionado (tipo Quantitativo ou Qualitativo)
  - Novo valor ou percentual de progresso (ex.: 45%)
  - Campo opcional de nota curta (ex.: motivo da atualização)

- **Saídas/Comportamento:**
  - Atualiza o valor de progresso armazenado para o KR.
  - Atualiza a data/hora da última atualização.
  - Recalcula o status do Objetivo (RF-08) após atualização.
  - Pode criar automaticamente um check-in simplificado (opcional) ou apenas atualizar campos.

- **Regras de validação:**
  - Para KRs quantitativos:
    - Opcionalmente garantir que o valor esteja entre 0 e 100 (se progresso em %).
  - Para KRs qualitativos:
    - Pode usar um campo de escala (0–100, 0–1 ou texto). V1: usar % simples.

- **Erros/mensagens:**
  - “Valor de progresso inválido.”

---

### RF-04 — Cadastrar Requisitos de KR Projeto

- **Descrição:**  
  Permitir que o usuário decomponha um KR do tipo Projeto em requisitos menores, cada um com critério de aceite.

- **Entradas:**
  - KR do tipo Projeto selecionado
  - Para cada requisito:
    - Título (obrigatório)
    - Descrição (obrigatória)
    - Critério de aceite (obrigatório)

- **Saídas/Comportamento:**
  - Criação de um registro de requisito com:
    - ID único
    - Referência ao KR
    - Status inicial = “Não implementado”
  - Listagem dos requisitos abaixo do KR, em uma seção específica.

- **Regras de validação:**
  - Bloquear criação se Título estiver vazio.
  - Bloquear criação se Descrição estiver vazia.
  - Bloquear criação se Critério de aceite estiver vazio.
  - Um KR de Projeto pode ter 0 ou mais requisitos, mas para cálculo de progresso é recomendável >= 1.

- **Erros/mensagens:**
  - “Título do requisito é obrigatório.”
  - “Descrição do requisito é obrigatória.”
  - “Critério de aceite do requisito é obrigatório.”

---

### RF-05 — Marcar Requisito de KR Projeto como Implementado

- **Descrição:**  
  Permitir que o usuário marque cada requisito de um KR Projeto como implementado ou não implementado.

- **Entradas:**
  - Requisito selecionado
  - Ação: marcar ou desmarcar “Implementado”

- **Saídas/Comportamento:**
  - Atualiza o status do requisito (Implementado / Não implementado).
  - Dispara recálculo do progresso do KR Projeto (RF-06).
  - Atualiza a data da última alteração do KR (para rastreabilidade).

- **Regras de validação:**
  - Requisito deve pertencer a um KR do tipo Projeto.

- **Erros/mensagens:**
  - “Requisito inválido para este tipo de KR.” (se for o caso)

---

### RF-06 — Calcular Progresso do KR Projeto

- **Descrição:**  
  Calcular de forma automática o progresso de um KR de Projeto com base no número de requisitos implementados.

- **Entradas:**
  - Lista de requisitos do KR:
    - Total de requisitos
    - Quantidade marcados como Implementado

- **Lógica (V1):**
  - Se total de requisitos = 0 → Progresso = 0%
  - Caso contrário:
    - Progresso (%) = (Requisitos implementados / Total de requisitos) * 100

- **Saídas/Comportamento:**
  - Atualiza o campo de progresso do KR.
  - Atualiza o status do KR (por exemplo:
    - 0% → “Não iniciado”
    - >0% e <100% → “Em andamento”
    - 100% → “Concluído”
  - Dispara recálculo do status do Objetivo (RF-08).

---

### RF-07 — Registrar Check-ins (Notas de Acompanhamento)

- **Descrição:**  
  Permitir registrar check-ins pontuais, com data, em Objetivos e KRs, para documentar o que aconteceu em momentos específicos (similar a comentários de Planner).

- **Entradas:**
  - Contexto:
    - Objetivo OU
    - KR
  - Texto do check-in (obrigatório)
  - (Opcional) Marcação de tipo, ex.: “Atualização semanal”, “Reunião com time”, etc. (futuro)

- **Saídas/Comportamento:**
  - Criação de um registro de check-in com:
    - ID único
    - Data/hora de criação
    - Texto
    - Referência ao Objetivo ou KR
  - Exibição em uma lista de check-ins ordenada da mais recente para a mais antiga.

- **Regras de validação:**
  - Texto não pode estar vazio.

- **Erros/mensagens:**
  - “Texto do check-in é obrigatório.”

---

### RF-08 — Calcular Status de Objetivo

- **Descrição:**  
  Determinar o status consolidado do Objetivo com base no progresso dos KRs associados.

- **Entradas:**
  - Todos os KRs do Objetivo, com seus respectivos progressos (%)

- **Lógica sugerida (V1 – simples):**
  - Calcular a média simples do progresso (%) dos KRs.
  - Mapear progresso médio para status, por exemplo:
    - 0% → “Não iniciado”
    - >0% e <50% → “Em andamento”
    - ≥50% e <100% → “Em andamento (Avançado)”
    - 100% → “Concluído”
  - (Opcional futuro: permitir ponderação por peso de KR.)

- **Saídas/Comportamento:**
  - Atualizar campo de status do Objetivo.
  - Atualizar visualização (cores, ícones etc.).

---

### RF-09 — Definir Prioridade de Objetivos

- **Descrição:**  
  Permitir classificar Objetivos em Alta, Média ou Baixa prioridade.

- **Entradas:**
  - Objetivo selecionado
  - Valor da prioridade: Alta / Média / Baixa

- **Saídas/Comportamento:**
  - Armazena a prioridade.
  - Exibe priorização na tela de lista (ex.: ícone, cor ou label).

---

### RF-10 — Tela Web Interativa em Lista

- **Descrição:**  
  Prover uma tela principal em formato de lista para visualização dos OKRs, com capacidade de filtragem simples.

- **Entradas:**
  - Filtro de Ciclo (combo ou selector)
  - Filtro de Time/Frente (combo ou selector)
  - (Opcional) Ordenação: por prioridade, por progresso ou por time

- **Saídas/Comportamento:**
  - Exibir, para cada Objetivo no ciclo/time selecionado:
    - Título
    - Time/Frente
    - Ciclo
    - Prioridade
    - Progresso (%)
    - Status
    - Quantidade de KRs
  - Permitir expandir Objetivo para mostrar KRs.
  - Permitir expandir KR de Projeto para mostrar requisitos.

---

### RF-11 — Visão Consolidada por Time/Frente

- **Descrição:**  
  Permitir que o usuário tenha uma visão consolidada de todos os Objetivos e KRs de um time específico em um determinado ciclo.

- **Entradas:**
  - Time/Frente selecionado
  - Ciclo selecionado

- **Saídas/Comportamento:**
  - Lista de Objetivos daquele time no ciclo.
  - Progresso e status de cada Objetivo e seus KRs.
  - Essa visão será base para exportação em Adaptive Card.

---

### RF-12 — Histórico de Ciclos Anteriores

- **Descrição:**  
  Permitir consultar OKRs de ciclos que não são o atual, preservando dados.

- **Entradas:**
  - Seleção de ciclo anterior na UI.

- **Saídas/Comportamento:**
  - Lista de Objetivos daquele ciclo, com seus KRs e respectivos registros.
  - Dados somente leitura (edição apenas no ciclo atual, se desejado).

---

### RF-13 — Registrar Fatos Relevantes

- **Descrição:**  
  Permitir destacar eventos importantes que impactam diretamente o Objetivo ou KR.

- **Entradas:**
  - Objetivo ou KR de contexto
  - Texto do fato relevante (obrigatório)

- **Saídas/Comportamento:**
  - Registro de fato relevante com:
    - ID
    - Data/hora de criação
    - Texto
  - Exibição em área visualmente destacada (mais evidente que check-ins comuns).

---

### RF-14 — Registrar Riscos e Pontos de Atenção

- **Descrição:**  
  Permitir registrar riscos ou pontos de atenção ligados a um Objetivo ou KR, incluindo o impacto.

- **Entradas:**
  - Objetivo ou KR de contexto
  - Descrição do risco (obrigatória)
  - Impacto:
    - Campo livre OU
    - Enum: Baixo, Médio, Alto (V1 pode ser simplesmente texto)

- **Saídas/Comportamento:**
  - Lista de riscos/pontos de atenção:
    - Data
    - Descrição
    - Impacto
  - Poder diferenciar visualmente riscos de fatos relevantes.

---

### RF-15 — Exportar OKRs para Adaptive Card (Outlook)

- **Descrição:**  
  Permitir exportar os OKRs de um time em determinado ciclo em um formato adequado para criação de um Adaptive Card no Outlook.

- **Entradas:**
  - Time/Frente selecionado
  - Ciclo selecionado
  - (Opcional) Seleção de quais Objetivos incluir

- **Saídas/Comportamento:**
  - Geração de uma estrutura (ex.: JSON ou pseudo-JSON) contendo:
    - Lista de Objetivos:
      - Título
      - Progresso
      - Status
      - (Opcional) Prioridade
    - KRs de cada Objetivo:
      - Título
      - Tipo
      - Progresso
      - Status
    - Fatos relevantes (resumo, se houver)
    - Riscos principais (resumo, se houver)
  - Pré-visualização textual/estrutural para o usuário revisar.
  - Possibilidade de copiar o conteúdo para colar no Outlook.

---

## 6. Requisitos Não Funcionais (RNF)

### RNF-01 — Preservação Total do Histórico

- O sistema deve preservar todos os registros de Objetivos, KRs, requisitos, check-ins, fatos relevantes e riscos.
- Não deve existir opção de exclusão de Objetivos e KRs antes de 2 anos de sua criação.
- Alterações não podem apagar o histórico de forma implícita.

### RNF-02 — Integridade dos Dados

- Deve ser garantido que:
  - Todo KR possui um Objetivo pai válido.
  - Todo requisito pertence a um KR do tipo Projeto.
- Operações de atualização não devem deixar registros “órfãos”.

### RNF-03 — Simplicidade de Uso

- O fluxo de criação e atualização de OKRs deve ser direto, com o mínimo de telas.
- Não deve haver necessidade de autenticação/login.

### RNF-04 — Acesso Imediato

- Ao abrir o sistema, o usuário deve ser direcionado diretamente para a visualização do ciclo atual.
- Não exigir login nem troca de contexto inicial.

### RNF-05 — Performance Básica

- Carregamento da lista principal deve ser perceptivelmente rápido (sem travamentos).
- Operações de salvar/atualizar devem ser praticamente instantâneas para o usuário.

### RNF-06 — Escalabilidade Leve

- O sistema deve suportar armazenamento de múltiplos anos de OKRs sem degradação significativa de performance percebida.

### RNF-07 — Auditabilidade

- Para Objetivos, KRs e requisitos, deve existir ao menos:
  - data de criação;
  - data da última atualização.

### RNF-08 — Hierarquia Visual

- Diferenciar claramente:
  - Check-ins (notas comuns)
  - Fatos relevantes
  - Riscos/pontos de atenção

### RNF-09 — Portabilidade

- Interface deve funcionar em navegadores modernos em desktop e, idealmente, em tablets.

### RNF-10 — Compatibilidade com Adaptive Cards

- A estrutura exportada deve seguir o schema oficial de Adaptive Cards suportado pelo Outlook.

### RNF-11 — Retenção Mínima de 2 Anos

- OKRs (Objetivos + KRs) devem ser mantidos por, no mínimo, 2 anos.
- Antes de 2 anos, não pode haver botão ou ação de exclusão para esses registros.
- Após 2 anos:
  - Pode ser apresentado um botão de “Excluir histórico” por ciclo.
  - Exclusão deve exigir confirmação explícita (ex.: “Tem certeza? Esta ação não poderá ser desfeita.”).

---

## 7. Fluxos de Usuário (User Journeys Detalhados)

### Fluxo 1 — Criar um Objetivo

- **Ator:** Usuário
- **Intenção:** Registrar um novo objetivo para um ciclo específico.

**Precondições:**
- Sistema aberto na tela principal.
- Ciclo atual selecionado por padrão.

**Passos:**
1. Usuário clica em “+ Criar Objetivo”.
2. Sistema exibe formulário com campos:
   - Título
   - Descrição
   - Ciclo (preenchido com ciclo atual)
   - Time/Frente
   - Prioridade
3. Usuário preenche Título.
4. Usuário preenche Descrição.
5. Se necessário, ajusta o Ciclo (caso não queira o atual).
6. Informa o Time/Frente (seleciona ou digita).
7. Define a Prioridade (ou aceita o padrão).
8. Clica em “Salvar”.
9. Sistema valida campos obrigatórios.
10. Se tudo ok, cria o Objetivo e retorna para a lista, mostrando o novo Objetivo.

**Pós-condições:**
- Objetivo cadastrado com progresso 0% e status “Não iniciado”.
- Objetivo visível na lista do ciclo selecionado.

**Fluxos alternativos:**
- Se Título ou Descrição não preenchidos, o sistema:
  - Exibe mensagem de erro;
  - Mantém dados já preenchidos;
  - Não cria o Objetivo até correção.

---

### Fluxo 2 — Criar um KR (Quantitativo, Qualitativo ou Projeto)

- **Ator:** Usuário
- **Intenção:** Adicionar um KR a um Objetivo existente.

**Precondições:**
- Objetivo já cadastrado.
- Usuário está na tela de detalhes do Objetivo.

**Passos:**
1. Usuário acessa o Objetivo na lista e abre seus detalhes.
2. Clica em “+ Adicionar KR”.
3. Sistema exibe formulário de KR com campos:
   - Título
   - Descrição
   - Tipo (Quantitativo, Qualitativo, Projeto)
4. Usuário preenche Título.
5. Usuário preenche Descrição.
6. Usuário seleciona Tipo:
   - Se **Quantitativo**, pode preencher alvo e valor inicial (opcional).
   - Se **Qualitativo**, pode detalhar critérios de avaliação (na descrição).
   - Se **Projeto**, sabe que detalhará requisitos a seguir (Fluxo 4).
7. Usuário clica em “Salvar”.
8. Sistema valida obrigatórios.
9. Sistema cria o KR e atualiza a tela do Objetivo, exibindo o novo KR.

**Pós-condições:**
- KR associado ao Objetivo.
- Progresso do KR = 0%, status = “Não iniciado”.

**Fluxos alternativos:**
- Campos obrigatórios vazios → erro e permanência na tela de edição.

---

### Fluxo 3 — Atualizar Progresso de KR Quantitativo/Qualitativo

- **Ator:** Usuário
- **Intenção:** Atualizar manualmente o avanço de um KR.

**Precondições:**
- KR do tipo Quantitativo ou Qualitativo já existe.

**Passos:**
1. Usuário acessa o Objetivo.
2. Localiza o KR desejado.
3. Clica em ação “Atualizar Progresso”.
4. Sistema exibe modal com:
   - Campo para valor de progresso (%)
   - Campo opcional de nota
5. Usuário insere o novo valor.
6. (Opcional) Escreve rápida observação.
7. Clica em “Salvar”.
8. Sistema valida o valor.
9. Sistema atualiza o progresso do KR.
10. Sistema atualiza status do KR (se necessário).
11. Sistema recalcula status do Objetivo.
12. Sistema fecha modal e mostra valor atualizado na tela.

**Pós-condições:**
- Progresso do KR atualizado com data da última alteração.
- Status do Objetivo refletindo nova média de progresso dos KRs.

---

### Fluxo 4 — Adicionar Requisitos a um KR Projeto

- **Ator:** Usuário
- **Intenção:** Decompor um KR Projeto em requisitos menores.

**Precondições:**
- KR do tipo Projeto já cadastrado.

**Passos:**
1. Usuário acessa o Objetivo e, em seguida, o KR Projeto.
2. Na seção “Requisitos”, clica em “+ Adicionar requisito”.
3. Sistema exibe formulário:
   - Título
   - Descrição
   - Critério de aceite
4. Usuário preenche Título.
5. Usuário preenche Descrição.
6. Usuário preenche Critério de aceite.
7. Clica em “Salvar”.
8. Sistema valida campos obrigatórios.
9. Sistema adiciona o requisito à lista, com status “Não implementado”.
10. Usuário pode repetir o processo para adicionar outros requisitos.

**Pós-condições:**
- KR Projeto passa a ter uma lista de requisitos associados.
- Progresso ainda pode ser 0% até que requisitos sejam marcados como implementados.

---

### Fluxo 5 — Marcar Requisito de KR Projeto como Implementado

- **Ator:** Usuário
- **Intenção:** Atualizar o avanço do KR Projeto.

**Precondições:**
- KR Projeto com pelo menos um requisito cadastrado.

**Passos:**
1. Usuário acessa o KR Projeto.
2. Visualiza a lista de requisitos.
3. Identifica o requisito que foi concluído.
4. Marca a caixa “Implementado”.
5. Sistema registra a mudança de status do requisito.
6. Sistema recalcula o progresso do KR:
   - Conta quantos requisitos estão implementados.
   - Atualiza a % conforme RF-06.
7. Sistema atualiza a barra de progresso do KR Projeto.
8. Sistema recalcula status do Objetivo, se aplicável.

**Pós-condições:**
- Progresso do KR Projeto refletindo a quantidade de requisitos concluídos.
- Estado do Objetivo atualizado de acordo com os KRs.

---

### Fluxo 6 — Registrar um Check-in (Objetivo ou KR)

- **Ator:** Usuário
- **Intenção:** Registrar uma nota de acompanhamento em um momento específico.

**Precondições:**
- Objetivo ou KR já existindo.

**Passos:**
1. Usuário abre um Objetivo OU um KR.
2. Clica em “Registrar check-in” ou “+ Check-in”.
3. Sistema exibe campo de texto.
4. Usuário digita a nota (ex.: “Reunião com time X – foi decidido Y.”).
5. Clica em “Salvar”.
6. Sistema registra:
   - Texto
   - Data/hora
7. Sistema exibe o check-in na lista, até o momento ordenada do mais recente para o mais antigo.

**Pós-condições:**
- Novo check-in visível e vinculado ao Objetivo/KR.

---

### Fluxo 7 — Registrar Fato Relevante

- **Ator:** Usuário
- **Intenção:** Destacar um evento importante associado ao Objetivo ou KR.

**Precondições:**
- Objetivo ou KR existente.

**Passos:**
1. Usuário abre Objetivo ou KR.
2. Navega até seção “Fatos Relevantes”.
3. Clica em “+ Adicionar fato relevante”.
4. Sistema exibe campo de texto.
5. Usuário descreve o fato (ex.: “Mudança de escopo aprovada pelo superintendente.”).
6. Clica em “Salvar”.
7. Sistema registra:
   - Texto
   - Data/hora
8. Sistema exibe o fato em área destacada (ícone, cor ou card diferenciado).

**Pós-condições:**
- Fato relevante visível e associado ao Objetivo/KR.

---

### Fluxo 8 — Registrar Risco ou Ponto de Atenção

- **Ator:** Usuário
- **Intenção:** Registrar riscos e impactos.

**Precondições:**
- Objetivo ou KR existente.

**Passos:**
1. Usuário abre Objetivo ou KR.
2. Vai à seção “Riscos e Pontos de Atenção”.
3. Clica em “+ Adicionar risco”.
4. Sistema exibe campos:
   - Descrição do risco
   - Impacto (texto ou nível)
5. Usuário preenche a descrição.
6. Usuário preenche o impacto (ex.: “Alto – pode impedir conclusão do KR.”).
7. Clica em “Salvar”.
8. Sistema adiciona o risco à lista, com data.

**Pós-condições:**
- Risco fica registrado e visualmente identificado.

---

### Fluxo 9 — Visualizar OKRs por Time ou Ciclo

- **Ator:** Usuário
- **Intenção:** Ver o panorama consolidado dos OKRs por time e por ciclo.

**Precondições:**
- Existência de Objetivos no ciclo.

**Passos:**
1. Usuário abre o sistema.
2. Por padrão, a tela mostra o ciclo atual selecionado.
3. Usuário vê lista de Objetivos:
   - Título
   - Time
   - Progresso
   - Status
   - Prioridade
4. Usuário utiliza filtro de Time para restringir a visão a um time específico.
5. Opcionalmente, usuário troca o Ciclo para visualizar ciclos anteriores.
6. Usuário pode expandir um Objetivo para ver seus KRs.
7. Pode expandir um KR Projeto para ver seus requisitos.

**Pós-condições:**
- Usuário obtém rapidamente a visão da situação de um time no ciclo atual ou anterior.
---


### Fluxo 10 — Exportar OKRs como Adaptive Card para Outlook

- **Ator:** Usuário
- **Intenção:** Preparar conteúdo em formato de Adaptive Card para enviar por e-mail à superintendência.

**Precondições:**
- Objetivos e KRs de um time já cadastrados no ciclo selecionado.

**Passos:**
1. Usuário acessa a visualização filtrada por time e ciclo.
2. Confere os Objetivos listados.
3. Clica em botão “Exportar para Adaptive Card”.
4. Sistema compila dados:
   - Objetivos do time no ciclo selecionado.
   - KRs de cada Objetivo.
   - Progresso e status de Objetivos e KRs.
   - (Opcional) Fatos relevantes e riscos resumidos.
5. Sistema exibe uma prévia estruturada (ex.: JSON ou pseudo-JSON).
6. Usuário revisa a prévia.
7. Usuário clica em “Copiar” ou “Exportar”.
8. Usuário abre Outlook, inicia e-mail para o superintendente.
9. Cola o conteúdo no corpo do e-mail (conforme o padrão de Adaptive Card adotado).
10. Superintendente recebe e visualiza o card com os Objetivos/KRs e seus estados.

**Pós-condições:**
- Conteúdo de OKRs do time disponível em formato adequado para leitura direta no Outlook.

---

## 8. Critérios de Aceitação (Macro)

1. **CA-01 — Cadastro de Objetivos e KRs completo e utilizável**
   - Usuário consegue criar Objetivo e KRs de todos os tipos sem erros.
   - Tela principal exibe Objetivos com seus KRs corretamente.

2. **CA-02 — KRs de Projeto plenamente funcionais**
   - É possível cadastrar requisitos com título, descrição e critério de aceite.
   - Progresso do KR Projeto é atualizado automaticamente ao marcar requisitos como implementados.

3. **CA-03 — Status de Objetivo refletindo progresso dos KRs**
   - Atualizar qualquer KR (quantitativo, qualitativo ou projeto) altera o status do Objetivo de forma coerente.

4. **CA-04 — Notas, fatos e riscos utilizáveis na prática**
   - Usuário consegue registrar check-ins, fatos relevantes e riscos sem fricção.
   - Esses registros aparecem claramente nos detalhes do Objetivo/KR.

5. **CA-05 — Tela principal em lista com filtros por time e ciclo**
   - Ao abrir o sistema, o usuário tem visão do ciclo atual.
   - Filtros de time e ciclo funcionam corretamente.

6. **CA-06 — Exportação em formato Adaptive Card funcional**
   - Usuário consegue gerar conteúdo e colar no Outlook.
   - Card é legível e compreensível pela superintendência.

7. **CA-07 — Preservação de histórico e ausência de exclusão acidental**
   - Não há funcionalidade de exclusão de OKRs antes de 2 anos.
   - Usuário consegue visualizar ciclos anteriores sem perda de dados.

8. **CA-08 — Uso real em um ciclo completo**
   - Usuário consegue usar o OKR Tracker durante um ciclo inteiro sem precisar de ferramentas paralelas para registrar OKRs.

---

## 9. Métricas de Sucesso

### Métricas Primárias

- **MS-01 — Redução de retrabalho de consolidação**
  - Evidência: menos uso de planilhas e documentos paralelos para preparar apresentações.
- **MS-02 — Clareza rápida do estado dos times**
  - Evidência: usuário consegue responder “como está o time X no ciclo atual?” em menos de 1 minuto ao abrir o sistema.
- **MS-03 — Acompanhamento consistente via check-ins**
  - Proporção de Objetivos/KRs com ao menos um check-in por ciclo.

### Métricas Secundárias

- **MS-04 — Uso efetivo de KRs Projeto**
  - Percentual de KRs Projeto com requisitos e progresso atualizado ao longo do ciclo.

- **MS-05 — Uso de Fatos Relevantes e Riscos**
  - Número médio de fatos relevantes e riscos registrados por Objetivo em ciclos importantes.

### Métricas Saudáveis

- **MS-06 — Zero perda de histórico**
  - Incidentes de perda de dados ou exclusão indevida = 0.

- **MS-07 — Uso contínuo**
  - O sistema é utilizado com frequência (semanal) durante o ciclo.

---

## 10. Pressupostos & Dependências

### Pressupostos

1. Sistema será usado por **apenas 1 usuário**.
2. Não haverá necessidade de autenticação/login.
3. Interface será acessada via navegador em desktop.
4. Maturidade de métricas continuará relativamente baixa no curto prazo, justificando KRs Projeto.
5. Exportação de Adaptive Card será feita via copiar/colar para o Outlook.

### Dependências

1. **Compatibilidade com Adaptive Cards no Outlook.**
2. **Ambiente de execução estável**, com capacidade de persistir dados localmente ou em armazenamento simples.
3. **Navegador moderno** (Edge, Chrome, Firefox).

---

## 11. Riscos & Mitigações

1. **Risco:** perda de histórico por corrupção de arquivo ou falha de armazenamento.  
   - **Mitigação:** backups periódicos; formatos robustos de persistência; ausência de exclusão antes de 2 anos.

2. **Risco:** crescimento grande de volume de dados ao longo dos anos.  
   - **Mitigação:** separação clara entre ciclo atual e ciclos antigos; otimização de carregamento.

3. **Risco:** inconsistência entre tipos de KRs.  
   - **Mitigação:** regras de validação simples, lógicas claras para cada tipo; testes de uso.

4. **Risco:** alterações futuras no suporte do Outlook a Adaptive Cards.  
   - **Mitigação:** aderir ao schema oficial; modularizar lógica de exportação.

5. **Risco:** tela ficar visualmente poluída.  
   - **Mitigação:** visão em lista resumida, detalhes apenas ao expandir.

6. **Risco:** falta de disciplina na atualização se o processo for burocrático.  
   - **Mitigação:** reduzir campos obrigatórios ao essencial; tornar atualização de progresso e check-ins muito rápida.

---

## 12. Anexos / Observações

- (Reservado para anexos futuros, exemplos de Adaptive Cards, prints de tela, etc.)

---

_Fim do PRD — OKR Tracker._