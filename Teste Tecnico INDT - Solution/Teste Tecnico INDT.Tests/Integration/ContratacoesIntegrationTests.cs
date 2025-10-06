using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Teste_Tecnico_INDT.Api;
using Teste_Tecnico_INDT.Dominio;
using Teste_Tecnico_INDT.Infraestrutura;
using Xunit;

namespace Teste_Tecnico_INDT.Tests.Integration;

public class ContratacoesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ContratacoesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Adiciona o DbContext em memória para testes
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task POST_Contratacoes_ComPropostaAprovada_DeveRetornarCreated()
    {
        // Arrange - Criar uma proposta e aprová-la
        var propostaRequest = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var propostaResponse = await _client.PostAsJsonAsync("/api/propostas", propostaRequest);
        var propostaContent = await propostaResponse.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<PropostaResponse>(propostaContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Aprovar a proposta
        await _client.PostAsync($"/api/propostas/{proposta!.Id}/aprovar", null);

        var contratacaoRequest = new CriarContratacaoRequest
        {
            PropostaId = proposta.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contratacoes", contratacaoRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var contratacaoResponse = JsonSerializer.Deserialize<ContratacaoResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(contratacaoResponse);
        Assert.Equal(proposta.Id, contratacaoResponse.PropostaId);
        Assert.NotEqual(Guid.Empty, contratacaoResponse.Id);
        Assert.True(contratacaoResponse.DataContratacaoUtc <= DateTime.UtcNow);
    }

    [Fact]
    public async Task POST_Contratacoes_ComPropostaNaoAprovada_DeveRetornarNotFound()
    {
        // Arrange - Criar uma proposta (sem aprovar)
        var propostaRequest = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var propostaResponse = await _client.PostAsJsonAsync("/api/propostas", propostaRequest);
        var propostaContent = await propostaResponse.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<PropostaResponse>(propostaContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var contratacaoRequest = new CriarContratacaoRequest
        {
            PropostaId = proposta!.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contratacoes", contratacaoRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Contratacoes_ComPropostaInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var propostaIdInexistente = Guid.NewGuid();
        var contratacaoRequest = new CriarContratacaoRequest
        {
            PropostaId = propostaIdInexistente
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contratacoes", contratacaoRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_Contratacoes_DeveRetornarListaDeContratacoes()
    {
        // Arrange - Criar algumas propostas, aprová-las e contratá-las
        var proposta1 = new CriarPropostaRequest { NomeCliente = "João Silva", Produto = "Seguro Auto" };
        var proposta2 = new CriarPropostaRequest { NomeCliente = "Maria Santos", Produto = "Seguro Residencial" };
        
        var proposta1Response = await _client.PostAsJsonAsync("/api/propostas", proposta1);
        var proposta1Content = await proposta1Response.Content.ReadAsStringAsync();
        var proposta1Data = JsonSerializer.Deserialize<PropostaResponse>(proposta1Content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var proposta2Response = await _client.PostAsJsonAsync("/api/propostas", proposta2);
        var proposta2Content = await proposta2Response.Content.ReadAsStringAsync();
        var proposta2Data = JsonSerializer.Deserialize<PropostaResponse>(proposta2Content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Aprovar as propostas
        await _client.PostAsync($"/api/propostas/{proposta1Data!.Id}/aprovar", null);
        await _client.PostAsync($"/api/propostas/{proposta2Data!.Id}/aprovar", null);

        // Contratar as propostas
        await _client.PostAsJsonAsync("/api/contratacoes", new CriarContratacaoRequest { PropostaId = proposta1Data.Id });
        await _client.PostAsJsonAsync("/api/contratacoes", new CriarContratacaoRequest { PropostaId = proposta2Data.Id });

        // Act
        var response = await _client.GetAsync("/api/contratacoes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var contratacoes = JsonSerializer.Deserialize<List<ContratacaoResponse>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(contratacoes);
        Assert.True(contratacoes.Count >= 2);
        Assert.Contains(contratacoes, c => c.PropostaId == proposta1Data.Id);
        Assert.Contains(contratacoes, c => c.PropostaId == proposta2Data.Id);
    }

    [Fact]
    public async Task GET_Contratacoes_ComListaVazia_DeveRetornarListaVazia()
    {
        // Act
        var response = await _client.GetAsync("/api/contratacoes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var contratacoes = JsonSerializer.Deserialize<List<ContratacaoResponse>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(contratacoes);
        Assert.Empty(contratacoes);
    }

    [Fact]
    public async Task POST_Contratacoes_ComPropostaRejeitada_DeveRetornarNotFound()
    {
        // Arrange - Criar uma proposta e rejeitá-la
        var propostaRequest = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var propostaResponse = await _client.PostAsJsonAsync("/api/propostas", propostaRequest);
        var propostaContent = await propostaResponse.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<PropostaResponse>(propostaContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Rejeitar a proposta
        await _client.PostAsync($"/api/propostas/{proposta!.Id}/rejeitar", null);

        var contratacaoRequest = new CriarContratacaoRequest
        {
            PropostaId = proposta.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contratacoes", contratacaoRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Contratacoes_ComPropostaIdVazio_DeveRetornarNotFound()
    {
        // Arrange
        var contratacaoRequest = new CriarContratacaoRequest
        {
            PropostaId = Guid.Empty
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contratacoes", contratacaoRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
