using FeedbackApi.Api.Extensions;
using FeedbackApi.Api.Middlewares;
using FeedbackApi.Infrastructure.Data;
using MongoDB.Driver;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDatabase(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddApiDocumentation()
                .AddJWTConfig(builder.Configuration)
                .AddApiCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    MongoConfig.Configure(scope.ServiceProvider.GetRequiredService<IMongoDatabase>());
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>()
    .UseMiddleware<ExceptionMiddleware>()
    .UseMiddleware<RequestLoggingMiddleware>()
    .UseCors("DefaultCors")
    .UseHttpsRedirection()
    .UseHttpMetrics()
    .UseAuthentication()
    .UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();
app.MapMetrics();

app.Run();
