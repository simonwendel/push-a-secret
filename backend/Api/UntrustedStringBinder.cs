using Microsoft.AspNetCore.Mvc.ModelBinding;
using Validation;

namespace Api;

public class UntrustedStringBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext? context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var modelName = context.ModelName;
        var valueResult = context.ValueProvider.GetValue(modelName);
        if (valueResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        context.ModelState.SetModelValue(modelName, valueResult);

        var value = valueResult.FirstValue;
        if (!string.IsNullOrEmpty(value))
        {
            var model = new UntrustedValue<string>(value);
            context.Result = ModelBindingResult.Success(model);
        }

        return Task.CompletedTask;
    }
}
