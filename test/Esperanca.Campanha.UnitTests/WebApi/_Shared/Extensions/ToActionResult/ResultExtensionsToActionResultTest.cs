using Esperanca.Campanha.Application._Shared.Results;
using Esperanca.Campanha.WebApi._Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Esperanca.Campanha.UnitTests.WebApi._Shared.Extensions.ToActionResult;

public class ResultExtensionsToActionResultTest
{
    private static readonly FakeController _controller = new();

    [Fact]
    public void ToActionResult_WhenResultIsOk_ThenReturn200ComDados()
    {
        var result = Result<string>.Ok("valor");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(200);
        objectResult.Value.ShouldBe("valor");
    }

    [Fact]
    public void ToActionResult_WhenResultIsCreated_ThenReturn201ComDados()
    {
        var result = Result<string>.Created("criado");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(201);
        objectResult.Value.ShouldBe("criado");
    }

    [Fact]
    public void ToActionResult_WhenResultIsAccepted_ThenReturn202ComDados()
    {
        var result = Result<string>.Accepted("aceito");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(202);
        objectResult.Value.ShouldBe("aceito");
    }

    [Fact]
    public void ToActionResult_WhenResultIsFail_ThenReturn400ComErro()
    {
        var result = Result<string>.Fail("Campanha:001");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(400);
        objectResult.Value.ShouldBe("Campanha:001");
    }

    [Fact]
    public void ToActionResult_WhenResultIsNotFound_ThenReturn404ComErro()
    {
        var result = Result<string>.NotFound("Campanha:901");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(404);
        objectResult.Value.ShouldBe("Campanha:901");
    }

    [Fact]
    public void ToActionResult_WhenResultIsUnauthorized_ThenReturn401ComErro()
    {
        var result = Result<string>.Unauthorized("Campanha:902");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(401);
        objectResult.Value.ShouldBe("Campanha:902");
    }

    [Fact]
    public void ToActionResult_WhenResultIsForbidden_ThenReturn403ComErro()
    {
        var result = Result<string>.Forbidden("Campanha:902");

        var objectResult = result.ToActionResult(_controller).ShouldBeOfType<ObjectResult>();

        objectResult.StatusCode.ShouldBe(403);
        objectResult.Value.ShouldBe("Campanha:902");
    }

    private sealed class FakeController : ControllerBase { }
}
