namespace FindLiteAI.Internal.Math;

/// <summary>
/// Provides cosine similarity calculations for semantic embedding vectors.
/// </summary>
internal static class CosineSimilarityCalculator
{
    /// <summary>
    /// Calculates the cosine similarity between two embedding vectors.
    /// </summary>
    /// <param name="vectorA">
    /// The first embedding vector.
    /// </param>
    /// <param name="vectorB">
    /// The second embedding vector.
    /// </param>
    /// <returns>
    /// A cosine similarity score between -1 and 1.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when vector dimensions do not match.
    /// </exception>
    public static double Calculate(
        IReadOnlyList<float> vectorA,
        IReadOnlyList<float> vectorB)
    {
        if (vectorA.Count != vectorB.Count)
        {
            throw new ArgumentException(
                "Embedding vector dimensions must match.");
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int index = 0; index < vectorA.Count; index++)
        {
            dotProduct += vectorA[index] * vectorB[index];

            magnitudeA += vectorA[index] * vectorA[index];

            magnitudeB += vectorB[index] * vectorB[index];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct /
               (System.Math.Sqrt(magnitudeA) *
                System.Math.Sqrt(magnitudeB));
    }
}
