namespace FindLiteAI.Core.Constants;

/// <summary>
/// Provides constants related to supported AI embedding models.
/// </summary>
public static class ModelConstants
{
    /// <summary>
    /// Gets the identifier for the MiniLM model profile.
    /// </summary>
    public const string MiniLm = "all-MiniLM-L6-v2";

    /// <summary>
    /// Gets the identifier for the Arctic XS model profile.
    /// </summary>
    public const string ArcticXs = "snowflake-arctic-embed-xs";

    /// <summary>
    /// Gets the identifier for the MPNet model profile.
    /// </summary>
    public const string MpNet = "all-mpnet-base-v2";

    /// <summary>
    /// Gets the embedding dimensions for the MiniLM model.
    /// </summary>
    public const int MiniLmDimensions = 384;

    /// <summary>
    /// Gets the embedding dimensions for the Arctic XS model.
    /// </summary>
    public const int ArcticXsDimensions = 384;

    /// <summary>
    /// Gets the embedding dimensions for the MPNet model.
    /// </summary>
    public const int MpNetDimensions = 768;
}
