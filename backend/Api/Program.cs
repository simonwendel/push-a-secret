using Api;
using Conversion;
using Storage;
using Validation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(
    options => options.ModelBinderProviders.Insert(0, new UntrustedValueBinderProvider()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddValidationModule()
    .AddStorageModule()
    .AddConversionModule();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
