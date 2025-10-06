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
# Porta padrão (se disponível)
docker run -d -p 8080:8080 --name seguro indt/seguro:latest

# Ou usar porta alternativa se 8080 estiver ocupada
docker run -d -p 8081:8080 --name seguro indt/seguro:latest
```

3. Verificar se está funcionando:
```bash
# Verificar logs
docker logs seguro

# Verificar se o container está rodando
docker ps

# Testar a API
curl http://localhost:8080/api/propostas
# ou
curl http://localhost:8081/api/propostas
```

4. Parar o container:
```bash
docker stop seguro
docker rm seguro
```

**Observações:**
- A aplicação inicia, aplica migrações (SQLite) automaticamente e expõe a API
- Swagger disponível em: http://localhost:8080/swagger (ou porta alternativa)
- O arquivo `propostas.db` fica no filesystem interno do container (`/app/propostas.db`)
- Para persistência, monte um volume:
```bash
docker run -d -p 8080:8080 -v %CD%/data:/app/data -e ConnectionStrings__Default="Data Source=/app/data/propostas.db" --name seguro indt/seguro:latest
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

## Executar Testes

O projeto inclui testes unitários e de integração completos:

```bash
# Todos os testes
dotnet test

# Apenas testes unitários
dotnet test --filter "FullyQualifiedName!~Integration"

# Apenas testes de integração
dotnet test --filter "FullyQualifiedName~Integration"

# Testes com relatório detalhado
dotnet test --logger "console;verbosity=detailed"
```

**Cobertura de Testes:**
- ✅ 23 testes unitários (Controllers)
- ✅ 18 testes de integração (Endpoints completos)
- ✅ 100% dos endpoints testados
- ✅ Cenários de sucesso, erro e casos extremos

## Observações
- Banco padrão: SQLite `Data Source=propostas.db` (configurável em `appsettings.json`)
- Migrações aplicadas automaticamente no startup
- Aplicação testada e funcionando no Docker
- Projeto de testes incluído na solution

## Diagramas

O projeto inclui diagramas detalhados da arquitetura:

- **`diagrama-infraestrutura.md`**: Visão geral da infraestrutura e componentes
- **`diagrama-aplicacao-detalhado.md`**: Diagrama detalhado com fluxos de dados e sequências

Estes diagramas mostram:
- Arquitetura em camadas (API, Aplicação, Domínio, Infraestrutura)
- Fluxos de processo detalhados
- Estrutura de dados
- Regras de negócio implementadas
- Configurações de ambiente
