using ExerciseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExerciseTracker.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var dict = new Dictionary<string, string[]>
            {
                ["errors"] = new[] { context.Exception.Message }
            };
            var error = new ErrorModel("Bad Request", 400, dict);
            context.Result = new BadRequestObjectResult(error);
        }
    }
}
