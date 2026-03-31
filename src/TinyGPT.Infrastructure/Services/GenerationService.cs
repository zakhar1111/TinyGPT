using TinyGPT.Domain.Aggregates;
using TinyGPT.Domain.Helpers;
using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Services;

namespace TinyGPT.Infrastructure.Services;

public class GenerationService
{
    private readonly Transformer _transformer;
    private readonly IVocabularyRepository _vocab;
    private readonly Random _rnd;

    public GenerationService(Transformer transformer, IVocabularyRepository vocab, Random rnd)
    {
        _transformer = transformer;
        _vocab = vocab;
        _rnd = rnd;
    }

    public string Generate(string start, int length, int seqLength)
    {
        var tokens = start.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                          .Select(w => _vocab.GetToken(w).Id)
                          .ToList();

        var window = new Queue<int>(tokens);
        while (window.Count < seqLength)
            window.Enqueue(window.Peek());

        var result = new List<string>(tokens.Select(id => _vocab.GetToken(id).Value));

        for (int step = 0; step < length; step++)
        {
            var input = window.ToArray();
            var X = _transformer.EmbedWithPosition(input);
            var output = _transformer.Forward(X);
            var logits = MatrixOperations.MatMul(output[^1], _transformer.WoFinal);
            var probs = SoftmaxService.Softmax(logits);

            int next = SampleWithBinarySearch(probs);
            window.Dequeue();
            window.Enqueue(next);
            result.Add(_vocab.GetToken(next).Value);
        }
        return string.Join(" ", result);
    }

    private int SampleWithBinarySearch(float[] probs)
    {
        float[] prefix = new float[probs.Length];
        prefix[0] = probs[0];
        for (int i = 1; i < probs.Length; i++)
            prefix[i] = prefix[i - 1] + probs[i];

        float r = (float)_rnd.NextDouble() * prefix[^1];
        int left = 0, right = prefix.Length - 1;
        while (left < right)
        {
            int mid = (left + right) / 2;
            if (prefix[mid] >= r) right = mid;
            else left = mid + 1;
        }
        return left;
    }
}
