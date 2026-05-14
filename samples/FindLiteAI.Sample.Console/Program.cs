using FindLiteAI.Embeddings.Onnx;
using Microsoft.Extensions.Logging.Abstractions;

Console.WriteLine("FindLiteAI Model Package Validation Sample");
Console.WriteLine("------------------------------------------");

string cacheDirectory =
    Path.Combine(
        Path.GetTempPath(),
        "FindLiteAI",
        "Models");

Console.WriteLine($"Cache directory: {cacheDirectory}");
Console.WriteLine();

foreach (FindLiteAIModelDefinition model in FindLiteAIModels.GetAll())
{
    Console.WriteLine($"Testing model: {model.DisplayName}");
    Console.WriteLine($"Profile: {model.Profile}");

    string modelDirectory =
        await ModelInstallService.InstallAsync(
            model,
            cacheDirectory,
            overwrite: false);

    Console.WriteLine($"Model directory: {modelDirectory}");

    using OnnxEmbeddingProvider provider =
        OnnxEmbeddingProviderFactory.FromInstalledModel(
            model,
            NullLogger<OnnxEmbeddingProvider>.Instance,
            cacheDirectory,
            maxTokenLength: 256,
            warmupOnLoad: true);

    IReadOnlyList<float> embedding =
        await provider.GenerateEmbeddingAsync(
            "SFTP authentication failed for remote user.");

    Console.WriteLine($"Embedding dimensions: {embedding.Count}");
    Console.WriteLine($"Expected dimensions: {model.Dimensions}");
    Console.WriteLine($"First 5 values: {string.Join(", ", embedding.Take(5))}");

    if (embedding.Count != model.Dimensions)
    {
        throw new InvalidOperationException(
            $"Model '{model.Id}' returned {embedding.Count} dimensions, expected {model.Dimensions}.");
    }

    Console.WriteLine("Status: OK");
    Console.WriteLine();
}

Console.WriteLine("All FindLiteAI models validated successfully.");
