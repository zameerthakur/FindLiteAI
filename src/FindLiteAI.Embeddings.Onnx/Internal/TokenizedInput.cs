namespace FindLiteAI.Embeddings.Onnx.Internal;

/// <summary>
/// Represents tokenized ONNX model input values.
/// </summary>
/// <param name="InputIds">
/// The token identifier values.
/// </param>
/// <param name="AttentionMask">
/// The attention mask values.
/// </param>
/// <param name="TokenTypeIds">
/// The token type identifier values.
/// </param>
/// <param name="Length">
/// The actual number of tokens before padding.
/// </param>
internal sealed record TokenizedInput(
    long[] InputIds,
    long[] AttentionMask,
    long[] TokenTypeIds,
    int Length);
