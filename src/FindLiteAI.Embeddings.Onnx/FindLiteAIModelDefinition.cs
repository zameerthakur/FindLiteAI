namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Represents a built-in FindLiteAI model definition.
/// </summary>
public sealed class FindLiteAIModelDefinition
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
    /// Gets or sets the recommended minimum RAM in GB.
    /// </summary>
    public int MinimumRamGb { get; set; }

    /// <summary>
    /// Gets or sets the recommended RAM in GB.
    /// </summary>
    public int RecommendedRamGb { get; set; }

    /// <summary>
    /// Gets or sets the original model source URL.
    /// </summary>
    public required string Source { get; set; }

    /// <summary>
    /// Gets or sets the FindLiteAI optimized package source URL.
    /// </summary>
    public required string PackageSource { get; set; }

    /// <summary>
    /// Gets or sets the model license identifier.
    /// </summary>
    public required string License { get; set; }
}
