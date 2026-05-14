using FindLiteAI.AspNetCore.Models;
using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Exceptions;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

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
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                ILogger logger =
                    loggerFactory.CreateLogger("FindLiteAI.AspNetCore");

                try
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
                }
                catch (ArgumentException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Invalid add document request for collection '{Collection}'.",
                        collection);

                    return Results.BadRequest(
                        CreateProblemDetails(
                            "Invalid request.",
                            exception.Message,
                            StatusCodes.Status400BadRequest));
                }
                catch (SearchException exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to add document to collection '{Collection}'.",
                        collection);

                    return Results.Problem(
                        title: "FindLiteAI operation failed.",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });

        group.MapPost(
            "/collections/{collection}/documents/batch",
            async (
                string collection,
                AddDocumentsRequest request,
                ISemanticSearchEngine engine,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                ILogger logger =
                    loggerFactory.CreateLogger("FindLiteAI.AspNetCore");

                try
                {
                    IReadOnlyList<SemanticDocument> documents =
                        request.Documents
                            .Select(document =>
                                new SemanticDocument
                                {
                                    Id = document.Id,
                                    Text = document.Text,
                                    Metadata = document.Metadata
                                })
                            .ToList();

                    await engine.AddRangeAsync(
                        collection,
                        documents,
                        cancellationToken);

                    return Results.Ok();
                }
                catch (ArgumentException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Invalid batch add request for collection '{Collection}'.",
                        collection);

                    return Results.BadRequest(
                        CreateProblemDetails(
                            "Invalid request.",
                            exception.Message,
                            StatusCodes.Status400BadRequest));
                }
                catch (SearchException exception)
                {
                    logger.LogError(
                        exception,
                        "Failed to add documents to collection '{Collection}'.",
                        collection);

                    return Results.Problem(
                        title: "FindLiteAI batch add failed.",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });

        group.MapPost(
            "/collections/{collection}/search",
            async (
                string collection,
                SearchRequest request,
                ISemanticSearchEngine engine,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                ILogger logger =
                    loggerFactory.CreateLogger("FindLiteAI.AspNetCore");

                try
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
                            .Select(MapSearchResponse)
                            .ToList();

                    return Results.Ok(response);
                }
                catch (ArgumentException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Invalid search request for collection '{Collection}'.",
                        collection);

                    return Results.BadRequest(
                        CreateProblemDetails(
                            "Invalid request.",
                            exception.Message,
                            StatusCodes.Status400BadRequest));
                }
                catch (SearchException exception)
                {
                    logger.LogError(
                        exception,
                        "Search failed for collection '{Collection}'.",
                        collection);

                    return Results.Problem(
                        title: "FindLiteAI search failed.",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });

        group.MapGet(
            "/collections/{collection}/documents/{documentId}/similar",
            async (
                string collection,
                string documentId,
                ISemanticSearchEngine engine,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                ILogger logger =
                    loggerFactory.CreateLogger("FindLiteAI.AspNetCore");

                try
                {
                    IReadOnlyList<SearchResult> results =
                        await engine.FindSimilarAsync(
                            collection,
                            documentId,
                            cancellationToken: cancellationToken);

                    IReadOnlyList<SearchResponse> response =
                        results
                            .Select(MapSearchResponse)
                            .ToList();

                    return Results.Ok(response);
                }
                catch (ArgumentException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Invalid similar search request for collection '{Collection}'.",
                        collection);

                    return Results.BadRequest(
                        CreateProblemDetails(
                            "Invalid request.",
                            exception.Message,
                            StatusCodes.Status400BadRequest));
                }
                catch (SearchException exception)
                {
                    logger.LogError(
                        exception,
                        "Similar search failed for document '{DocumentId}' in collection '{Collection}'.",
                        documentId,
                        collection);

                    return Results.Problem(
                        title: "FindLiteAI similar search failed.",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });

        group.MapDelete(
            "/collections/{collection}/documents/{documentId}",
            async (
                string collection,
                string documentId,
                ISemanticSearchEngine engine,
                ILoggerFactory loggerFactory,
                CancellationToken cancellationToken) =>
            {
                ILogger logger =
                    loggerFactory.CreateLogger("FindLiteAI.AspNetCore");

                try
                {
                    await engine.DeleteAsync(
                        collection,
                        documentId,
                        cancellationToken);

                    return Results.NoContent();
                }
                catch (ArgumentException exception)
                {
                    logger.LogWarning(
                        exception,
                        "Invalid delete request for collection '{Collection}'.",
                        collection);

                    return Results.BadRequest(
                        CreateProblemDetails(
                            "Invalid request.",
                            exception.Message,
                            StatusCodes.Status400BadRequest));
                }
                catch (SearchException exception)
                {
                    logger.LogError(
                        exception,
                        "Delete failed for document '{DocumentId}' in collection '{Collection}'.",
                        documentId,
                        collection);

                    return Results.Problem(
                        title: "FindLiteAI delete failed.",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });

        return app;
    }

    private static SearchResponse MapSearchResponse(
        SearchResult result)
    {
        return new SearchResponse
        {
            Id = result.Document.Id,
            Text = result.Document.Text,
            Metadata = result.Document.Metadata,
            Score = result.Score,
            Rank = result.Rank
        };
    }

    private static object CreateProblemDetails(
        string title,
        string detail,
        int statusCode)
    {
        return new
        {
            title,
            detail,
            status = statusCode
        };
    }
}
