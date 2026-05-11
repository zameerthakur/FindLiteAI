namespace FindLiteAI.Internal.Math;

/// <summary>
/// Provides hybrid relevance score calculations.
/// </summary>
internal static class HybridScoreCalculator
{
    private const double SemanticWeight = 0.70;
    private const double KeywordWeight = 0.30;

    /// <summary>
    /// Combines semantic similarity and keyword relevance
    /// into a single hybrid score.
    /// </summary>
    /// <param name="semanticScore">
    /// The semantic similarity score.
    /// </param>
    /// <param name="keywordScore">
    /// The keyword relevance score.
    /// </param>
    /// <returns>
    /// A normalized hybrid relevance score.
    /// </returns>
    public static double Calculate(
        double semanticScore,
        double keywordScore)
    {
        semanticScore = Normalize(semanticScore);
        keywordScore = Normalize(keywordScore);

        return (semanticScore * SemanticWeight) +
               (keywordScore * KeywordWeight);
    }

    /// <summary>
    /// Normalizes a score into the 0 to 1 range.
    /// </summary>
    /// <param name="score">
    /// The source score.
    /// </param>
    /// <returns>
    /// A normalized score.
    /// </returns>
    private static double Normalize(double score)
    {
        if (score < 0)
        {
            return 0;
        }

        if (score > 1)
        {
            return 1;
        }

        return score;
    }
}
