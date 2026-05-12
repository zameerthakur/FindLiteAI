using FindLiteAI.Core.Models;
using FindLiteAI.Storage.LiteDb;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains integration tests for the LiteDB semantic store.
/// </summary>
public sealed class LiteDbSemanticStoreTests
{
    /// <summary>
    /// Verifies that a document and embedding can be stored and retrieved.
    /// </summary>
    [Fact]
    public async Task UpsertAsync_WhenDocumentIsStored_ShouldRetrieveDocument()
    {
        string databasePath = CreateDatabasePath();

        using LiteDbSemanticStore store = CreateStore(databasePath);

        SemanticDocument document = new()
        {
            Id = "doc-1",
            Text = "SFTP authentication failed.",
            Metadata = new Dictionary<string, string>
            {
                ["module"] = "FTP"
            }
        };

        float[] embedding =
        [
            1f,
            0f,
            0f
        ];

        await store.UpsertAsync(
            "logs",
            document,
            embedding);

        (SemanticDocument Document, IReadOnlyList<float> Embedding)? stored =
            await store.GetByIdAsync(
                "logs",
                "doc-1");

        stored.Should().NotBeNull();

        stored!.Value.Document.Id.Should().Be("doc-1");

        stored.Value.Document.Text.Should().Be("SFTP authentication failed.");

        stored.Value.Embedding.Should().Equal(embedding);
    }

    /// <summary>
    /// Verifies that multiple documents can be stored and retrieved from a collection.
    /// </summary>
    [Fact]
    public async Task UpsertRangeAsync_WhenDocumentsAreStored_ShouldRetrieveAllDocuments()
    {
        string databasePath = CreateDatabasePath();

        using LiteDbSemanticStore store = CreateStore(databasePath);

        SemanticDocument[] documents =
        [
            new()
            {
                Id = "doc-1",
                Text = "SFTP authentication failed."
            },
            new()
            {
                Id = "doc-2",
                Text = "SQL database timeout occurred."
            }
        ];

        IReadOnlyList<float>[] embeddings =
        [
            [1f, 0f, 0f],
            [0f, 1f, 0f]
        ];

        await store.UpsertRangeAsync(
            "logs",
            documents,
            embeddings);

        IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> stored =
            await store.GetAllAsync("logs");

        stored.Should().HaveCount(2);

        stored
            .Select(item => item.Document.Id)
            .Should()
            .BeEquivalentTo(
                "doc-1",
                "doc-2");
    }

    /// <summary>
    /// Verifies that deleted documents are removed from the LiteDB store.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WhenDocumentExists_ShouldRemoveDocument()
    {
        string databasePath = CreateDatabasePath();

        using LiteDbSemanticStore store = CreateStore(databasePath);

        await store.UpsertAsync(
            "logs",
            new SemanticDocument
            {
                Id = "doc-1",
                Text = "SQL database timeout occurred."
            },
            [0f, 1f, 0f]);

        await store.DeleteAsync(
            "logs",
            "doc-1");

        (SemanticDocument Document, IReadOnlyList<float> Embedding)? stored =
            await store.GetByIdAsync(
                "logs",
                "doc-1");

        stored.Should().BeNull();
    }

    /// <summary>
    /// Creates a temporary database path for an integration test.
    /// </summary>
    /// <returns>
    /// A temporary LiteDB database path.
    /// </returns>
    private static string CreateDatabasePath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            $"findliteai-test-{Guid.NewGuid():N}.db");
    }

    /// <summary>
    /// Creates a LiteDB semantic store for testing.
    /// </summary>
    /// <param name="databasePath">
    /// The database path.
    /// </param>
    /// <returns>
    /// A configured LiteDB semantic store.
    /// </returns>
    private static LiteDbSemanticStore CreateStore(
        string databasePath)
    {
        return new LiteDbSemanticStore(
            new LiteDbOptions
            {
                DatabasePath = databasePath
            },
            NullLogger<LiteDbSemanticStore>.Instance);
    }
}
