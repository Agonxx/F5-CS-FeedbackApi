# F5-CS-FeedbackApi

Microsserviço de feedback do doador da plataforma **Conexão Solidária** (Hackathon FIAP Pós Tech, Fase 5). Repositório-irmão de [conexao-solidaria](https://github.com/Agonxx/conexao-solidaria), que concentra a documentação e as decisões do projeto.

## Responsabilidades

- O `Doador` logado avalia uma doação sua: rapidez (1–5), dificuldade (1–5), se pretende voltar a doar (`Sim`/`Talvez`/`Nao`) e comentário opcional
- **Um feedback por doação**, e só do dono dela
- O `GestorONG` lê um resumo por campanha: total de feedbacks, média de rapidez e dificuldade, % que pretende voltar

## Por que MongoDB aqui

SQL Server guarda o que é transacional (usuários, campanhas, doações). O feedback é um documento de formato flexível: as perguntas podem mudar sem migração de schema. A unicidade por doação é garantida por índice único em `IdDoacao`.

## Como funciona a checagem de dono

A doação mora no SQL Server da CampanhasApi, que este serviço não acessa. Ao receber um feedback, a FeedbackApi chama `GET /api/Doacao/MinhasDoacoes` na CampanhasApi **repassando o JWT do doador** e só aceita `IdDoacao` que apareça nessa lista. Sem essa lista a requisição falha (a CampanhasApi precisa estar de pé).

O JWT é gerado pelo [F5-CS-UsersApi](https://github.com/Agonxx/F5-CS-UsersApi); aqui ele só é validado (mesma `JwtSettings:SecretKey`).

## Stack

.NET 9, MongoDB (driver oficial), JWT Bearer, Swagger, Prometheus (`/metrics`).

## Como rodar localmente

Pré-requisito: Docker Desktop, com a CampanhasApi e o UsersApi no ar.

```
docker compose up -d --build
```

Sobe a API (porta 5004) e o MongoDB (27017). Swagger em `http://localhost:5004/swagger`. A URL da CampanhasApi vem de `CampanhasApi__BaseUrl` (no compose, `http://host.docker.internal:5002/`; em cluster, o nome do Service).

## Endpoints

| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/Feedback/Enviar` | `Doador` — `{ "idDoacao", "rapidez", "dificuldade", "pretendeVoltar", "comentario" }` |
| GET | `/api/Feedback/Meus` | `Doador` |
| GET | `/api/Feedback/Resumo` | `GestorONG` |

`pretendeVoltar` é numérico, como os demais enums do projeto: `1` = Sim, `2` = Talvez, `3` = Nao.

## Testes

```
dotnet test FeedbackApi.Tests/FeedbackApi.Tests.csproj
```
