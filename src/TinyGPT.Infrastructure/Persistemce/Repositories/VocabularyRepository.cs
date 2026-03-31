using TinyGPT.Domain.Entities;
using TinyGPT.Domain.Repositories;

namespace TinyGPT.Infrastructure.Persistemce.Repositories;

public class VocabularyRepository : IVocabularyRepository
{
    private readonly Dictionary<string, Token> wordToToken = new();
    private readonly Dictionary<int, Token> idToToken = new();

    public VocabularyRepository(IEnumerable<string> words)
    {
        int id = 0;
        foreach (var w in words.Distinct())
        {
            var token = new Token(id, w);
            wordToToken[w] = token;
            idToToken[id] = token;
            id++;
        }
    }

    public Token GetToken(string word) => wordToToken[word];
    public Token GetToken(int id) => idToToken[id];
    public IReadOnlyList<Token> GetAllTokens() => idToToken.Values.ToList();
}