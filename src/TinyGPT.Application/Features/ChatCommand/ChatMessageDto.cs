namespace TinyGPT.Application.Features.ChatCommand;

public sealed class ChatMessageDto
{
    //public string Role { get; set; } = default!; //  user | assistant //TODO :fix request with role 
    public string Content { get; set; } = default!;
}