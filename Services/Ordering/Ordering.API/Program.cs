
using Ordering.Infrastructure.Persistence;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.API.Extensions;
using Microsoft.OpenApi.Models;
using MassTransit;
using EventBus.Messages.Common;
using Ordering.API.EventBusConsumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Set the Ordering.Application project configuration
builder.Services.AddApplicationServices();
// Set the Ordering.Infrastructure project configuration
builder.Services.AddInfrastructureServices(builder.Configuration);

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config => {

    config.AddConsumer<BasketCheckoutConsumer>();

    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed
        .SeedAsync(context, logger)
        .Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
