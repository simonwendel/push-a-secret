using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Validation;

namespace Api;

public class UntrustedIdentifierBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext? context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var name = context.ModelName;
        var result = context.ValueProvider.GetValue(name);
        if (result == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        context.ModelState.SetModelValue(name, result);
        var value = result.FirstValue;
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var identifier = new Identifier(value);
        var model = new UntrustedValue<Identifier>(identifier);
        context.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}
