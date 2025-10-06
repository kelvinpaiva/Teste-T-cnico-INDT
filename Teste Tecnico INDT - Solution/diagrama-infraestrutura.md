# Diagrama de Infraestrutura - Sistema de Seguros

## Visão Geral da Arquitetura

```mermaid
graph TB
    subgraph "Cliente"
        C[Cliente/Usuário]
    end
    
    subgraph "API Layer"
        API[API REST<br/>ASP.NET Core]
        SWAGGER[Swagger UI<br/>Documentação]
    end
    
    subgraph "Application Layer"
        SP[ServicoProposta<br/>Regras de Negócio]
        SC[ServicoContratacao<br/>Regras de Negócio]
    end
    
    subgraph "Domain Layer"
        P[Proposta<br/>Entidade]
        C_ENT[Contratacao<br/>Entidade]
        STATUS[StatusProposta<br/>Enum]
    end
    
    subgraph "Infrastructure Layer"
        DB[(SQLite Database<br/>propostas.db)]
        EF[Entity Framework Core<br/>ORM]
        REPO_P[PropostaRepositorio]
        REPO_C[ContratacaoRepositorio]
    end
    
    subgraph "Deployment"
        DOCKER[Docker Container<br/>Porta 8080]
        LOCAL[Local Development<br/>Porta 5171]
    end
    
    C --> API
    API --> SWAGGER
    API --> SP
    API --> SC
    SP --> P
    SC --> C_ENT
    P --> STATUS
    SP --> REPO_P
    SC --> REPO_C
    REPO_P --> EF
    REPO_C --> EF
    EF --> DB
    
    API --> DOCKER
    API --> LOCAL
    
    style API fill:#e1f5fe
    style DB fill:#f3e5f5
    style DOCKER fill:#e8f5e8
    style LOCAL fill:#fff3e0
```

## Componentes Principais

### 1. **API Layer**
- **Controllers**: `PropostasController`, `ContratacoesController`
- **DTOs**: `CriarPropostaRequest`, `PropostaResponse`, `CriarContratacaoRequest`, `ContratacaoResponse`
- **Swagger**: Documentação automática da API

### 2. **Application Layer**
- **Serviços**: Orquestração de regras de negócio
- **Interfaces**: `IServicoProposta`, `IServicoContratacao`

### 3. **Domain Layer**
- **Entidades**: `Proposta`, `Contratacao`
- **Enums**: `StatusProposta` (EmAnalise, Aprovada, Rejeitada)
- **Validações**: Regras de domínio encapsuladas

### 4. **Infrastructure Layer**
- **Database**: SQLite com arquivo `propostas.db`
- **ORM**: Entity Framework Core
- **Repositórios**: Implementação de persistência
- **Migrations**: Controle de versão do banco

### 5. **Deployment Options**
- **Docker**: Containerização com porta 8080
- **Local**: Desenvolvimento com porta 5171
- **Auto-migration**: Aplicação automática de migrações no startup

## Fluxo de Dados

1. **Cliente** faz requisição HTTP para a **API**
2. **Controller** recebe e valida a requisição
3. **Serviço** processa as regras de negócio
4. **Repositório** persiste/recupera dados via **Entity Framework**
5. **SQLite** armazena os dados
6. **Response** é retornada ao cliente

## Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **ASP.NET Core**: Web API
- **Entity Framework Core**: ORM
- **SQLite**: Banco de dados
- **Docker**: Containerização
- **Swagger**: Documentação da API
