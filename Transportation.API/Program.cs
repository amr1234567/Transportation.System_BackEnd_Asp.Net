using Hangfire;
using Serilog;
using System.Text.Json.Serialization;
using Transportation.API.Filters;
using Transportation.API.Helpers;
using Transportation.API.Helpers.DI;

var builder = WebApplication.CreateBuilder(args);

//DI
builder.Services.AddApplicationDi(builder.Configuration);

builder.Services.AddModelHelpersServices(builder.Configuration);

builder.Services.AddHangfire(options =>
{
    options.UseSqlServerStorage(builder.Configuration.GetConnectionString("AppConnString"));
});
builder.Services.AddHangfireServer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidationFilterAttribute));
}).ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/logging.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigSwagger();
builder.Services.AddAuthConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/dashboard");
app.MapControllers();

app.Run();
