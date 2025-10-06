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

public class PropostasIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PropostasIntegrationTests(WebApplicationFactory<Program> factory)
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
    public async Task POST_Propostas_ComDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/propostas", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var propostaResponse = JsonSerializer.Deserialize<PropostaResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(propostaResponse);
        Assert.Equal("João Silva", propostaResponse.NomeCliente);
        Assert.Equal("Seguro Auto", propostaResponse.Produto);
        Assert.Equal(StatusProposta.EmAnalise, propostaResponse.Status);
        Assert.NotEqual(Guid.Empty, propostaResponse.Id);
    }

    [Fact]
    public async Task POST_Propostas_ComDadosInvalidos_DeveRetornarBadRequest()
    {
        // Arrange
        var request = new CriarPropostaRequest
        {
            NomeCliente = "", // Nome vazio
            Produto = "Seguro Auto"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/propostas", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GET_Propostas_DeveRetornarListaDePropostas()
    {
        // Arrange - Criar algumas propostas primeiro
        var proposta1 = new CriarPropostaRequest { NomeCliente = "João Silva", Produto = "Seguro Auto" };
        var proposta2 = new CriarPropostaRequest { NomeCliente = "Maria Santos", Produto = "Seguro Residencial" };
        
        await _client.PostAsJsonAsync("/api/propostas", proposta1);
        await _client.PostAsJsonAsync("/api/propostas", proposta2);

        // Act
        var response = await _client.GetAsync("/api/propostas");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var propostas = JsonSerializer.Deserialize<List<PropostaResponse>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(propostas);
        Assert.True(propostas.Count >= 2);
        Assert.Contains(propostas, p => p.NomeCliente == "João Silva");
        Assert.Contains(propostas, p => p.NomeCliente == "Maria Santos");
    }

    [Fact]
    public async Task GET_Propostas_Id_ComIdValido_DeveRetornarProposta()
    {
        // Arrange - Criar uma proposta primeiro
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/propostas", request);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var propostaCriada = JsonSerializer.Deserialize<PropostaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.GetAsync($"/api/propostas/{propostaCriada!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<PropostaResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(proposta);
        Assert.Equal(propostaCriada.Id, proposta.Id);
        Assert.Equal("João Silva", proposta.NomeCliente);
        Assert.Equal("Seguro Auto", proposta.Produto);
    }

    [Fact]
    public async Task GET_Propostas_Id_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/propostas/{idInexistente}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Propostas_Id_Aprovar_ComIdValido_DeveRetornarPropostaAprovada()
    {
        // Arrange - Criar uma proposta primeiro
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/propostas", request);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var propostaCriada = JsonSerializer.Deserialize<PropostaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.PostAsync($"/api/propostas/{propostaCriada!.Id}/aprovar", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<PropostaResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(proposta);
        Assert.Equal(propostaCriada.Id, proposta.Id);
        Assert.Equal(StatusProposta.Aprovada, proposta.Status);
    }

    [Fact]
    public async Task POST_Propostas_Id_Aprovar_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/propostas/{idInexistente}/aprovar", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Propostas_Id_Rejeitar_ComIdValido_DeveRetornarPropostaRejeitada()
    {
        // Arrange - Criar uma proposta primeiro
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/propostas", request);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var propostaCriada = JsonSerializer.Deserialize<PropostaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Act
        var response = await _client.PostAsync($"/api/propostas/{propostaCriada!.Id}/rejeitar", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var proposta = JsonSerializer.Deserialize<PropostaResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(proposta);
        Assert.Equal(propostaCriada.Id, proposta.Id);
        Assert.Equal(StatusProposta.Rejeitada, proposta.Status);
    }

    [Fact]
    public async Task POST_Propostas_Id_Rejeitar_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/propostas/{idInexistente}/rejeitar", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Propostas_Id_Aprovar_ComPropostaJaAprovada_DeveRetornarBadRequest()
    {
        // Arrange - Criar uma proposta e aprová-la
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/propostas", request);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var propostaCriada = JsonSerializer.Deserialize<PropostaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Aprovar a proposta
        await _client.PostAsync($"/api/propostas/{propostaCriada!.Id}/aprovar", null);

        // Act - Tentar aprovar novamente
        var response = await _client.PostAsync($"/api/propostas/{propostaCriada.Id}/aprovar", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_Propostas_Id_Rejeitar_ComPropostaJaRejeitada_DeveRetornarBadRequest()
    {
        // Arrange - Criar uma proposta e rejeitá-la
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/propostas", request);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var propostaCriada = JsonSerializer.Deserialize<PropostaResponse>(createContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Rejeitar a proposta
        await _client.PostAsync($"/api/propostas/{propostaCriada!.Id}/rejeitar", null);

        // Act - Tentar rejeitar novamente
        var response = await _client.PostAsync($"/api/propostas/{propostaCriada.Id}/rejeitar", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
