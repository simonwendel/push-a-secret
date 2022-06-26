using System.Text.Json;
using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Validation;

namespace Api;

public class UntrustedSecretBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext? context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var modelName = context.FieldName;
        var value = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                var secret = JsonSerializer.Deserialize<Secret>(value)
                             ?? throw new InvalidOperationException();
                var model = new UntrustedValue<Secret>(secret);
                context.ModelState.SetModelValue(modelName, new ValueProviderResult(value));
                context.ModelState.MarkFieldValid(modelName);
                context.Result = ModelBindingResult.Success(model);
            }
            catch (Exception)
            {
                // ignored because we simply won't bother to bind if something happens
            }
        }
    }
}
