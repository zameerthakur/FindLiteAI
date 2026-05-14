using FindLiteAI.Embeddings.Onnx;
using Microsoft.Extensions.Logging.Abstractions;

Console.WriteLine("FindLiteAI Model Package Sample");
Console.WriteLine("--------------------------------");

string cacheDirectory =
    Path.Combine(
        Path.GetTempPath(),
        "FindLiteAI",
        "Models");

Console.WriteLine($"Cache directory: {cacheDirectory}");

string modelDirectory =
    await ModelInstallService.InstallAsync(
        FindLiteAIModels.MiniLm,
        cacheDirectory,
        overwrite: false);

Console.WriteLine($"Model directory: {modelDirectory}");

using OnnxEmbeddingProvider provider =
    OnnxEmbeddingProviderFactory.FromInstalledModel(
        FindLiteAIModels.MiniLm,
        NullLogger<OnnxEmbeddingProvider>.Instance,
        cacheDirectory,
        maxTokenLength: 256,
        warmupOnLoad: true);

IReadOnlyList<float> embedding =
    await provider.GenerateEmbeddingAsync(
        "SFTP authentication failed for remote user.");

Console.WriteLine($"Embedding dimensions: {embedding.Count}");
Console.WriteLine($"First 5 values: {string.Join(", ", embedding.Take(5))}");
