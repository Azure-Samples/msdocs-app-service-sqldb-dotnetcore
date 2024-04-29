using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetCoreSqlDb;

public class ActionTimerFilterAttribute : ActionFilterAttribute
{
    private readonly Stopwatch _stopwatch = new();

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch.Start();
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        if (context.Result is ViewResult) ((ViewResult)context.Result).ViewData["TimeElapsed"] = _stopwatch.Elapsed;
        _stopwatch.Reset();
    }
}