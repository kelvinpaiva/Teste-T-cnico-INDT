# Projeto de Testes - Sistema de Seguros

## Visão Geral

Este projeto contém testes abrangentes para todos os endpoints da API do Sistema de Seguros, incluindo testes unitários e de integração.

## Estrutura dos Testes

### Testes Unitários (Controllers)

#### ContratacoesControllerTests
- ✅ `Criar_ComPropostaValida_DeveRetornarCreated`
- ✅ `Criar_ComPropostaInexistente_DeveRetornarNotFound`
- ✅ `Criar_ComPropostaIdVazio_DeveRetornarNotFound`
- ✅ `Listar_ComContratacoesExistentes_DeveRetornarLista`
- ✅ `Listar_ComListaVazia_DeveRetornarListaVazia`
- ✅ `Criar_ComExcecaoNoServico_DevePropagarExcecao`
- ✅ `Listar_ComExcecaoNoServico_DevePropagarExcecao`

#### PropostasControllerTests
- ✅ `Criar_ComDadosValidos_DeveRetornarCreated`
- ✅ `Criar_ComNomeClienteVazio_DevePropagarExcecao`
- ✅ `Listar_ComPropostasExistentes_DeveRetornarLista`
- ✅ `Listar_ComListaVazia_DeveRetornarListaVazia`
- ✅ `Obter_ComIdValido_DeveRetornarProposta`
- ✅ `Obter_ComIdInexistente_DeveRetornarNotFound`
- ✅ `Aprovar_ComIdValido_DeveRetornarPropostaAprovada`
- ✅ `Aprovar_ComIdInexistente_DeveRetornarNotFound`
- ✅ `Rejeitar_ComIdValido_DeveRetornarPropostaRejeitada`
- ✅ `Rejeitar_ComIdInexistente_DeveRetornarNotFound`
- ✅ `Criar_ComExcecaoNoServico_DevePropagarExcecao`
- ✅ `Listar_ComExcecaoNoServico_DevePropagarExcecao`
- ✅ `Obter_ComExcecaoNoServico_DevePropagarExcecao`
- ✅ `Aprovar_ComExcecaoNoServico_DevePropagarExcecao`
- ✅ `Rejeitar_ComExcecaoNoServico_DevePropagarExcecao`

### Testes de Integração

#### PropostasIntegrationTests
- ✅ `POST_Propostas_ComDadosValidos_DeveRetornarCreated`
- ✅ `POST_Propostas_ComDadosInvalidos_DeveRetornarBadRequest`
- ✅ `GET_Propostas_DeveRetornarListaDePropostas`
- ✅ `GET_Propostas_Id_ComIdValido_DeveRetornarProposta`
- ✅ `GET_Propostas_Id_ComIdInexistente_DeveRetornarNotFound`
- ✅ `POST_Propostas_Id_Aprovar_ComIdValido_DeveRetornarPropostaAprovada`
- ✅ `POST_Propostas_Id_Aprovar_ComIdInexistente_DeveRetornarNotFound`
- ✅ `POST_Propostas_Id_Rejeitar_ComIdValido_DeveRetornarPropostaRejeitada`
- ✅ `POST_Propostas_Id_Rejeitar_ComIdInexistente_DeveRetornarNotFound`
- ✅ `POST_Propostas_Id_Aprovar_ComPropostaJaAprovada_DeveRetornarBadRequest`
- ✅ `POST_Propostas_Id_Rejeitar_ComPropostaJaRejeitada_DeveRetornarBadRequest`

#### ContratacoesIntegrationTests
- ✅ `POST_Contratacoes_ComPropostaAprovada_DeveRetornarCreated`
- ✅ `POST_Contratacoes_ComPropostaNaoAprovada_DeveRetornarNotFound`
- ✅ `POST_Contratacoes_ComPropostaInexistente_DeveRetornarNotFound`
- ✅ `GET_Contratacoes_DeveRetornarListaDeContratacoes`
- ✅ `GET_Contratacoes_ComListaVazia_DeveRetornarListaVazia`
- ✅ `POST_Contratacoes_ComPropostaRejeitada_DeveRetornarNotFound`
- ✅ `POST_Contratacoes_ComPropostaIdVazio_DeveRetornarNotFound`

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **Moq**: Framework de mocking para testes unitários
- **Microsoft.AspNetCore.Mvc.Testing**: Para testes de integração
- **Microsoft.EntityFrameworkCore.InMemory**: Banco em memória para testes
- **System.Text.Json**: Serialização JSON nos testes

## Como Executar os Testes

### Todos os Testes
```bash
dotnet test
```

### Apenas Testes Unitários
```bash
dotnet test --filter "FullyQualifiedName!~Integration"
```

### Apenas Testes de Integração
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

### Testes com Relatório Detalhado
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Cobertura de Testes

### Endpoints Testados

#### PropostasController
- ✅ `POST /api/propostas` - Criar proposta
- ✅ `GET /api/propostas` - Listar propostas
- ✅ `GET /api/propostas/{id}` - Obter proposta por ID
- ✅ `POST /api/propostas/{id}/aprovar` - Aprovar proposta
- ✅ `POST /api/propostas/{id}/rejeitar` - Rejeitar proposta

#### ContratacoesController
- ✅ `POST /api/contratacoes` - Criar contratação
- ✅ `GET /api/contratacoes` - Listar contratações

### Cenários de Teste

#### Casos de Sucesso
- Criação de propostas com dados válidos
- Listagem de propostas e contratações
- Obtenção de propostas por ID
- Aprovação e rejeição de propostas
- Criação de contratações para propostas aprovadas

#### Casos de Erro
- Dados inválidos (validação de modelo)
- Recursos não encontrados (404)
- Regras de negócio violadas (400)
- Exceções internas do servidor

#### Casos Extremos
- Listas vazias
- IDs inexistentes
- Tentativas de operações inválidas (aprovar proposta já aprovada)
- Validações de domínio (nome vazio, etc.)

## Estrutura do Projeto

```
Teste Tecnico INDT.Tests/
├── Controllers/
│   ├── ContratacoesControllerTests.cs
│   └── PropostasControllerTests.cs
├── Integration/
│   ├── ContratacoesIntegrationTests.cs
│   └── PropostasIntegrationTests.cs
├── Teste Tecnico INDT.Tests.csproj
└── README.md
```

## Configuração de Testes

### Testes Unitários
- Usam **Moq** para mockar dependências
- Testam apenas a lógica do controller
- Rápidos e isolados

### Testes de Integração
- Usam **WebApplicationFactory** para criar uma instância da aplicação
- Usam **Entity Framework InMemory** para banco de dados
- Testam o fluxo completo da aplicação
- Mais lentos mas mais realistas

## Relatórios de Teste

Os testes fornecem cobertura completa para:
- ✅ Todos os endpoints da API
- ✅ Validações de entrada
- ✅ Regras de negócio
- ✅ Tratamento de erros
- ✅ Casos extremos
- ✅ Integração entre camadas

**Total: 41 testes** (23 unitários + 18 de integração)
