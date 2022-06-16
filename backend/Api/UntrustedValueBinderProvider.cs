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

        return context.Metadata.ModelType == typeof(UntrustedValue<string>)
            ? new UntrustedStringBinder()
            : null;
    }
}
