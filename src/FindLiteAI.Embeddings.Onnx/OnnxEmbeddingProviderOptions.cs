namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Defines configuration options for the ONNX embedding provider.
/// </summary>
public sealed class OnnxEmbeddingProviderOptions
{
    /// <summary>
    /// Gets or sets the path to the ONNX model file.
    /// </summary>
    public required string ModelPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the tokenizer JSON file.
    /// </summary>
    public required string TokenizerPath { get; set; }

    /// <summary>
    /// Gets or sets the maximum token length used during tokenization.
    /// </summary>
    public int MaxTokenLength { get; set; } = 256;

    /// <summary>
    /// Gets or sets a value indicating whether the ONNX session should be warmed up during initialization.
    /// </summary>
    public bool WarmupOnLoad { get; set; } = true;
}
