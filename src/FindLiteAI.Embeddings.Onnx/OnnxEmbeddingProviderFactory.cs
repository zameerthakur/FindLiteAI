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
}
