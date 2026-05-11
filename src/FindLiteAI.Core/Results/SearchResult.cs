using FindLiteAI.Core.Models;

namespace FindLiteAI.Core.Results;

/// <summary>
/// Represents a ranked search result.
/// </summary>
public sealed class SearchResult
{
    /// <summary>
    /// Gets or sets the matched document.
    /// </summary>
    public required SemanticDocument Document { get; set; }

    /// <summary>
    /// Gets or sets the calculated relevance score.
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Gets or sets the zero-based ranking position.
    /// </summary>
    public int Rank { get; set; }
}
