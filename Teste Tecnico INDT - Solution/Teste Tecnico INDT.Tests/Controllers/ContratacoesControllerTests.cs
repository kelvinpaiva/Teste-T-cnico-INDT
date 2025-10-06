using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Teste_Tecnico_INDT.Api;
using Teste_Tecnico_INDT.Aplicacao;
using Teste_Tecnico_INDT.Controllers;
using Teste_Tecnico_INDT.Dominio;
using Xunit;

namespace Teste_Tecnico_INDT.Tests.Controllers;

public class ContratacoesControllerTests
{
    private readonly Mock<IServicoContratacao> _mockServico;
    private readonly ContratacoesController _controller;

    public ContratacoesControllerTests()
    {
        _mockServico = new Mock<IServicoContratacao>();
        _controller = new ContratacoesController(_mockServico.Object);
    }

    [Fact]
    public async Task Criar_ComPropostaValida_DeveRetornarCreated()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var request = new CriarContratacaoRequest { PropostaId = propostaId };
        var contratacao = new Contratacao(propostaId);
        
        _mockServico.Setup(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(contratacao);

        // Act
        var result = await _controller.Criar(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ContratacaoResponse>(createdResult.Value);
        Assert.Equal(contratacao.Id, response.Id);
        Assert.Equal(propostaId, response.PropostaId);
        Assert.Equal(contratacao.DataContratacaoUtc, response.DataContratacaoUtc);
        
        _mockServico.Verify(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_ComPropostaInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var request = new CriarContratacaoRequest { PropostaId = propostaId };
        
        _mockServico.Setup(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Contratacao?)null);

        // Act
        var result = await _controller.Criar(request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockServico.Verify(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_ComPropostaIdVazio_DeveRetornarCreated()
    {
        // Arrange
        var propostaId = Guid.Empty;
        var request = new CriarContratacaoRequest { PropostaId = propostaId };
        var contratacao = new Contratacao(propostaId);
        
        _mockServico.Setup(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(contratacao);

        // Act
        var result = await _controller.Criar(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ContratacaoResponse>(createdResult.Value);
        Assert.Equal(contratacao.Id, response.Id);
        Assert.Equal(propostaId, response.PropostaId);
    }

    [Fact]
    public async Task Listar_ComContratacoesExistentes_DeveRetornarLista()
    {
        // Arrange
        var contratacoes = new List<Contratacao>
        {
            new Contratacao(Guid.NewGuid()),
            new Contratacao(Guid.NewGuid())
        };
        
        _mockServico.Setup(s => s.ListarAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(contratacoes);

        // Act
        var result = await _controller.Listar(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responses = Assert.IsAssignableFrom<IEnumerable<ContratacaoResponse>>(okResult.Value);
        var responseList = responses.ToList();
        
        Assert.Equal(2, responseList.Count);
        Assert.Equal(contratacoes[0].Id, responseList[0].Id);
        Assert.Equal(contratacoes[1].Id, responseList[1].Id);
        
        _mockServico.Verify(s => s.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_ComListaVazia_DeveRetornarListaVazia()
    {
        // Arrange
        var contratacoes = new List<Contratacao>();
        
        _mockServico.Setup(s => s.ListarAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(contratacoes);

        // Act
        var result = await _controller.Listar(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responses = Assert.IsAssignableFrom<IEnumerable<ContratacaoResponse>>(okResult.Value);
        var responseList = responses.ToList();
        
        Assert.Empty(responseList);
        _mockServico.Verify(s => s.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_ComExcecaoNoServico_DevePropagarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var request = new CriarContratacaoRequest { PropostaId = propostaId };
        var expectedException = new InvalidOperationException("Erro interno");
        
        _mockServico.Setup(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Criar(request, CancellationToken.None));
        
        Assert.Equal("Erro interno", exception.Message);
        _mockServico.Verify(s => s.ContratarAsync(propostaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_ComExcecaoNoServico_DevePropagarExcecao()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Erro interno");
        
        _mockServico.Setup(s => s.ListarAsync(It.IsAny<CancellationToken>()))
                   .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Listar(CancellationToken.None));
        
        Assert.Equal("Erro interno", exception.Message);
        _mockServico.Verify(s => s.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
