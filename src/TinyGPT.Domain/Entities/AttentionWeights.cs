namespace TinyGPT.Domain.Entities;

public class AttentionWeights
{
    public float[][] Values { get; }
    public AttentionWeights(float[][] values) =>
        Values = values ?? throw new ArgumentNullException(nameof(values));
}

