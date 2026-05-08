using Esperanca.Campanha.Application._Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace Esperanca.Campanha.WebApi._Shared.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller) =>
        result.Sucesso
            ? controller.StatusCode(result.StatusCode, result.Dados)
            : controller.StatusCode(result.StatusCode, result.Erro);
}
