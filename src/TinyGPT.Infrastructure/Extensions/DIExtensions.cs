using Microsoft.Extensions.DependencyInjection;
using TinyGPT.Infrastructure.Services;

namespace TinyGPT.Infrastructure.Extensions;

public static class DIExtensions
{
    public static void AddTinyGPTServices(this IServiceCollection services)
    {
        services.AddSingleton<TrainingService>();
        services.AddSingleton<GenerationService>();
    }
}
