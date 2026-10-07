using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Perfumes.Api.Services;
public sealed class ApiExceptionHandler(IProblemDetailsService problems, ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var sql = exception as SqlException ?? exception.InnerException as SqlException;
        var status = exception is RegraException regra ? regra.Status
            : sql?.Number is 2601 or 2627 or 1205 || exception is DbUpdateConcurrencyException ? 409 : 500;
        var mensagem = exception is RegraException regra2 ? regra2.Message
            : status == 409 ? "Conflito de gravação. Atualize os dados e tente novamente."
            : "Não foi possível concluir a operação.";
        if (status == 500) logger.LogError(exception, "Erro na API: {TraceId}", context.TraceIdentifier);
        context.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext {
            HttpContext = context,
            ProblemDetails = new ProblemDetails { Status = status, Title = mensagem, Extensions = { ["traceId"] = context.TraceIdentifier } }
        });
    }
}

