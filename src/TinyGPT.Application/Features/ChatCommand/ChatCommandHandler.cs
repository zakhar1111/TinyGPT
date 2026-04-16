using Microsoft.Extensions.Configuration;
using System.Text;
using TinyGPT.Domain.Services;

namespace TinyGPT.Application.Features.ChatCommand;

public sealed class ChatCommandHandler
    : IOperationHandler<ChatCommand, string>
{
    private readonly IGenerationService _generationService;
    private readonly int _seqLength;

    public ChatCommandHandler(
        IGenerationService generationService,
        IConfiguration config)
    {
        _generationService = generationService;
        _seqLength = config.GetValue<int>("ModelConfig:SeqLength");
    }

    public async Task<string> HandleAsync(
        ChatCommand request,
        CancellationToken ct = default)
    {
        if (request.Messages == null || request.Messages.Count == 0)
            throw new ArgumentException("Messages cannot be empty.");

        var prompt = BuildPrompt(request.Messages); 

        var result = await _generationService.GenerateAsync(
            prompt,
            request.MaxTokens,
            _seqLength);

        return result;
    }

    private static string BuildPrompt(List<ChatMessageDto> messages)
    {
        var sb = new StringBuilder();

        foreach (var msg in messages) 
            sb.Append(msg.Content).Append(" ");

        return sb.ToString();
    }
}