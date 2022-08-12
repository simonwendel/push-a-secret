using Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSecretsStorage();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
