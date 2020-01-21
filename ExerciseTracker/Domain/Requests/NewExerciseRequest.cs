using ExerciseTracker.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExerciseTracker.Domain.Requests
{
    public class NewExerciseRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public double Duration { get; set; }

        [BindingBehavior(BindingBehavior.Optional)]
        [BindProperty(BinderType = typeof(CustomDateTimeBinder))]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
