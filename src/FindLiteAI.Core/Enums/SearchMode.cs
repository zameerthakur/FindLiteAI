namespace FindLiteAI.Core.Enums;

/// <summary>
/// Defines the search strategy used by FindLiteAI.
/// </summary>
public enum SearchMode
{
    /// <summary>
    /// Uses semantic similarity based on AI-generated embeddings.
    /// </summary>
    Semantic = 0,

    /// <summary>
    /// Uses keyword-based matching.
    /// </summary>
    Keyword = 1,

    /// <summary>
    /// Combines semantic similarity and keyword matching.
    /// </summary>
    Hybrid = 2
}
