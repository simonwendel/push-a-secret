﻿using Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Validation;

namespace Api;

public class UntrustedValueBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext? context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(UntrustedValue<Identifier>))
        {
            return new UntrustedIdentifierBinder();
        }

        if (context.Metadata.ModelType == typeof(UntrustedValue<Secret>))
        {
            return new UntrustedSecretBinder();
        }

        return null;
    }
}
