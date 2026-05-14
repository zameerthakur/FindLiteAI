using FindLiteAI.AspNetCore;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

string cacheDirectory =
    Path.Combine(
        Path.GetTempPath(),
        "FindLiteAI",
        "Models");

await ModelInstallService.InstallAsync(
    FindLiteAIModels.MiniLm,
    cacheDirectory,
    overwrite: false);

string modelDirectory =
    Path.Combine(
        cacheDirectory,
        FindLiteAIModels.MiniLm.Id);

builder.Services.AddFindLiteAI(options =>
{
    options.DatabasePath = "findliteai-sample.db";
    options.ModelCacheDirectory = modelDirectory;
});

WebApplication app =
    builder.Build();

app.MapGet(
    "/",
    () => "FindLiteAI ASP.NET Core sample is running.");

app.MapFindLiteAI("/api/findliteai");

app.Run();
