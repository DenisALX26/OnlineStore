using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OnlineStoreApp.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIService> _logger;

    public AIService(HttpClient httpClient, IConfiguration configuration, ILogger<AIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        // Configurează base URL
        var baseUrl = _configuration["AIService:BaseUrl"] ?? "https://api.openai.com";
        
        if (!string.IsNullOrEmpty(baseUrl))
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        // Setează content type
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetAnswerAsync(string question, string context)
    {
        try
        {
            var apiKey = _configuration["AIService:ApiKey"];
            var baseUrl = _configuration["AIService:BaseUrl"] ?? "https://api.openai.com";
            var endpoint = _configuration["AIService:Endpoint"] ?? "/v1/chat/completions";
            
            _logger.LogInformation("Attempting AI Service call. BaseUrl: {BaseUrl}, Endpoint: {Endpoint}, ApiKey configured: {HasKey}", 
                baseUrl, endpoint, !string.IsNullOrEmpty(apiKey));
            
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("API key is not configured. Returning default response.");
                return "The assistant service is currently unavailable. Please try again later.";
            }

            var requestBody = new
            {
                model = _configuration["AIService:Model"] ?? "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = $"You are a virtual assistant for an online footwear store. Context: {context}" },
                    new { role = "user", content = question }
                },
                max_tokens = int.Parse(_configuration["AIService:MaxTokens"] ?? "500"),
                temperature = double.Parse(_configuration["AIService:Temperature"] ?? "0.7")
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Creează un request cu header-ul de autentificare
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = content
            };
            
            // Determină tipul de autentificare bazat pe prefix-ul cheii
            var authType = _configuration["AIService:AuthType"] ?? "Bearer";
            
            // Pentru servicii custom, poate fi necesar un format diferit
            if (apiKey.StartsWith("sk-proj-"))
            {
                // Serviciu custom - poate necesita format diferit
                // Încearcă mai întâi cu Bearer, apoi cu header custom dacă e necesar
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                
                // Dacă serviciul necesită header custom, adaugă-l aici
                var customHeader = _configuration["AIService:CustomAuthHeader"];
                if (!string.IsNullOrEmpty(customHeader))
                {
                    request.Headers.Add(customHeader, apiKey);
                }
            }
            else
            {
                // Format standard OpenAI
                request.Headers.Authorization = new AuthenticationHeaderValue(authType, apiKey);
            }
            
            var fullUrl = baseUrl.TrimEnd('/') + endpoint;
            _logger.LogInformation("Sending request to: {Url}, ApiKey prefix: {Prefix}, AuthType: {AuthType}", 
                fullUrl, apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0)) ?? "N/A", authType);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);

                // Extrage răspunsul din JSON (adaptat pentru formatul OpenAI)
                if (responseJson.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var contentElement))
                    {
                        return contentElement.GetString() ?? "Could not generate a response.";
                    }
                }

                _logger.LogError("Unexpected API response format: {Response}", responseContent);
                return "Received an unexpected response from the AI service.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error calling API: {StatusCode} - {Error}. Request URL: {Url}", 
                    response.StatusCode, errorContent, _httpClient.BaseAddress + endpoint);
                
                // Gestionare specifică pentru diferite tipuri de erori
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("API key is invalid or expired. Check the API key in User Secrets.");
                    return "API key is invalid or expired. Please check your configuration or contact the administrator.";
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests || 
                    response.StatusCode == (System.Net.HttpStatusCode)429)
                {
                    // Încearcă să extragă mesajul de eroare din JSON
                    try
                    {
                        var errorJson = JsonDocument.Parse(errorContent);
                        if (errorJson.RootElement.TryGetProperty("error", out var errorObj))
                        {
                            if (errorObj.TryGetProperty("message", out var message))
                            {
                                var errorMessage = message.GetString();
                                if (errorMessage?.Contains("quota") == true || 
                                    errorMessage?.Contains("insufficient_quota") == true)
                                {
                                    _logger.LogWarning("OpenAI quota exceeded. Using local logic as fallback.");
                                    return "AI service quota has been exceeded. Using local responses.";
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Dacă nu putem parsa JSON-ul, continuăm cu mesajul generic
                    }
                    
                    _logger.LogWarning("Too many requests to AI service (Rate Limit). Using local logic as fallback.");
                    return "AI service is temporarily unavailable due to usage limits. Using local responses.";
                }
                
                // Returnează un mesaj mai specific pentru debugging
                return $"Could not get a response from the AI service (Status: {response.StatusCode}). Please try again later.";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling AI service");
            return "Connection error to the AI service. Please try again later.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in AI service");
            return "An unexpected error occurred. Please try again later.";
        }
    }
}

