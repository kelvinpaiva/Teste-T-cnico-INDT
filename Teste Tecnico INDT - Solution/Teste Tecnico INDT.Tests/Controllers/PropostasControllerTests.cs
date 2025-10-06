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

public class PropostasControllerTests
{
    private readonly Mock<IServicoProposta> _mockServico;
    private readonly PropostasController _controller;

    public PropostasControllerTests()
    {
        _mockServico = new Mock<IServicoProposta>();
        _controller = new PropostasController(_mockServico.Object);
    }

    [Fact]
    public async Task Criar_ComDadosValidos_DeveRetornarCreated()
    {
        // Arrange
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        var proposta = new Proposta("João Silva", "Seguro Auto");
        
        _mockServico.Setup(s => s.CriarAsync("João Silva", "Seguro Auto", It.IsAny<CancellationToken>()))
                   .ReturnsAsync(proposta);

        // Act
        var result = await _controller.Criar(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<PropostaResponse>(createdResult.Value);
        Assert.Equal(proposta.Id, response.Id);
        Assert.Equal("João Silva", response.NomeCliente);
        Assert.Equal("Seguro Auto", response.Produto);
        Assert.Equal(StatusProposta.EmAnalise, response.Status);
        
        _mockServico.Verify(s => s.CriarAsync("João Silva", "Seguro Auto", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_ComNomeClienteVazio_DeveRetornarCreated()
    {
        // Arrange
        var request = new CriarPropostaRequest
        {
            NomeCliente = "",
            Produto = "Seguro Auto"
        };
        var proposta = new Proposta("", "Seguro Auto");
        
        _mockServico.Setup(s => s.CriarAsync("", "Seguro Auto", It.IsAny<CancellationToken>()))
                   .ReturnsAsync(proposta);

        // Act
        var result = await _controller.Criar(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<PropostaResponse>(createdResult.Value);
        Assert.Equal("", response.NomeCliente);
        Assert.Equal("Seguro Auto", response.Produto);
    }

    [Fact]
    public async Task Listar_ComPropostasExistentes_DeveRetornarLista()
    {
        // Arrange
        var propostas = new List<Proposta>
        {
            new Proposta("João Silva", "Seguro Auto"),
            new Proposta("Maria Santos", "Seguro Residencial")
        };
        
        _mockServico.Setup(s => s.ListarAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(propostas);

        // Act
        var result = await _controller.Listar(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responses = Assert.IsAssignableFrom<IEnumerable<PropostaResponse>>(okResult.Value);
        var responseList = responses.ToList();
        
        Assert.Equal(2, responseList.Count);
        Assert.Equal(propostas[0].Id, responseList[0].Id);
        Assert.Equal(propostas[1].Id, responseList[1].Id);
        Assert.Equal("João Silva", responseList[0].NomeCliente);
        Assert.Equal("Maria Santos", responseList[1].NomeCliente);
        
        _mockServico.Verify(s => s.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_ComListaVazia_DeveRetornarListaVazia()
    {
        // Arrange
        var propostas = new List<Proposta>();
        
        _mockServico.Setup(s => s.ListarAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(propostas);

        // Act
        var result = await _controller.Listar(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var responses = Assert.IsAssignableFrom<IEnumerable<PropostaResponse>>(okResult.Value);
        var responseList = responses.ToList();
        
        Assert.Empty(responseList);
        _mockServico.Verify(s => s.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Obter_ComIdValido_DeveRetornarProposta()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proposta = new Proposta("João Silva", "Seguro Auto");
        
        _mockServico.Setup(s => s.ObterAsync(id, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(proposta);

        // Act
        var result = await _controller.Obter(id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PropostaResponse>(okResult.Value);
        Assert.Equal(proposta.Id, response.Id);
        Assert.Equal("João Silva", response.NomeCliente);
        Assert.Equal("Seguro Auto", response.Produto);
        
        _mockServico.Verify(s => s.ObterAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Obter_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        _mockServico.Setup(s => s.ObterAsync(id, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Proposta?)null);

        // Act
        var result = await _controller.Obter(id, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockServico.Verify(s => s.ObterAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Aprovar_ComIdValido_DeveRetornarPropostaAprovada()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proposta = new Proposta("João Silva", "Seguro Auto");
        proposta.Aprovar();
        
        _mockServico.Setup(s => s.AprovarAsync(id, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(proposta);

        // Act
        var result = await _controller.Aprovar(id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PropostaResponse>(okResult.Value);
        Assert.Equal(proposta.Id, response.Id);
        Assert.Equal(StatusProposta.Aprovada, response.Status);
        
        _mockServico.Verify(s => s.AprovarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Aprovar_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        _mockServico.Setup(s => s.AprovarAsync(id, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Proposta?)null);

        // Act
        var result = await _controller.Aprovar(id, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockServico.Verify(s => s.AprovarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Rejeitar_ComIdValido_DeveRetornarPropostaRejeitada()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proposta = new Proposta("João Silva", "Seguro Auto");
        proposta.Rejeitar();
        
        _mockServico.Setup(s => s.RejeitarAsync(id, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(proposta);

        // Act
        var result = await _controller.Rejeitar(id, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PropostaResponse>(okResult.Value);
        Assert.Equal(proposta.Id, response.Id);
        Assert.Equal(StatusProposta.Rejeitada, response.Status);
        
        _mockServico.Verify(s => s.RejeitarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Rejeitar_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        _mockServico.Setup(s => s.RejeitarAsync(id, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((Proposta?)null);

        // Act
        var result = await _controller.Rejeitar(id, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        _mockServico.Verify(s => s.RejeitarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_ComExcecaoNoServico_DevePropagarExcecao()
    {
        // Arrange
        var request = new CriarPropostaRequest
        {
            NomeCliente = "João Silva",
            Produto = "Seguro Auto"
        };
        var expectedException = new InvalidOperationException("Erro interno");
        
        _mockServico.Setup(s => s.CriarAsync("João Silva", "Seguro Auto", It.IsAny<CancellationToken>()))
                   .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Criar(request, CancellationToken.None));
        
        Assert.Equal("Erro interno", exception.Message);
        _mockServico.Verify(s => s.CriarAsync("João Silva", "Seguro Auto", It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact]
    public async Task Obter_ComExcecaoNoServico_DevePropagarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Erro interno");
        
        _mockServico.Setup(s => s.ObterAsync(id, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Obter(id, CancellationToken.None));
        
        Assert.Equal("Erro interno", exception.Message);
        _mockServico.Verify(s => s.ObterAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Aprovar_ComExcecaoNoServico_DevePropagarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Erro interno");
        
        _mockServico.Setup(s => s.AprovarAsync(id, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Aprovar(id, CancellationToken.None));
        
        Assert.Equal("Erro interno", exception.Message);
        _mockServico.Verify(s => s.AprovarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Rejeitar_ComExcecaoNoServico_DevePropagarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Erro interno");
        
        _mockServico.Setup(s => s.RejeitarAsync(id, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Rejeitar(id, CancellationToken.None));
        
        Assert.Equal("Erro interno", exception.Message);
        _mockServico.Verify(s => s.RejeitarAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
