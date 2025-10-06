# Teste Técnico INDT - Seguro (PT-BR)

## Como executar (sem Docker)

1. Requisitos: .NET 8 SDK
2. Restaurar/compilar:
```bash
cd "Teste Tecnico INDT - Solution"
dotnet build
```
3. Executar API:
```bash
cd "Teste Tecnico INDT - Solution/Teste Tecnico INDT"
dotnet run
```
4. Swagger: http://localhost:5171/swagger (ou porta mostrada no console)

O app aplica migrações automaticamente na inicialização e cria o banco SQLite `propostas.db`.

## Executar com Docker

1. Build da imagem:
```bash
cd "Teste Tecnico INDT - Solution"
docker build -t indt/seguro:latest -f "Teste Tecnico INDT/Dockerfile" .
```
2. Rodar o container:
```bash
docker run -p 8080:8080 --name seguro indt/seguro:latest
```
- A aplicação inicia, aplica migrações (SQLite) e expõe Swagger em http://localhost:8080/swagger
- O arquivo `propostas.db` fica no filesystem interno do container. Para persistência, monte um volume:
```bash
docker run -p 8080:8080 -v %CD%/data:/app/data -e ConnectionStrings__Default="Data Source=/app/data/propostas.db" indt/seguro:latest
```

## Endpoints principais

- Propostas
  - POST `/api/propostas`
  - GET `/api/propostas`
  - GET `/api/propostas/{id}`
  - POST `/api/propostas/{id}/aprovar`
  - POST `/api/propostas/{id}/rejeitar`
- Contratações
  - POST `/api/contratacoes`
  - GET `/api/contratacoes`

Corpos (exemplos):
```json
POST /api/propostas
{
  "nomeCliente": "Maria Silva",
  "produto": "Auto"
}
```
```json
POST /api/contratacoes
{
  "propostaId": "<guid-da-proposta-aprovada>"
}
```

## Arquitetura (Hexagonal simplificada)

```
+---------------------------+
|           API             |
|  Controllers (REST)       |
+-------------+-------------+
              |
              v
+---------------------------+
|        Aplicação          |
|  Serviços (regras orques.)|
+-------------+-------------+
              |
              v
+---------------------------+
|          Domínio          |
|  Entidades/Agregados      |
+-------------+-------------+
              |
              v
+---------------------------+
|       Infraestrutura      |
|  EF Core (SQLite), Repos  |
+---------------------------+
```

- Domínio: `Proposta`, `StatusProposta`, `Contratacao`
- Aplicação: `ServicoProposta`, `ServicoContratacao`
- Infraestrutura: `AppDbContext`, `PropostaRepositorio`, `ContratacaoRepositorio`
- API: `PropostasController`, `ContratacoesController`

## Migrações

O app aplica `db.Database.Migrate()` no startup. Para criar/atualizar migrações localmente:
```bash
cd "Teste Tecnico INDT - Solution/Teste Tecnico INDT"
dotnet ef migrations add NovaMudanca
```

## Observações
- Banco padrão: SQLite `Data Source=propostas.db` (configurável em `appsettings.json`).
