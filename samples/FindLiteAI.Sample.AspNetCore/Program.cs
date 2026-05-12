using FindLiteAI.AspNetCore;
using FindLiteAI.Extensions.DependencyInjection;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddFindLiteAI(options =>
{
    options.DatabasePath = "findliteai-sample.db";

    options.ModelPath =
        @"D:\AIModels\FindLiteAI\all-MiniLM-L6-v2\model.onnx";
});

WebApplication app =
    builder.Build();

app.MapGet(
    "/",
    () => "FindLiteAI ASP.NET Core sample is running.");

app.MapFindLiteAI("/api/findliteai");

app.Run();
