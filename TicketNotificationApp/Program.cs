using System.Text.Json.Serialization;
using TicketNotificationApp.Data.Repositories;
using TicketNotificationApp.Gateway;
using TicketNotificationApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ticket Notification API",
        Version = "v1",
        Description = "Sends Email/SMS/Push notifications for tickets with a defined lifecycle and scheduled retries."
    });
});

builder.Services.AddSingleton<ITicketRepository, TicketRepository>();

builder.Services.AddSingleton<INotificationGateway, NotificationGateway>();

builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket Notification API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();