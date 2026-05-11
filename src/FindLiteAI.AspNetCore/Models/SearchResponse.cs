namespace FindLiteAI.AspNetCore.Models;

/// <summary>
/// Represents a semantic search response item.
/// </summary>
public sealed class SearchResponse
{
    /// <summary>
    /// Gets or sets the document identifier.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the searchable document text.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets optional metadata values.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the calculated relevance score.
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Gets or sets the ranking position.
    /// </summary>
    public int Rank { get; set; }
}
