using Microsoft.Extensions.DependencyInjection;
using TinyGPT.Application.Features.GenerateTextQuery;
using TinyGPT.Application.Features.TrainModelCommand;

namespace TinyGPT.Application.Extensions;

public static class DIExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOperationHandler<TrainModelCommand, string>, TrainModelCommandHandler>();
        services.AddScoped<IOperationHandler<GenerateTextQuery, string>, GenerateTextQueryHandler>();

        services.AddScoped<OperationExecutor>();
    }
}
