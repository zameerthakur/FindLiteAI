using FindLiteAI.Core.Enums;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Tests.Core.Fakes;
using FluentAssertions;

namespace FindLiteAI.Tests.Core;

/// <summary>
/// Contains unit tests for the FindLiteAI engine.
/// </summary>
public sealed class FindLiteAIEngineTests
{
    /// <summary>
    /// Verifies that a document can be indexed and found using hybrid search.
    /// </summary>
    [Fact]
    public async Task SearchAsync_WhenDocumentMatchesQuery_ShouldReturnResult()
    {
        FindLiteAIEngine engine = CreateEngine();

        await engine.AddAsync(
            "logs",
            new SemanticDocument
            {
                Id = "log-1",
                Text = "SFTP authentication failed for remote user."
            });

        IReadOnlyList<SearchResult> results =
            await engine.SearchAsync(
                "logs",
                "login issue",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MinimumScore = 0.1,
                    MaxResults = 10
                });

        results.Should().HaveCount(1);

        results[0].Document.Id.Should().Be("log-1");
    }

    /// <summary>
    /// Verifies that unrelated documents are filtered when the minimum score is high enough.
    /// </summary>
    [Fact]
    public async Task SearchAsync_WhenDocumentDoesNotMatchQuery_ShouldReturnNoResults()
    {
        FindLiteAIEngine engine = CreateEngine();

        await engine.AddAsync(
            "logs",
            new SemanticDocument
            {
                Id = "log-1",
                Text = "SMTP relay accepted the outgoing email."
            });

        IReadOnlyList<SearchResult> results =
            await engine.SearchAsync(
                "logs",
                "database timeout",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MinimumScore = 0.8,
                    MaxResults = 10
                });

        results.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that similar documents can be found using an existing indexed document.
    /// </summary>
    [Fact]
    public async Task FindSimilarAsync_WhenSimilarDocumentExists_ShouldReturnSimilarDocument()
    {
        FindLiteAIEngine engine = CreateEngine();

        await engine.AddAsync(
            "tickets",
            new SemanticDocument
            {
                Id = "ticket-1",
                Text = "User cannot login to SFTP server."
            });

        await engine.AddAsync(
            "tickets",
            new SemanticDocument
            {
                Id = "ticket-2",
                Text = "SSH authentication failed because credentials were rejected."
            });

        IReadOnlyList<SearchResult> results =
            await engine.FindSimilarAsync(
                "tickets",
                "ticket-1",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MinimumScore = 0.1,
                    MaxResults = 10
                });

        results.Should().HaveCount(1);

        results[0].Document.Id.Should().Be("ticket-2");
    }

    /// <summary>
    /// Verifies that deleted documents are no longer returned in search results.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WhenDocumentIsDeleted_ShouldRemoveDocumentFromSearchResults()
    {
        FindLiteAIEngine engine = CreateEngine();

        await engine.AddAsync(
            "logs",
            new SemanticDocument
            {
                Id = "log-1",
                Text = "SQL database timeout occurred."
            });

        await engine.DeleteAsync(
            "logs",
            "log-1");

        IReadOnlyList<SearchResult> results =
            await engine.SearchAsync(
                "logs",
                "database timeout",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MinimumScore = 0.1,
                    MaxResults = 10
                });

        results.Should().BeEmpty();
    }

    /// <summary>
    /// Creates a FindLiteAI engine configured with test doubles.
    /// </summary>
    /// <returns>
    /// A test engine instance.
    /// </returns>
    private static FindLiteAIEngine CreateEngine()
    {
        return new FindLiteAIEngine(
            new FakeEmbeddingProvider(),
            new InMemorySemanticStore());
    }
}
