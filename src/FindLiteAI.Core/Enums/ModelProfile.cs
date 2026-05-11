namespace FindLiteAI.Core.Enums;

/// <summary>
/// Defines the AI embedding model profile used by FindLiteAI.
/// </summary>
public enum ModelProfile
{
    /// <summary>
    /// Optimized for speed and low memory usage.
    /// </summary>
    Fast = 0,

    /// <summary>
    /// Provides balanced performance and semantic quality.
    /// </summary>
    Balanced = 1,

    /// <summary>
    /// Optimized for higher semantic accuracy.
    /// </summary>
    Quality = 2
}
