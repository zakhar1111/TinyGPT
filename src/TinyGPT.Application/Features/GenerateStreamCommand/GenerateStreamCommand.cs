namespace TinyGPT.Application.Features.GenerateStreamCommand;

public class GenerateStreamCommand
    : IRequest<IAsyncEnumerable<string>>
{
    public string Prompt { get; set; } = default!;
    public int MaxTokens { get; set; } = 50;
}
