namespace TinyGPT.Application.Features.ChatCommand;

public sealed class ChatCommand 
    : IRequest<string>
{
    public List<ChatMessageDto> Messages { get; }
    public int MaxTokens { get; }

    public ChatCommand(List<ChatMessageDto> messages, int maxTokens)
    {
        Messages = messages;
        MaxTokens = maxTokens;
    }
}