using Microsoft.ML.Tokenizers;

namespace FindLiteAI.Embeddings.Onnx.Internal;

/// <summary>
/// Provides tokenization support for ONNX embedding models.
/// </summary>
internal sealed class TokenizerService
{
    private readonly BertTokenizer _tokenizer;
    private readonly int _maxTokenLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenizerService"/> class.
    /// </summary>
    /// <param name="options">
    /// The ONNX embedding provider options.
    /// </param>
    public TokenizerService(OnnxEmbeddingProviderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!File.Exists(options.VocabularyPath))
        {
            throw new FileNotFoundException(
                "The tokenizer vocabulary file could not be found.",
                options.VocabularyPath);
        }

        _tokenizer = BertTokenizer.Create(options.VocabularyPath);

        _maxTokenLength = options.MaxTokenLength;
    }

    /// <summary>
    /// Tokenizes text into ONNX model input tensors.
    /// </summary>
    /// <param name="text">
    /// The source text.
    /// </param>
    /// <returns>
    /// Tokenized model inputs.
    /// </returns>
    public TokenizedInput Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be null or empty.",
                nameof(text));
        }

        IReadOnlyList<int> tokenIds =
            _tokenizer.EncodeToIds(text);

        long[] inputIds = new long[_maxTokenLength];
        long[] attentionMask = new long[_maxTokenLength];
        long[] tokenTypeIds = new long[_maxTokenLength];

        int length = Math.Min(
            tokenIds.Count,
            _maxTokenLength);

        for (int index = 0; index < length; index++)
        {
            inputIds[index] = tokenIds[index];
            attentionMask[index] = 1;
            tokenTypeIds[index] = 0;
        }

        return new TokenizedInput(
            inputIds,
            attentionMask,
            tokenTypeIds,
            length);
    }
}
