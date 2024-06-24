using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace DotNetCoreSqlDb
{
    public class ActionTimerFilterAttribute : ActionFilterAttribute
    {
        readonly Stopwatch _stopwatch = new Stopwatch();

        public override void OnActionExecuting(ActionExecutingContext context) => _stopwatch.Start();

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            if (context.Result is ViewResult)
            {
                ((ViewResult)context.Result).ViewData["TimeElapsed"] = _stopwatch.Elapsed;
            }
            _stopwatch.Reset();
        }
    }
}