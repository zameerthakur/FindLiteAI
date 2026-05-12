using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Embeddings.Onnx.Internal;

/// <summary>
/// Resolves runtime file paths from an extracted model package.
/// </summary>
public static class ModelPackageResolver
{
    /// <summary>
    /// Resolves ONNX provider options from an extracted model package directory.
    /// </summary>
    /// <param name="modelDirectory">
    /// The extracted model package directory.
    /// </param>
    /// <param name="maxTokenLength">
    /// The maximum token length used during tokenization.
    /// </param>
    /// <param name="warmupOnLoad">
    /// A value indicating whether the provider should warm up during initialization.
    /// </param>
    /// <returns>
    /// ONNX embedding provider options resolved from the model package.
    /// </returns>
    /// <exception cref="SearchException">
    /// Thrown when the model package is invalid.
    /// </exception>
    public static OnnxEmbeddingProviderOptions Resolve(
        string modelDirectory,
        int maxTokenLength = 256,
        bool warmupOnLoad = true)
    {
        ModelPackageInfo modelInfo =
            ModelPackageReader.Read(modelDirectory);

        string modelPath =
            Path.Combine(
                modelDirectory,
                modelInfo.OnnxPath);

        string vocabularyPath =
            Path.Combine(
                modelDirectory,
                modelInfo.TokenizerPath);

        if (!File.Exists(modelPath))
        {
            throw new SearchException(
                $"ONNX model file from package metadata was not found: '{modelPath}'.");
        }

        if (!File.Exists(vocabularyPath))
        {
            throw new SearchException(
                $"Tokenizer vocabulary file from package metadata was not found: '{vocabularyPath}'.");
        }

        return new OnnxEmbeddingProviderOptions
        {
            ModelPath = modelPath,
            VocabularyPath = vocabularyPath,
            MaxTokenLength = maxTokenLength,
            WarmupOnLoad = warmupOnLoad
        };
    }
}
