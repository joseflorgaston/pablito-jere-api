using Azure.Storage.Blobs;
using PablitoJere;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var logger = app.Services.GetService(typeof(ILogger<Startup>)) as ILogger<Startup>;

var blob = new BlobStorage();


startup.Configure(app, app.Environment, logger);

// blob.ConfigureBlobStorage();

app.Run();
