using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TinyGPT.Domain.Aggregates;
using TinyGPT.Domain.Repositories;
using TinyGPT.Domain.Services;
using TinyGPT.Infrastructure.Persistemce.Repositories;
using TinyGPT.Infrastructure.Services;

namespace TinyGPT.Infrastructure.Extensions;

public static class DIExtensions
{
    public static void AddIfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // ================= CONFIG =================
        int embedSize = config.GetValue<int>("ModelConfig:EmbedSize");
        int seqLength = config.GetValue<int>("ModelConfig:SeqLength");
        int numHeads = config.GetValue<int>("ModelConfig:NumHeads");

        services.AddSingleton<Random>();

        // ================= REPOSITORIES =================
        services.AddSingleton<IVocabularyRepository, VocabularyRepository>();
        services.AddSingleton<IEmbeddingRepository>(sp =>
        {
            var rnd = sp.GetRequiredService<Random>();
            return new EmbeddingRepository(
                seqLength, 
                embedSize, 
                rnd);
        });

        // ================= AGGREGATE =================
        services.AddSingleton<Transformer>(sp =>
        {
            var embeddingRepo = sp.GetRequiredService<IEmbeddingRepository>();
            var rnd = sp.GetRequiredService<Random>();

            return new Transformer(
                embeddingRepo,
                embedSize,
                seqLength,
                numHeads,
                rnd);
        });

        // ================= SERVICES =================
        services.AddSingleton<ITrainingService>(sp =>
        {
            var transformer = sp.GetRequiredService<Transformer>();
            var vocab = sp.GetRequiredService<IVocabularyRepository>();
            var embeddingRepo = sp.GetRequiredService<IEmbeddingRepository>();

            return new TrainingService(
                transformer,
                vocab,
                embeddingRepo,
                seqLength
            );
        });


        services.AddSingleton<IGenerationService>(sp =>
        {
            var transformer = sp.GetRequiredService<Transformer>();
            var vocab = sp.GetRequiredService<IVocabularyRepository>();
            var rnd = sp.GetRequiredService<Random>();

            return new GenerationService(
                transformer,
                vocab,
                rnd);
        });

    }

}
