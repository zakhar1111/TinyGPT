using TinyGPT.Domain.Aggregates;
using TinyGPT.Domain.Entities;
using TinyGPT.Domain.Helpers;
using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Services;

namespace TinyGPT.Infrastructure.Services;

public class TrainingService 
    : ITrainingService
{
    private readonly Transformer _transformer;
    private readonly IVocabularyRepository _vocab;
    private readonly IEmbeddingRepository _embeddingRepo;
    private readonly int _seqLength;
    //private readonly Random _rnd;

    public TrainingService(
        Transformer transformer, 
        IVocabularyRepository vocab,
        IEmbeddingRepository embeddingRepo,
        int seqLength,
        CancellationToken ct = default)//,Random rnd)
    {
        _transformer = transformer;
        _vocab = vocab;
        _embeddingRepo = embeddingRepo;
        _seqLength = seqLength;
        //_rnd = rnd;
    }

    public async Task TrainAsync(string text, int epochs = 100, float lr = 0.01f, CancellationToken ct = default)
    {
        // 🔥 Cancellation support 
        ct.ThrowIfCancellationRequested();

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // ✅ 1. Build vocab
        _vocab.Build(words);

        // ✅ 2. Init embeddings
        int vocabSize = _vocab.GetAllTokens().Count;
        _embeddingRepo.Initialize(vocabSize);

        // ✅ 3. Init output layer
        _transformer.InitializeOutputLayer(vocabSize);

        var sequences = BuildSequences(words);


        int embedSize = _transformer.WoFinal.GetLength(0);
        //int vocabSize = _vocab.GetAllTokens().Count;

        // ✅ 4. Train with simplified forward + loss + gradient update
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            // 🔥 Cancellation support 
            ct.ThrowIfCancellationRequested();

            float loss = 0;
            foreach (var seq in sequences)
            {
                // 🔥 Cancellation support 
                ct.ThrowIfCancellationRequested();

                var tokenIds = seq.Tokens.Select(t => t.Id).ToArray();
                var X = _transformer.EmbedWithPosition(tokenIds);

                var output = _transformer.Forward(X);
                var logits = MatrixOperations.MatMul(output[^1], _transformer.WoFinal);
                var probs = SoftmaxService.Softmax(logits);

                loss += -MathF.Log(probs[seq.Target.Id] + 1e-8f);

                // simplified gradient update for WoFinal
                for (int i = 0; i < embedSize; i++)
                {
                    if ((i & 15) == 0) // every 16 iterations
                        ct.ThrowIfCancellationRequested();
                    for (int j = 0; j < vocabSize; j++)
                    {
                        _transformer.WoFinal[i, j] -=
                            lr * (probs[j] - (j == seq.Target.Id ? 1 : 0)) * output[^1][i];
                    }
                }
            }


            if (epoch % 20 == 0)
            {
                Console.WriteLine($"Epoch {epoch}, Loss: {loss:F2}");
                await Task.Delay(1, ct); 
            }
        }
    }

    // ================= DSA: Sliding Window =================
    private  List<Sequence> BuildSequences(string[] words)
    {
        var sequences = new List<Sequence>();

        for (int i = 0; i < words.Length - _seqLength; i++)
        {
            var input = new List<Token>();

            for (int j = 0; j < _seqLength; j++)
                input.Add(_vocab.GetToken(words[i + j]));

            var target = _vocab.GetToken(words[i + _seqLength]);

            sequences.Add(new Sequence(input, target));
        }

        return sequences;
    }
}
