namespace FindLiteAI.Embeddings.Onnx;

/// <summary>
/// Provides built-in FindLiteAI model definitions.
/// </summary>
public static class FindLiteAIModels
{
    /// <summary>
    /// Gets the fast lightweight MiniLM model definition.
    /// </summary>
    public static FindLiteAIModelDefinition MiniLm { get; } = new()
    {
        Id = "all-MiniLM-L6-v2",
        DisplayName = "all-MiniLM-L6-v2",
        Profile = "Fast",
        Dimensions = 384,
        MinimumRamGb = 4,
        RecommendedRamGb = 8,
        Source = "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2",
        PackageSource = "https://github.com/zameerthakur/FindLiteAI/releases/download/v1/findliteai-minilm-v1.zip",
        License = "Apache-2.0"
    };

    /// <summary>
    /// Gets the balanced MPNet model definition.
    /// </summary>
    public static FindLiteAIModelDefinition MpNet { get; } = new()
    {
        Id = "all-mpnet-base-v2",
        DisplayName = "all-mpnet-base-v2",
        Profile = "Balanced",
        Dimensions = 768,
        MinimumRamGb = 8,
        RecommendedRamGb = 16,
        Source = "https://huggingface.co/sentence-transformers/all-mpnet-base-v2",
        PackageSource = "https://github.com/zameerthakur/FindLiteAI/releases/download/v1/findliteai-mpnet-v1.zip",
        License = "Apache-2.0"
    };

    /// <summary>
    /// Gets the advanced Arctic Embed XS model definition.
    /// </summary>
    public static FindLiteAIModelDefinition ArcticXs { get; } = new()
    {
        Id = "arctic-embed-xs",
        DisplayName = "Snowflake Arctic Embed XS",
        Profile = "Advanced",
        Dimensions = 384,
        MinimumRamGb = 8,
        RecommendedRamGb = 16,
        Source = "https://huggingface.co/Snowflake/snowflake-arctic-embed-xs",
        PackageSource = "https://github.com/zameerthakur/FindLiteAI/releases/download/v1/findliteai-arctic-xs-v1.zip",
        License = "Apache-2.0"
    };

    /// <summary>
    /// Gets all built-in FindLiteAI model definitions.
    /// </summary>
    /// <returns>
    /// A read-only list of built-in model definitions.
    /// </returns>
    public static IReadOnlyList<FindLiteAIModelDefinition> GetAll()
    {
        return
        [
            MiniLm,
            MpNet,
            ArcticXs
        ];
    }
}
