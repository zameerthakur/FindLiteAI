using FindLiteAI.Core.Enums;

namespace FindLiteAI.Core.Options;

/// <summary>
/// Defines configurable search behavior for FindLiteAI operations.
/// </summary>
public sealed class SearchOptions
{
    /// <summary>
    /// Gets or sets the maximum number of results to return.
    /// </summary>
    public int MaxResults { get; set; } = 10;

    /// <summary>
    /// Gets or sets the minimum relevance score required for a result.
    /// </summary>
    public double MinimumScore { get; set; } = 0.50;

    /// <summary>
    /// Gets or sets the search mode used during retrieval.
    /// </summary>
    public SearchMode SearchMode { get; set; } = SearchMode.Hybrid;
}
