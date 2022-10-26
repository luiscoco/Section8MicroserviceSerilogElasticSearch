using Common.Logging;
using Serilog;
using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Shopping.Aggregator", Version = "v1" });
});

builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
                 c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
                 c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
                .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
                 c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
                 .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Host.UseSerilog(SeriLogger.Configure);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping.Aggregator v1"));
}
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();