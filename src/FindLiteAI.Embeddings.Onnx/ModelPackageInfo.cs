namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Represents metadata for a FindLiteAI model package.
/// </summary>
public sealed class ModelPackageInfo
{
    /// <summary>
    /// Gets or sets the model identifier.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the model profile name.
    /// </summary>
    public required string Profile { get; set; }

    /// <summary>
    /// Gets or sets the embedding dimension count.
    /// </summary>
    public int Dimensions { get; set; }

    /// <summary>
    /// Gets or sets the runtime type.
    /// </summary>
    public required string Runtime { get; set; }

    /// <summary>
    /// Gets or sets the relative ONNX model path.
    /// </summary>
    public required string OnnxPath { get; set; }

    /// <summary>
    /// Gets or sets the relative tokenizer vocabulary path.
    /// </summary>
    public string TokenizerPath { get; set; } = "vocab.txt";

    /// <summary>
    /// Gets or sets the pooling strategy.
    /// </summary>
    public string Pooling { get; set; } = "mean";

    /// <summary>
    /// Gets or sets the original model source URL.
    /// </summary>
    public required string Source { get; set; }

    /// <summary>
    /// Gets or sets the FindLiteAI optimized model package source URL.
    /// </summary>
    public required string PackageSource { get; set; }

    /// <summary>
    /// Gets or sets the package optimization target.
    /// </summary>
    public string OptimizedFor { get; set; } = "FindLiteAI";

    /// <summary>
    /// Gets or sets the model license identifier.
    /// </summary>
    public required string License { get; set; }
}
