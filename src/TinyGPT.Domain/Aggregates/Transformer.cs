using TinyGPT.Domain.Helpers;
using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Services;

namespace TinyGPT.Domain.Aggregates;

public class Transformer
{
    private readonly int _embedSize;
    private readonly int _seqLength;
    private readonly int _numHeads;
    private readonly Random _rnd;

    private readonly IEmbeddingRepository _embeddingRepo;

    // Model weights
    public List<float[,]> Wq_heads = new();
    public List<float[,]> Wk_heads = new();
    public List<float[,]> Wv_heads = new();
    public float[,] Wo;
    public float[,] WoFinal;
    public float[,] W1;
    public float[,] W2;

    public Transformer(IEmbeddingRepository embeddingRepo, int embedSize, int seqLength, int numHeads, Random rnd)
    {
        _embeddingRepo = embeddingRepo;
        _embedSize = embedSize;
        _seqLength = seqLength;
        _numHeads = numHeads;
        _rnd = rnd;

        InitWeights();
    }

    private void InitWeights()
    {
        for (int i = 0; i < _numHeads; i++)
        {
            Wq_heads.Add(MatrixOperations.RandomMatrix(_embedSize, _embedSize, _rnd));
            Wk_heads.Add(MatrixOperations.RandomMatrix(_embedSize, _embedSize, _rnd));
            Wv_heads.Add(MatrixOperations.RandomMatrix(_embedSize, _embedSize, _rnd));
        }
        Wo = MatrixOperations.RandomMatrix(_embedSize * _numHeads, _embedSize, _rnd);
        //WoFinal = MatrixOperations.RandomMatrix(_embedSize, _embeddingRepo.Embeddings.GetLength(0), _rnd);
        W1 = MatrixOperations.RandomMatrix(_embedSize, _embedSize * 2, _rnd);
        W2 = MatrixOperations.RandomMatrix(_embedSize * 2, _embedSize, _rnd);
    }

    // called AFTER vocab is known
    public void InitializeOutputLayer(int vocabSize)
    {
        WoFinal = MatrixOperations.RandomMatrix(_embedSize, vocabSize, _rnd);
    }

    // Transformer block
    public float[][] Forward(float[][] X)
    {
        var attn = MultiHeadAttention(X);
        var x1 = AddAndNorm(X, attn);
        var ff = FeedForward(x1);
        return AddAndNorm(x1, ff);
    }

    private float[][] MultiHeadAttention(float[][] X)
    {
        var heads = new List<float[][]>();
        for (int h = 0; h < _numHeads; h++)
        {
            var Q = MatrixOperations.MatMul(X, Wq_heads[h]);
            var K = MatrixOperations.MatMul(X, Wk_heads[h]);
            var V = MatrixOperations.MatMul(X, Wv_heads[h]);

            var scores = MatrixOperations.MatMul(Q, MatrixOperations.Transpose(K));

            // causal mask
            for (int i = 0; i < scores.Length; i++)
                for (int j = i + 1; j < scores.Length; j++)
                    scores[i][j] = -1e9f;

            MatrixOperations.Scale(scores, 1.0f / MathF.Sqrt(_embedSize));

            var weights = SoftmaxService.SoftmaxRows(scores);
            var attn = MatrixOperations.MatMul(weights, V);
            heads.Add(attn);
        }
        var concat = MatrixOperations.ConcatHeads(heads);
        return MatrixOperations.MatMul(concat, Wo);
    }

    private float[][] FeedForward(float[][] X)
    {
        var hidden = MatrixOperations.MatMul(X, W1);
        ApplyReLU(hidden);
        return MatrixOperations.MatMul(hidden, W2);
    }

    private float[][] AddAndNorm(float[][] X, float[][] sub)
    {
        var result = new float[X.Length][];
        for (int i = 0; i < X.Length; i++)
        {
            result[i] = new float[_embedSize];
            for (int j = 0; j < _embedSize; j++)
                result[i][j] = X[i][j] + sub[i][j];
        }
        return LayerNorm(result);
    }

    private float[][] LayerNorm(float[][] X)
    {
        for (int i = 0; i < X.Length; i++)
        {
            float mean = X[i].Average();
            float var = X[i].Select(v => (v - mean) * (v - mean)).Average();
            float std = MathF.Sqrt(var + 1e-5f);
            for (int j = 0; j < X[i].Length; j++)
                X[i][j] = (X[i][j] - mean) / std;
        }
        return X;
    }

    private void ApplyReLU(float[][] X)
    {
        for (int i = 0; i < X.Length; i++)
            for (int j = 0; j < X[i].Length; j++)
                X[i][j] = Math.Max(0, X[i][j]);
    }

    public float[][] EmbedWithPosition(int[] tokenIds)
    {
        var result = new float[tokenIds.Length][];
        for (int i = 0; i < tokenIds.Length; i++)
        {
            result[i] = new float[_embedSize];
            for (int j = 0; j < _embedSize; j++)
                result[i][j] = _embeddingRepo.Embeddings[tokenIds[i], j] + _embeddingRepo.Positional[i, j];
        }
        return result;
    }
}

