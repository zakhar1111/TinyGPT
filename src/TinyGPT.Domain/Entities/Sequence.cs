namespace TinyGPT.Domain.Entities;

public class Sequence
{
    public IReadOnlyList<Token> Tokens { get; }
    public Token Target { get; }

    public Sequence(IReadOnlyList<Token> tokens, Token target)
    {
        Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        Target = target ?? throw new ArgumentNullException(nameof(target));
    }
}

