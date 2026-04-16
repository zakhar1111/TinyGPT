using Microsoft.Extensions.DependencyInjection;
using TinyGPT.Application.Features.ChatCommand;
using TinyGPT.Application.Features.GenerateStreamCommand;
using TinyGPT.Application.Features.GenerateTextCommand;
using TinyGPT.Application.Features.TrainModelCommand;

namespace TinyGPT.Application.Extensions;

public static class DIExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOperationHandler<TrainModelCommand, string>, TrainModelCommandHandler>();
        services.AddScoped<IOperationHandler<GenerateTextCommand, string>, GenerateTextHandler>();
        services.AddScoped<IOperationHandler<GenerateStreamCommand, IAsyncEnumerable<string>>, GenerateStreamHandler>();
        services.AddScoped<IOperationHandler<ChatCommand, string>, ChatCommandHandler>();

        services.AddScoped<OperationExecutor>();
    }
}
