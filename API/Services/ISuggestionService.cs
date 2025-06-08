namespace API.Services
{
    public interface ISuggestionService
    {
        Task<string> GetSuggestionAsync(int userId);
    }
}