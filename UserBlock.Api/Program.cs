using Corp.Billing.Shared;
using UserBlock.Api;
using UserBlock.Application;
using UserBlock.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddConfigureAuthentication(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddExternalServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<MemoryCacheHandlerMiddleware>();
app.Run();