namespace FindLiteAI.Internal.Services;

/// <summary>
/// Provides lightweight keyword relevance scoring.
/// </summary>
internal static class KeywordMatcher
{
    /// <summary>
    /// Calculates a keyword relevance score between a query and source text.
    /// </summary>
    /// <param name="query">
    /// The search query.
    /// </param>
    /// <param name="text">
    /// The source document text.
    /// </param>
    /// <returns>
    /// A normalized keyword relevance score between 0 and 1.
    /// </returns>
    public static double CalculateScore(
        string query,
        string text)
    {
        if (string.IsNullOrWhiteSpace(query) ||
            string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        string[] queryTerms =
            query.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

        if (queryTerms.Length == 0)
        {
            return 0;
        }

        int matchedTerms = 0;

        foreach (string queryTerm in queryTerms)
        {
            if (text.Contains(
                queryTerm,
                StringComparison.OrdinalIgnoreCase))
            {
                matchedTerms++;
            }
        }

        return (double)matchedTerms / queryTerms.Length;
    }
}
