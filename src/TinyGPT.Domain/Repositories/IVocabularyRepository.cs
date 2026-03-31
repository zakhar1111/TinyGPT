using TinyGPT.Domain.Entities;

namespace TinyGPT.Domain.Repositories;

public interface IVocabularyRepository
{
    Token GetToken(string word);
    Token GetToken(int id);
    IReadOnlyList<Token> GetAllTokens();
}
