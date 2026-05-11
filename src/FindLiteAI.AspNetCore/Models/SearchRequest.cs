using FindLiteAI.Core.Enums;

namespace FindLiteAI.AspNetCore.Models;

/// <summary>
/// Represents a semantic search request.
/// </summary>
public sealed class SearchRequest
{
    /// <summary>
    /// Gets or sets the search query text.
    /// </summary>
    public required string Query { get; set; }

    /// <summary>
    /// Gets or sets the search mode.
    /// </summary>
    public SearchMode SearchMode { get; set; } = SearchMode.Hybrid;

    /// <summary>
    /// Gets or sets the maximum number of results to return.
    /// </summary>
    public int MaxResults { get; set; } = 10;

    /// <summary>
    /// Gets or sets the minimum relevance score.
    /// </summary>
    public double MinimumScore { get; set; } = 0.10;
}
