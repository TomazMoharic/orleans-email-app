using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Orleans.Configuration;
using OrleansEmailApp;
using OrleansEmailApp.Services;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
// Add services to the container.
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.Local.json", false, true)
        .AddEnvironmentVariables();
}
else
{
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.json", false, true)
        .AddEnvironmentVariables();
}

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Orleans Email App",
        Version = "v1"
    });
});

builder.Services.AddTransient<IBreachedEmailService, BreachedEmailService>();


// add Orleans
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();   
    siloBuilder.AddAzureBlobGrainStorage(
        name: "emails",
        configureOptions: options =>
        {
            options.ConfigureBlobServiceClient(
                builder.Configuration["AzureBlobStorageAccessKey"]);
        });
});

var app = builder.Build();

app.UseSwagger(); 
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapPost("/{email}",
    async (IBreachedEmailService breachedEmailService, HttpRequest request, string email) =>
    {
        string? response = await breachedEmailService.AddEmailToBreachedList(email);

        if (response is null)
        {
            return Results.Conflict("This email already existing in the list of breached emails.");
        }
        
        var resultBuilder = new UriBuilder($"{ request.Scheme }://{ request.Host.Value}")
        {
            Path = $"/{ email }"
        };
        
        return Results.Created(resultBuilder.Uri, "Added breached email.");
    });

app.MapGet("/{email}",
    async (IBreachedEmailService breachedEmailService, string email) =>
    {
        string? response = await breachedEmailService.CheckIfEmailIsBreached(email);

        if (response is null)
        {
            return Results.NotFound("This email does not exist in the list of breached emails. This email is clean!");
        }

        return Results.Ok("This email exists in the list of breached emails. It has been breached!");
    });


app.MapControllers();

app.Run();