using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ChallengeTrackerApi.Errors;

public class GlobalExceptionHandler(IProblemDetailsService problems, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = 500;
        var title = "An unexpected error occurred.";
        Dictionary<string, string[]>? errors = null;
        if (exception is ApiException api)
        {
            status = api.StatusCode;
            title = api.Message;
            errors = api.Errors;
        }
        else if (exception is DbUpdateConcurrencyException)
        {
            status = 409;
            title = "The record changed. Reload it and try again.";
        }
        else if (exception is DbUpdateException update && IsUniqueConflict(update.InnerException))
        {
            status = 409;
            title = "This membership or daily progress entry already exists.";
        }
        else if (IsWriteConflict(exception) || exception is DbUpdateException updateConflict && IsWriteConflict(updateConflict.InnerException))
        {
            status = 409;
            title = "Another request changed this data. Please retry.";
        }
        else if (exception is BadHttpRequestException badRequest)
        {
            status = badRequest.StatusCode;
            title = "The request could not be read. Check its format and values.";
        }
        if (status == 500) logger.LogError(exception, "Unhandled API error for {Path}", context.Request.Path);
        var details = new ProblemDetails { Status = status, Title = title, Instance = context.Request.Path };
        if (errors is not null) details.Extensions["errors"] = errors;
        context.Response.StatusCode = status;
        await problems.WriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = details });
        return true;
    }

    private static bool IsWriteConflict(Exception? exception) => exception switch
    {
        SqliteException sqlite => sqlite.SqliteErrorCode is 5 or 6,
        SqlException sql => sql.Number is 1205 or 3960 or 547,
        _ => false
    };

    private static bool IsUniqueConflict(Exception? exception) => exception switch
    {
        SqliteException sqlite => sqlite.SqliteErrorCode == 19,
        SqlException sql => sql.Number is 2601 or 2627,
        _ => false
    };
}
