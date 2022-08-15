// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Storage;

public record StorageConfiguration
{
    public string? ConnectionString { get; init; }
    public string? DatabaseName { get; init; }
    public string? CollectionName { get; init; }

    internal bool IsValid()
        => ConnectionString != null && DatabaseName != null && CollectionName != null;
}
