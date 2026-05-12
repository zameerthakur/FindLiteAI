using FindLiteAI.Core.Exceptions;

namespace FindLiteAI.Embeddings.Onnx.Internal;

/// <summary>
/// Provides validation for ONNX embedding provider options.
/// </summary>
internal static class OnnxOptionsValidator
{
    /// <summary>
    /// Validates ONNX embedding provider options.
    /// </summary>
    /// <param name="options">
    /// The options to validate.
    /// </param>
    /// <exception cref="SearchException">
    /// Thrown when required model files are missing or invalid.
    /// </exception>
    public static void Validate(
        OnnxEmbeddingProviderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ModelPath))
        {
            throw new SearchException(
                "ONNX model path must be configured.");
        }

        if (!File.Exists(options.ModelPath))
        {
            throw new SearchException(
                $"ONNX model file was not found at '{options.ModelPath}'.");
        }

        if (!Path.GetExtension(options.ModelPath)
                .Equals(".onnx", StringComparison.OrdinalIgnoreCase))
        {
            throw new SearchException(
                $"ONNX model path must point to a .onnx file. Provided path: '{options.ModelPath}'.");
        }

        if (string.IsNullOrWhiteSpace(options.VocabularyPath))
        {
            throw new SearchException(
                "Tokenizer vocabulary path must be configured.");
        }

        if (!File.Exists(options.VocabularyPath))
        {
            throw new SearchException(
                $"Tokenizer vocabulary file was not found at '{options.VocabularyPath}'.");
        }

        if (!string.Equals(
                Path.GetFileName(options.VocabularyPath),
                "vocab.txt",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new SearchException(
                $"Tokenizer vocabulary path must point to vocab.txt. Provided path: '{options.VocabularyPath}'.");
        }

        if (options.MaxTokenLength <= 0)
        {
            throw new SearchException(
                "MaxTokenLength must be greater than zero.");
        }
    }
}
