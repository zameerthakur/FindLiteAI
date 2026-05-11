using FindLiteAI.Core.Enums;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Internal.Math;

namespace FindLiteAI.Internal.Services;

/// <summary>
/// Provides internal ranking logic for semantic, keyword, and hybrid search results.
/// </summary>
internal static class SearchRankingService
{
    /// <summary>
    /// Ranks indexed documents against a query embedding and query text.
    /// </summary>
    /// <param name="items">The indexed documents with their embedding vectors.</param>
    /// <param name="query">The original search query.</param>
    /// <param name="queryEmbedding">The query embedding vector.</param>
    /// <param name="options">The search options.</param>
    /// <returns>A ranked list of search results.</returns>
    public static IReadOnlyList<SearchResult> Rank(
        IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> items,
        string query,
        IReadOnlyList<float> queryEmbedding,
        SearchOptions options)
    {
        List<SearchResult> results = [];

        foreach ((SemanticDocument document, IReadOnlyList<float> embedding) in items)
        {
            double semanticScore = CosineSimilarityCalculator.Calculate(
                queryEmbedding,
                embedding);

            double keywordScore = KeywordMatcher.CalculateScore(
                query,
                document.Text);

            double finalScore = options.SearchMode switch
            {
                SearchMode.Semantic => semanticScore,
                SearchMode.Keyword => keywordScore,
                SearchMode.Hybrid => HybridScoreCalculator.Calculate(
                    semanticScore,
                    keywordScore),
                _ => HybridScoreCalculator.Calculate(
                    semanticScore,
                    keywordScore)
            };

            if (finalScore < options.MinimumScore)
            {
                continue;
            }

            results.Add(new SearchResult
            {
                Document = document,
                Score = finalScore
            });
        }

        return results
            .OrderByDescending(result => result.Score)
            .ThenBy(result => result.Document.Id, StringComparer.Ordinal)
            .Take(options.MaxResults)
            .Select((result, index) =>
            {
                result.Rank = index;
                return result;
            })
            .ToList();
    }
}
