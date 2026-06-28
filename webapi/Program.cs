using System;
using System.Collections.Generic;
using System.IO;
using app.Persistence;
using app.Services;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:3000");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("Oracle")
    ?? Environment.GetEnvironmentVariable("EPS_TRACKER_ORACLE_CONNECTION_STRING")
    ?? Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Oracle connection string nije podesen. Dodaj ConnectionStrings:Oracle ili EPS_TRACKER_ORACLE_CONNECTION_STRING.");
}

builder.Services.AddSingleton(_ =>
    new NHibernateSessionFactoryProvider(new OraclePersistenceOptions(connectionString)));
builder.Services.AddScoped<PotrosacService>();
builder.Services.AddScoped<BrojiloService>();
builder.Services.AddScoped<MerenjeService>();
builder.Services.AddScoped<RacunService>();
builder.Services.AddScoped<KvarService>();
builder.Services.AddScoped<StanjeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new { name = "EPS Tracker API" }));
app.MapControllers();

app.Run();

static void LoadDotEnv()
{
    foreach (var dotenvPath in FindDotEnvPaths())
    {
        if (!File.Exists(dotenvPath))
        {
            continue;
        }

        foreach (var rawLine in File.ReadAllLines(dotenvPath))
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line.Substring(0, separatorIndex).Trim();
            var value = line.Substring(separatorIndex + 1).Trim();

            if (value.Length >= 2
                && ((value[0] == '"' && value[value.Length - 1] == '"')
                    || (value[0] == '\'' && value[value.Length - 1] == '\'')))
            {
                value = value.Substring(1, value.Length - 2);
            }

            if (string.IsNullOrWhiteSpace(key)
                || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
            {
                continue;
            }

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

static IEnumerable<string> FindDotEnvPaths()
{
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

    while (directory != null)
    {
        yield return Path.Combine(directory.FullName, ".env");
        yield return Path.Combine(directory.FullName, "app", ".env");
        directory = directory.Parent;
    }
}
