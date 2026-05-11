using FindLiteAI.Core.Models;
using LiteDB;

namespace FindLiteAI.Storage.LiteDb.Internal;

/// <summary>
/// Represents a LiteDB persistence entity for semantic documents.
/// </summary>
internal sealed class LiteDbDocumentEntity
{
    /// <summary>
    /// Gets or sets the LiteDB document identifier.
    /// </summary>
    [BsonId]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the searchable document content.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets optional document metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the semantic embedding vector.
    /// </summary>
    public required float[] Embedding { get; set; }

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; }

    /// <summary>
    /// Gets or sets the UTC update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedUtc { get; set; }

    /// <summary>
    /// Creates a LiteDB entity from a semantic document and embedding.
    /// </summary>
    /// <param name="document">
    /// The semantic document.
    /// </param>
    /// <param name="embedding">
    /// The semantic embedding vector.
    /// </param>
    /// <returns>
    /// A LiteDB persistence entity.
    /// </returns>
    public static LiteDbDocumentEntity Create(
        SemanticDocument document,
        IReadOnlyList<float> embedding)
    {
        return new LiteDbDocumentEntity
        {
            Id = document.Id,
            Text = document.Text,
            Metadata = document.Metadata?.ToDictionary(),
            Embedding = embedding.ToArray(),
            CreatedUtc = document.CreatedUtc,
            UpdatedUtc = document.UpdatedUtc
        };
    }

    /// <summary>
    /// Converts the LiteDB entity into a semantic document.
    /// </summary>
    /// <returns>
    /// A semantic document instance.
    /// </returns>
    public SemanticDocument ToDocument()
    {
        return new SemanticDocument
        {
            Id = Id,
            Text = Text,
            Metadata = Metadata,
            CreatedUtc = CreatedUtc,
            UpdatedUtc = UpdatedUtc
        };
    }
}
