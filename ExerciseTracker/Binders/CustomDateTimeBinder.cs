using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace ExerciseTracker.Binders
{

    public class CustomDateTimeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // the name of the model this is bound to (eg PropertyName)
            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name. Gets the value the the model should recieve
            // value provider is a special value-type wrapper for the string value recieved
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            // check if no value 
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            // sets the value for the ModelState Entry
            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            // extracts the value from the valueProvider
            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // perform the model binding
            
            // add to model state if value cannot be parsed as datetime
            if(!DateTime.TryParse(value, out var dt))
            {
                bindingContext.ModelState.TryAddModelError(modelName, "Invalid date format");
                return Task.CompletedTask;
            }

            // pass the newly minted datetime object to the model to be bound
            bindingContext.Result = ModelBindingResult.Success(dt);
            return Task.CompletedTask;

        }
    }
}
