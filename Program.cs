using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Orleans.Configuration;
using OrleansEmailApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    // .AddJsonFile("settings.json", false, true)
    .AddJsonFile($"appsettings.Local.json", false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false, true)
    .AddEnvironmentVariables();


builder.Services.AddControllers();
builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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





// if (builder.Environment.IsDevelopment())
// {
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
    //siloBuilder.AddMemoryGrainStorage("emails");
    // siloBuilder.UseDashboard();
});
// }
// else
// {
//     builder.Host.UseOrleans(siloBuilder =>
//     {
//         var connectionString = "your_storage_connection_string";
//
         // siloBuilder.AddAzureBlobGrainStorage(
         //     name: "profileStore",
         //     configureOptions: options =>
         //     {
         //         options.ConfigureBlobServiceClient(
         //             builder.Configuration["AzureBlobStorageConnectionString"]);
         //     });
//         
//         siloBuilder.Configure<ClusterOptions>(options =>
//         {
//             options.ClusterId = "url-shortener";
//             options.ServiceId = "urls";
//         });
//     });
// }

// builder.Host.UseOrleans(siloBuilder =>
// {
//     siloBuilder.UseLocalhostClustering();
//     siloBuilder.AddMemoryGrainStorage("emails");
//
// });

var app = builder.Build();

app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapPost("/{email}",
    async (IGrainFactory grains, HttpRequest request, string email) =>
    {
        string emailDomain = Helpers.ExtractEmailDomain(email);
        
        IDomainBreachedEmailsGrain? emailDomainGrain = grains.GetGrain<IDomainBreachedEmailsGrain>(emailDomain);

        DomainBreachedEmails? domainObject = await emailDomainGrain.GetItem();

        string? addedEmail = domainObject?.DomainEmails.FirstOrDefault(de => de == email);
        
        if (domainObject is not null)
        {
            if (addedEmail is not null)
            {
                return Results.Conflict("This email already exists in the list of breached emails.");
            }
            
            domainObject.DomainEmails.Add(email);
        }
        else
        {
            domainObject = new DomainBreachedEmails
            {
                Domain = emailDomain,
                DomainEmails = new List<string> { email },
            };
        }

        
        await emailDomainGrain.SetItem(domainObject);
        
        var resultBuilder = new UriBuilder($"{ request.Scheme }://{ request.Host.Value}")
        {
            Path = $"/{ email }"
        };
        
        return Results.Created(resultBuilder.Uri, "Added breached email.");
    });

app.MapGet("/{email}",
    async (IGrainFactory grains, string email) =>
    {
        string emailDomain = Helpers.ExtractEmailDomain(email);
        
        IDomainBreachedEmailsGrain? emailDomainGrain = grains.GetGrain<IDomainBreachedEmailsGrain>(emailDomain);

        DomainBreachedEmails? domainObject = await emailDomainGrain.GetItem();
        
        var existingEmail = domainObject?.DomainEmails.FirstOrDefault(de => de == email);

        if (domainObject is null || existingEmail is null)
        {
            return Results.NotFound("This email does not exist in the list of breached emails. This email is clean!");
        }

        return Results.Ok("This email exists in the list of breached emails. It has been breached!");
    });


app.MapControllers();

app.Run();