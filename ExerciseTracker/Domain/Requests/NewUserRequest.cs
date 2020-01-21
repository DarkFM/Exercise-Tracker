using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Domain.Requests
{
    public class NewUserRequest
    {
        [Required]
        [FromForm(Name = "username")]
        public string Username { get; set; }
    }
}
