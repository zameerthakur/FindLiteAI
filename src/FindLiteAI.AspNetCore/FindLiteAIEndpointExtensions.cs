using FindLiteAI.AspNetCore.Models;
using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FindLiteAI.AspNetCore;

/// <summary>
/// Provides ASP.NET Core endpoint mapping extensions for FindLiteAI.
/// </summary>
public static class FindLiteAIEndpointExtensions
{
    /// <summary>
    /// Maps FindLiteAI API endpoints.
    /// </summary>
    /// <param name="app">
    /// The endpoint route builder.
    /// </param>
    /// <param name="basePath">
    /// The API base path.
    /// </param>
    /// <returns>
    /// The endpoint route builder.
    /// </returns>
    public static IEndpointRouteBuilder MapFindLiteAI(
        this IEndpointRouteBuilder app,
        string basePath = "/api/findliteai")
    {
        ArgumentNullException.ThrowIfNull(app);

        RouteGroupBuilder group =
            app.MapGroup(basePath);

        group.MapPost(
            "/collections/{collection}/documents",
            async (
                string collection,
                AddDocumentRequest request,
                ISemanticSearchEngine engine,
                CancellationToken cancellationToken) =>
            {
                SemanticDocument document = new()
                {
                    Id = request.Id,
                    Text = request.Text,
                    Metadata = request.Metadata
                };

                await engine.AddAsync(
                    collection,
                    document,
                    cancellationToken);

                return Results.Ok();
            });

        group.MapPost(
            "/collections/{collection}/search",
            async (
                string collection,
                SearchRequest request,
                ISemanticSearchEngine engine,
                CancellationToken cancellationToken) =>
            {
                IReadOnlyList<SearchResult> results =
                    await engine.SearchAsync(
                        collection,
                        request.Query,
                        new SearchOptions
                        {
                            SearchMode = request.SearchMode,
                            MaxResults = request.MaxResults,
                            MinimumScore = request.MinimumScore
                        },
                        cancellationToken);

                IReadOnlyList<SearchResponse> response =
                    results
                        .Select(result =>
                            new SearchResponse
                            {
                                Id = result.Document.Id,
                                Text = result.Document.Text,
                                Metadata = result.Document.Metadata,
                                Score = result.Score,
                                Rank = result.Rank
                            })
                        .ToList();

                return Results.Ok(response);
            });

        group.MapGet(
            "/collections/{collection}/documents/{documentId}/similar",
            async (
                string collection,
                string documentId,
                ISemanticSearchEngine engine,
                CancellationToken cancellationToken) =>
            {
                IReadOnlyList<SearchResult> results =
                    await engine.FindSimilarAsync(
                        collection,
                        documentId,
                        cancellationToken: cancellationToken);

                IReadOnlyList<SearchResponse> response =
                    results
                        .Select(result =>
                            new SearchResponse
                            {
                                Id = result.Document.Id,
                                Text = result.Document.Text,
                                Metadata = result.Document.Metadata,
                                Score = result.Score,
                                Rank = result.Rank
                            })
                        .ToList();

                return Results.Ok(response);
            });

        group.MapDelete(
            "/collections/{collection}/documents/{documentId}",
            async (
                string collection,
                string documentId,
                ISemanticSearchEngine engine,
                CancellationToken cancellationToken) =>
            {
                await engine.DeleteAsync(
                    collection,
                    documentId,
                    cancellationToken);

                return Results.NoContent();
            });

        return app;
    }
}
