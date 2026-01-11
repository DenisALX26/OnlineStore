namespace OnlineStoreApp.Services;

public interface IAIService
{
    Task<string> GetAnswerAsync(string question, string context);
}

