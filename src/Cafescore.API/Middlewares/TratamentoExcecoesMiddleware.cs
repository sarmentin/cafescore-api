using System.Net;
using System.Text.Json;
using Cafescore.Domain.Exceptions;

namespace Cafescore.API.Middlewares;

public class TratamentoExcecoesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TratamentoExcecoesMiddleware> _logger;

    public TratamentoExcecoesMiddleware(
        RequestDelegate next,
        ILogger<TratamentoExcecoesMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarExcecaoAsync(context, ex);
        }
    }

    private async Task TratarExcecaoAsync(HttpContext context, Exception ex)
    {
        var (status, mensagem) = ex switch
        {
            // Regras de negócio: mensagem segura para exibir
            RegraDeNegocioException => (HttpStatusCode.BadRequest, ex.Message),

            // Acesso negado
            UnauthorizedAccessException => (HttpStatusCode.Forbidden,
                "Você não tem permissão para realizar esta ação."),

            // Qualquer outra coisa: mensagem genérica
            _ => (HttpStatusCode.InternalServerError,
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };

        // Erros inesperados vão para o log com todos os detalhes
        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(ex, "Erro não tratado em {Caminho}", context.Request.Path);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new { mensagem }));
    }
}