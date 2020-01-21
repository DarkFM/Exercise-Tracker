using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ExerciseTracker.Models
{
    public class ErrorModel
    {
        public ErrorModel(string title, int status, IDictionary<string, string[]> errors)
        {
            Title = title;
            Status = status;
            Errors = errors;
        }

        public ErrorModel(ActionContext context)
        {
            Title = "Invalid arguments to the API";
            Status = 400;
            Errors = context
                .ModelState
                .Where(kvp => kvp.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        public string Title { get; set; }
        public int Status { get; set; }
        public IDictionary<string, string[]> Errors { get; }

    }
}
