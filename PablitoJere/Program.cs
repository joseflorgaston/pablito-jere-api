using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using PablitoJere;
using PablitoJere.Services;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var initDatabase = new InitDatabaseService();

var logger = app.Services.GetService(typeof(ILogger<Startup>)) as ILogger<Startup>;

startup.Configure(app, app.Environment, logger);

await initDatabase.InitDatabase(app.Services);

app.Run();