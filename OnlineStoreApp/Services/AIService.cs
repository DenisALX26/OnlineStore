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
            
            _logger.LogInformation("Încercare apel AI Service. BaseUrl: {BaseUrl}, Endpoint: {Endpoint}, ApiKey configurată: {HasKey}", 
                baseUrl, endpoint, !string.IsNullOrEmpty(apiKey));
            
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Cheia API nu este configurată. Returnez răspuns default.");
                return "Serviciul de asistent nu este disponibil momentan. Vă rugăm să încercați mai târziu.";
            }

            var requestBody = new
            {
                model = _configuration["AIService:Model"] ?? "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = $"Ești un asistent virtual pentru un magazin online de încălțăminte. Context: {context}" },
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
            _logger.LogInformation("Trimitere request către: {Url}, ApiKey prefix: {Prefix}, AuthType: {AuthType}", 
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
                        return contentElement.GetString() ?? "Nu am putut genera un răspuns.";
                    }
                }

                _logger.LogError("Formatul răspunsului API este neașteptat: {Response}", responseContent);
                return "Am primit un răspuns neașteptat de la serviciul de AI.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Eroare la apelul API: {StatusCode} - {Error}. Request URL: {Url}", 
                    response.StatusCode, errorContent, _httpClient.BaseAddress + endpoint);
                
                // Gestionare specifică pentru diferite tipuri de erori
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Cheia API este invalidă sau expirată. Verifică cheia API în User Secrets.");
                    return "Cheia API este invalidă sau expirată. Te rugăm să verifici configurația sau să contactezi administratorul.";
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
                                    _logger.LogWarning("Quota OpenAI depășit. Folosind logica locală ca fallback.");
                                    return "Quota serviciului AI a fost depășită. Folosim răspunsuri locale.";
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Dacă nu putem parsa JSON-ul, continuăm cu mesajul generic
                    }
                    
                    _logger.LogWarning("Prea multe cereri către serviciul AI (Rate Limit). Folosind logica locală ca fallback.");
                    return "Serviciul AI este temporar indisponibil din cauza limitelor de utilizare. Folosim răspunsuri locale.";
                }
                
                // Returnează un mesaj mai specific pentru debugging
                return $"Nu am putut obține un răspuns de la serviciul de AI (Status: {response.StatusCode}). Vă rugăm să încercați mai târziu.";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Eroare de rețea la apelul serviciului AI");
            return "Eroare de conexiune la serviciul de AI. Vă rugăm să încercați mai târziu.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare neașteptată în serviciul AI");
            return "A apărut o eroare neașteptată. Vă rugăm să încercați mai târziu.";
        }
    }
}

