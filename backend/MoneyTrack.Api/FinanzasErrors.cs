using Microsoft.AspNetCore.Diagnostics;
using MoneyTrack.Application;
namespace MoneyTrack.Api;
public sealed class FinanzasErrors : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx,Exception exception,CancellationToken ct)
    {
        if(exception is not FinanzasException error) return false;
        await Results.Problem(detail:error.Message,statusCode:error.Status).ExecuteAsync(ctx);
        return true;
    }
}
