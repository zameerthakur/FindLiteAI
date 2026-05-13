using FindLiteAI.Embeddings.Onnx.Internal;
using Microsoft.Extensions.Logging;

namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides factory methods for creating ONNX embedding providers.
/// </summary>
public static class OnnxEmbeddingProviderFactory
{
    /// <summary>
    /// Creates an ONNX embedding provider from an extracted FindLiteAI model package directory.
    /// </summary>
    /// <param name="modelDirectory">
    /// The extracted model package directory containing MODEL_INFO.json.
    /// </param>
    /// <param name="logger">
    /// The logger instance.
    /// </param>
    /// <param name="maxTokenLength">
    /// The maximum token length used during tokenization.
    /// </param>
    /// <param name="warmupOnLoad">
    /// A value indicating whether the provider should warm up during initialization.
    /// </param>
    /// <returns>
    /// A configured ONNX embedding provider.
    /// </returns>
    public static OnnxEmbeddingProvider FromModelPackage(
        string modelDirectory,
        ILogger<OnnxEmbeddingProvider> logger,
        int maxTokenLength = 256,
        bool warmupOnLoad = true)
    {
        OnnxEmbeddingProviderOptions options =
            ModelPackageResolver.Resolve(
                modelDirectory,
                maxTokenLength,
                warmupOnLoad);

        return new OnnxEmbeddingProvider(
            options,
            logger);
    }

    /// <summary>
    /// Creates an ONNX embedding provider from an installed built-in FindLiteAI model.
    /// </summary>
    /// <param name="model">
    /// The built-in model definition.
    /// </param>
    /// <param name="logger">
    /// The logger instance.
    /// </param>
    /// <param name="cacheDirectory">
    /// Optional model cache directory. If null, the default user cache directory is used.
    /// </param>
    /// <param name="maxTokenLength">
    /// The maximum token length used during tokenization.
    /// </param>
    /// <param name="warmupOnLoad">
    /// A value indicating whether the provider should warm up during initialization.
    /// </param>
    /// <returns>
    /// A configured ONNX embedding provider.
    /// </returns>
    public static OnnxEmbeddingProvider FromInstalledModel(
        FindLiteAIModelDefinition model,
        ILogger<OnnxEmbeddingProvider> logger,
        string? cacheDirectory = null,
        int maxTokenLength = 256,
        bool warmupOnLoad = true)
    {
        ArgumentNullException.ThrowIfNull(model);

        string modelDirectory =
            cacheDirectory is null
                ? ModelCachePaths.GetUserModelDirectory(model.Id)
                : Path.Combine(cacheDirectory, model.Id);

        return FromModelPackage(
            modelDirectory,
            logger,
            maxTokenLength,
            warmupOnLoad);
    }

}
