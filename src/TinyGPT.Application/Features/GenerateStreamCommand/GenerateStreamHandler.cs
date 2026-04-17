
using Microsoft.Extensions.Configuration;
using TinyGPT.Domain.Services;

namespace TinyGPT.Application.Features.GenerateStreamCommand;

public class GenerateStreamHandler
    : IOperationHandler<GenerateStreamCommand, IAsyncEnumerable<string>>
{
    private readonly IGenerationService _generationService;
    private readonly int _seqLength;
    public GenerateStreamHandler(
        IGenerationService generationService,
        IConfiguration config
    )
    {
        _generationService = generationService;
        _seqLength = config.GetValue<int>("ModelConfig:SeqLength"); ;
    }

    public async Task<IAsyncEnumerable<string>?> HandleAsync(
        GenerateStreamCommand request, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            throw new ArgumentException("Start text cannot be empty.");

        var stream = _generationService.GenerateStreamAsync(
            request.Prompt, 
            request.MaxTokens,
            _seqLength,
            ct);

        return stream;
    }
}
