﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        var model = new UntrustedValue<string>(value);
        context.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}
