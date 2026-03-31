using TinyGPT.Domain.Aggregates;
using TinyGPT.Domain.Entities;
using TinyGPT.Domain.Helpers;
using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Services;

namespace TinyGPT.Infrastructure.Services;

public class TrainingService
{
    private readonly Transformer _transformer;
    private readonly IVocabularyRepository _vocab;
    private readonly Random _rnd;

    public TrainingService(Transformer transformer, IVocabularyRepository vocab, Random rnd)
    {
        _transformer = transformer;
        _vocab = vocab;
        _rnd = rnd;
    }

    public void Train(List<Sequence> sequences, int epochs = 100, float lr = 0.01f)
    {
        int embedSize = _transformer.WoFinal.GetLength(0);
        int vocabSize = _vocab.GetAllTokens().Count;

        for (int epoch = 0; epoch < epochs; epoch++)
        {
            float loss = 0;
            foreach (var seq in sequences)
            {
                var tokenIds = seq.Tokens.Select(t => t.Id).ToArray();
                var X = _transformer.EmbedWithPosition(tokenIds);

                var output = _transformer.Forward(X);
                var logits = MatrixOperations.MatMul(output[^1], _transformer.WoFinal);
                var probs = SoftmaxService.Softmax(logits);

                loss += -MathF.Log(probs[seq.Target.Id] + 1e-8f);

                // simplified gradient update for WoFinal
                for (int i = 0; i < embedSize; i++)
                    for (int j = 0; j < vocabSize; j++)
                        _transformer.WoFinal[i, j] -= lr * (probs[j] - (j == seq.Target.Id ? 1 : 0)) * output[^1][i];
            }

            if (epoch % 20 == 0)
                Console.WriteLine($"Epoch {epoch}, Loss: {loss:F2}");
        }
    }
}
