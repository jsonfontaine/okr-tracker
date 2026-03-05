TED — Technical Specification Document (TED)
OKR Tracker — Sistema Pessoal de Gestão de OKRs

1. Título
OKR Tracker — Sistema Pessoal de Gestão de OKRs

2. Descrição da demanda
O objetivo é desenvolver um sistema pessoal, simples e portátil, para uso individual de um gestor operacional multifrentes, que precisa:

Registrar Objetivos e KRs (Key Results);
Suportar três tipos de KR: Quantitativo, Qualitativo e Requisito;
Registrar progresso dos KRs;
Calcular automaticamente o progresso e o status dos Objetivos;
Registrar Comentários, Fatos Relevantes e Riscos associados a Objetivos e KRs;
Manter histórico por vários anos;
Exportar OKRs em formato compatível com Adaptive Cards (Outlook);
Utilizar um banco de dados LiteDB em arquivo .db, cujo caminho completo será informado pelo usuário ao iniciar o sistema;
Rodar como um monólito leve (frontend + backend) em um único container Docker.

Características principais:

Uso single-user (apenas o próprio autor);
Sem autenticação;
Frontend em React;
Backend em C# / ASP.NET Core;
Banco LiteDB (arquivo .db externo ao container, versionável em git);
Sistema acessado localmente (localhost).


3. Serviços de aplicação
Esta seção lista os serviços de aplicação (casos de uso) que orquestram o domínio.
3.1. Lista de serviços de aplicação


ConfigurarBaseDeDadosService

Objetivo: Receber o caminho completo do arquivo .db, validar se o LiteDB consegue abrir e registrar esse caminho como base ativa.



CriarCicloService

Objetivo: Criar um novo ciclo (ex.: 2026-Q1).



ListarCiclosService

Objetivo: Listar todos os ciclos existentes.



AtualizarCicloService

Objetivo: Atualizar o nome de um ciclo existente.



ExcluirCicloService

Objetivo: Excluir um ciclo apenas se não houver objetivos associados.



CriarTimeService

Objetivo: Criar um novo time ou frente.



ListarTimesService

Objetivo: Listar todos os times cadastrados.



AtualizarTimeService

Objetivo: Atualizar nome/descrição de um time.



ExcluirTimeService

Objetivo: Excluir um time apenas se não houver objetivos associados.



CriarObjetivoService

Objetivo: Criar um objetivo vinculado a um ciclo e um time, com prioridade, farol, intruder, descoberta tardia, etc.



AtualizarObjetivoService

Objetivo: Atualizar campos do objetivo (título, descrição, ciclo, time, prioridade, farol, intruder, descoberta tardia).



ListarOKRsPorTimeECicloService

Objetivo: Listar objetivos filtrados obrigatoriamente por cicloId e timeId, já incluindo KRs e eventos.



CriarKRService

Objetivo: Criar um KR vinculado a um objetivo, com tipo (Quantitativo, Qualitativo, Requisito).



AtualizarKRService

Objetivo: Atualizar campos do KR (título, descrição, tipo, farol, intruder, descoberta tardia).



AtualizarProgressoKRService

Objetivo: Atualizar o progresso de um KR (validação especial para tipo Requisito).



ExcluirKRService

Objetivo: Excluir um KR garantindo que o objetivo continue com pelo menos 1 KR.



CriarComentarioService

Objetivo: Registrar um comentário em um objetivo ou em um KR.



CriarFatoRelevanteService

Objetivo: Registrar um fato relevante em um objetivo ou KR.



CriarRiscoService

Objetivo: Registrar um risco (com impacto) em um objetivo ou KR.



ExportarAdaptiveCardService

Objetivo: Gerar o JSON de Adaptive Card para um determinado time e ciclo.




4. Configurações e feature flags
4.1. Configuração da base de dados

Ao acessar o sistema pela primeira vez, o frontend exibe uma tela solicitando:

Campo: Caminho completo do arquivo .db (string).


Exemplo em modo debug (Windows):

C:\Users\jason\source\okr-tracker\okr-tracker.db


Exemplo em Docker (arquivo montado em /data):

/data/okr-tracker.db



Fluxo:

Usuário informa o caminho completo;
Frontend chama POST /api/config/database com { databasePath };
Backend tenta abrir o LiteDB com esse caminho:

Em caso de sucesso:

Armazena o caminho em um provider interno (IDatabasePathProvider);
Retorna success = true.


Em caso de erro:

Retorna success = false e uma mensagem amigável.





4.2. Feature flags
Nenhuma feature flag específica na V1.

5. Modelagem de dados
5.1. Entidades de domínio
5.1.1. Ciclo

Propriedades:

Id (string)
Nome (string, obrigatório, único)
DataCriacao (datetime)
UltimaAtualizacao (datetime)



5.1.2. Time

Propriedades:

Id (string)
Nome (string, obrigatório, único)
Descricao (string, opcional)
DataCriacao (datetime)
UltimaAtualizacao (datetime)



5.1.3. Objetivo


Propriedades:

Id (string)
Titulo (string, obrigatório)
Descricao (string, obrigatória)
CicloId (string, obrigatório)
TimeId (string, obrigatório)
Prioridade (string: Alta, Média, Baixa)
Progresso (double, 0–100, calculado)
Status (string)
Farol (string: Verde, Amarelo, Vermelho)
Intruder (bool)
DescobertaTardia (bool)
DataCriacao (datetime)
UltimaAtualizacao (datetime)



Invariantes:

Objetivo deve ter pelo menos 1 KR.
CicloId e TimeId devem referenciar registros válidos.



5.1.4. Key Result (KR)


Propriedades:

Id (string)
ObjetivoId (string, obrigatório)
Titulo (string, obrigatório)
Descricao (string, obrigatória)
Tipo (string: Quantitativo, Qualitativo, Requisito)
Progresso (double, 0–100)
Status (string)
Farol (string: Verde, Amarelo, Vermelho)
Intruder (bool)
DescobertaTardia (bool)
DataCriacao (datetime)
UltimaAtualizacao (datetime)



Invariantes:

Pertence a exatamente 1 Objetivo.
Progresso entre 0 e 100.
Se Tipo = Requisito, Progresso só pode ser 0 ou 100. Qualquer outro valor deve ser rejeitado.



5.1.5. Comentário


Propriedades:

Id (string)
Texto (string, obrigatório)
ObjetivoId (string, opcional)
KrId (string, opcional)
DataCriacao (datetime)



Regra:

Exatamente um entre ObjetivoId e KrId deve ser preenchido.



5.1.6. Fato Relevante

Propriedades:

Id (string)
Texto (string, obrigatório)
ObjetivoId (string, opcional)
KrId (string, opcional)
DataCriacao (datetime)



5.1.7. Risco

Propriedades:

Id (string)
Descricao (string, obrigatória)
Impacto (string, opcional)
ObjetivoId (string, opcional)
KrId (string, opcional)
DataCriacao (datetime)




5.2. Modelo lógico no LiteDB
Coleções:

ciclos
times
objetivos
krs
comentarios
fatosRelevantes
riscos

Enumerações (Prioridade, Farol, Tipo, Status) são armazenadas como string.
Índices:

ciclos: índice único em nome
times: índice único em nome
objetivos: índice composto em (cicloId, timeId)
krs: índice em objetivoId
comentarios, fatosRelevantes, riscos: índices em objetivoId e krId


6. Contratos das APIs
Formato: JSON.
6.1. Configuração
POST /api/config/database
Request:
{
"databasePath": "C:\Users\jason\source\okr-tracker.db"
}
Response sucesso:
{
"success": true
}
Response erro:
{
"success": false,
"message": "Não foi possível abrir a base de dados."
}

6.2. Ciclos
GET /api/ciclos

Retorna lista de ciclos.

POST /api/ciclos
Request:
{
"nome": "2026-Q1"
}
PUT /api/ciclos/{id}
Request:
{
"nome": "2026-Q2"
}
DELETE /api/ciclos/{id}

Só permite exclusão se não houver objetivos associados ao ciclo.


6.3. Times
GET /api/times
POST /api/times
PUT /api/times/{id}
DELETE /api/times/{id}

Mesma ideia de Ciclos, com validação de nome único e impedindo exclusão se houver objetivos vinculados.


6.4. Objetivos
GET /api/okr?cicloId={cicloId}&timeId={timeId}

Filtros cicloId e timeId são obrigatórios.
Retorna uma lista de Objetivos com:

Dados do objetivo
Lista de KRs
Comentários, Fatos Relevantes e Riscos associados



POST /api/objetivos
Request (exemplo):
{
"titulo": "Melhorar previsibilidade de entregas",
"descricao": "Criar processos e métricas...",
"cicloId": "ciclo-2026-q1",
"timeId": "time-bridge",
"prioridade": "Alta",
"farol": "Verde",
"intruder": false,
"descobertaTardia": false
}
PUT /api/objetivos/{id}

Permite atualizar título, descrição, cicloId, timeId, prioridade, farol, intruder, descobertaTardia.


6.5. KRs
POST /api/krs
Request:
{
"objetivoId": "obj-123",
"titulo": "Atingir 80% de cumprimento de prazos",
"descricao": "Monitorar entregas...",
"tipo": "Quantitativo",
"progresso": 0,
"farol": "Verde",
"intruder": false,
"descobertaTardia": false
}
PUT /api/krs/{id}

Atualiza campos do KR, exceto objetivoId.

PUT /api/krs/{id}/progresso
Request:
{
"progresso": 45
}
Regra:

Valida se progresso está entre 0 e 100.
Se tipo = Requisito, progresso só pode ser 0 ou 100; outros valores geram erro de validação.

DELETE /api/krs/{id}

Impedir exclusão se este for o último KR do objetivo correspondente.


6.6. Comentários
POST /api/comentarios
Request:
{
"objetivoId": "obj-123",
"krId": null,
"texto": "Reunião com o time; ajustamos o escopo."
}
Regras:

Texto obrigatório.
Exatamente um entre objetivoId e krId deve ser preenchido.


6.7. Fatos Relevantes
POST /api/fatos-relevantes
Request:
{
"objetivoId": "obj-123",
"krId": null,
"texto": "Mudança de escopo aprovada pela diretoria."
}

6.8. Riscos
POST /api/riscos
Request:
{
"objetivoId": null,
"krId": "kr-123",
"descricao": "Dependência de outro time para liberação de ambiente.",
"impacto": "Alto - pode atrasar 2 semanas."
}

6.9. Export Adaptive Card
GET /api/export/adaptive-card?cicloId={cicloId}&timeId={timeId}

Retorna:

{
"success": true,
"data": {
"type": "AdaptiveCard",
"version": "1.5",
"body": [
...
]
}
}

7. Integrações com recursos externos

V1 não possui integrações diretas com sistemas externos (Planner, Jira, etc.).
Exportação para Adaptive Card é feita via JSON, que o usuário copia e cola no Outlook.


8. Diretrizes transversais
8.1. Segurança

Aplicação roda apenas localmente (localhost);
Sem autenticação;
Usar HTTP simples;
Validar entradas (principalmente caminho do .db e valores numéricos de progresso);
No frontend React, não usar dangerouslySetInnerHTML.

8.2. Performance

Volume pequeno de dados (uso pessoal);
Índices em LiteDB para:

ciclos.nome
times.nome
objetivos (cicloId, timeId)
krs.objetivoId


Sem necessidade de paginação na V1.

8.3. Resiliência

try/catch ao abrir arquivo .db;
Mensagens claras ao usuário em caso de erro;
Regras de negócio fortes impedindo:

Objetivo sem KR;
Exclusão de ciclo ou time com objetivos.



8.4. Observabilidade

Usar logging padrão do ASP.NET Core;
Logar:

Tentativas de configuração de base;
Erros de validação;
Exceções ao acessar LiteDB.



8.5. Escalabilidade

Arquitetura desenhada para single-user e single-instance;
Sem cache distribuído, sem mensageria;
Caso seja necessário multiusuário no futuro, será preciso revisar arquitetura e persistência.


9. Riscos técnicos e mitigações


Caminho de .db inválido:

Mitigação: validação na API de configuração; mensagens claras.



Corrupção do arquivo .db:

Mitigação: versionar o arquivo .db em git; fazer backups periódicos.



Violação de invariantes (Objetivo sem KR, progresso inválido):

Mitigação: regras fortes na camada de aplicação; testes de domínio.



Integridade referencial (registros órfãos):

Mitigação: ao excluir Objetivo (quando permitido), excluir KRs e eventos; ao excluir KR, excluir eventos associados.



Base não configurada:

Mitigação: impedir uso dos demais endpoints se a base ainda não foi configurada; retornar erro claro.



Caminho de .db em Docker confuso:

Mitigação: documentar claramente que dentro do container o caminho será /data/...; explicar isso na tela de configuração e no README.




10. Cenários de teste
10.1. Configuração da base


Caminho válido:

Deve retornar success = true.



Caminho inválido:

Deve retornar success = false + mensagem.



Reconfiguração após erro:

Após falha, permitir nova tentativa com caminho válido e funcionar.



10.2. Ciclos

Criar ciclo com nome válido;
Impedir nome vazio;
Impedir nome duplicado;
Impedir exclusão se houver objetivos associados.

10.3. Times

Criar time válido;
Impedir nome vazio;
Impedir nome duplicado;
Impedir exclusão se houver objetivos associados.

10.4. Objetivos

Criar objetivo com cicloId e timeId válidos;
Impedir criação sem cicloId ou timeId;
Atualizar objetivo trocando ciclo e time;
Impedir exclusão antes de 2 anos (se a funcionalidade de exclusão for implementada).

10.5. KRs

Criar KR com objetivoId válido;
Impedir criação com objetivo inexistente;
Atualizar progresso de KR Quantitativo/Qualitativo (0–100);
Impedir progresso fora da faixa [0, 100];
Para tipo Requisito:

Impedir progresso diferente de 0 ou 100;


Impedir exclusão do último KR de um objetivo.

10.6. Eventos (comentários, fatos, riscos)

Criar comentário em objetivo;
Criar comentário em KR;
Impedir texto vazio;
Criar fato relevante;
Criar risco com impacto;
Garantir que outros fluxos não apaguem eventos inadvertidamente.

10.7. Listagem de OKRs


Chamar GET /api/okr sem cicloId ou timeId:

Deve retornar erro de validação ou 400.



Chamar GET /api/okr com cicloId e timeId:

Deve retornar objetivos + KRs + eventos corretos.



10.8. Export Adaptive Card


Exportar quando há dados:

Deve retornar JSON estruturado com objetivos e KRs.



Exportar sem dados:

Pode retornar card vazio ou com mensagem “Não há OKRs cadastrados para este time/ciclo”.