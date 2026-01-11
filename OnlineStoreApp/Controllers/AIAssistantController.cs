using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Services;
using System.Text.RegularExpressions;

namespace OnlineStoreApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAssistantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIService _aiService;
        private readonly ILogger<AIAssistantController> _logger;

        public AIAssistantController(
            ApplicationDbContext context, 
            IAIService aiService,
            ILogger<AIAssistantController> logger)
        {
            _context = context;
            _aiService = aiService;
            _logger = logger;
        }

        [HttpGet("test-config")]
        public IActionResult TestConfig()
        {
            var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = config["AIService:ApiKey"];
            var baseUrl = config["AIService:BaseUrl"];
            var endpoint = config["AIService:Endpoint"];
            
            return Ok(new
            {
                ApiKeyConfigured = !string.IsNullOrEmpty(apiKey),
                ApiKeyLength = apiKey?.Length ?? 0,
                ApiKeyPrefix = apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0)) ?? "N/A",
                BaseUrl = baseUrl,
                Endpoint = endpoint,
                Message = string.IsNullOrEmpty(apiKey) 
                    ? "Cheia API nu este configurată. Configurează-o cu: dotnet user-secrets set \"AIService:ApiKey\" \"your-key\" --project OnlineStoreApp"
                    : "Cheia API este configurată corect."
            });
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] AskQuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question) || request.ProductId <= 0)
            {
                return BadRequest(new { error = "Question and ProductId are required." });
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product == null)
            {
                return NotFound(new { error = "Product not found." });
            }

            try
            {
                await _context.Entry(product)
                    .Collection(p => p.FAQs!)
                    .LoadAsync();
            }
            catch (MySqlConnector.MySqlException)
            {
                product.FAQs = [];
            }

            string answer;

            // Construiește contextul pentru serviciul AI
            var context = BuildContext(product);

            // Încearcă să folosească serviciul AI extern
            try
            {
                answer = await _aiService.GetAnswerAsync(request.Question, context);
                
                // Verifică dacă răspunsul este un mesaj de eroare și folosește fallback
                if (answer.Contains("nu este disponibil") || 
                    answer.Contains("nu este configurat") ||
                    answer.Contains("Quota") ||
                    answer.Contains("quota") ||
                    answer.Contains("Rate Limit") ||
                    answer.Contains("temporar indisponibil"))
                {
                    _logger.LogInformation("Serviciul AI nu este disponibil sau quota depășită, folosind logica locală");
                    var normalizedQuestion = NormalizeText(request.Question);
                    answer = FindAnswer(product, normalizedQuestion);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Eroare la apelul serviciului AI, folosind logica locală");
                // Fallback la logica locală în caz de eroare
                var normalizedQuestion = NormalizeText(request.Question);
                answer = FindAnswer(product, normalizedQuestion);
            }

            return Ok(new { answer });
        }

        private string BuildContext(Models.Product product)
        {
            var context = new System.Text.StringBuilder();
            context.AppendLine($"Produs: {product.Title}");
            context.AppendLine($"Descriere: {product.Description}");
            context.AppendLine($"Preț: {product.Price} RON");
            context.AppendLine($"Stoc: {product.Stock} bucăți");
            
            if (product.Category != null)
            {
                context.AppendLine($"Categorie: {product.Category.Type}");
            }

            if (product.FAQs != null && product.FAQs.Any())
            {
                context.AppendLine("\nÎntrebări frecvente:");
                foreach (var faq in product.FAQs.Take(5))
                {
                    context.AppendLine($"Q: {faq.Question}");
                    context.AppendLine($"A: {faq.Answer}");
                }
            }

            return context.ToString();
        }

        private static string FindAnswer(Models.Product product, string normalizedQuestion)
        {
            // cauta în FAQ-uri existente
            if (product.FAQs != null && product.FAQs.Count > 0)
            {
                foreach (var faq in product.FAQs)
                {
                    var normalizedFaqQuestion = NormalizeText(faq.Question);
 
                    if (normalizedFaqQuestion.Contains(normalizedQuestion) || 
                        normalizedQuestion.Contains(normalizedFaqQuestion) ||
                        CalculateSimilarity(normalizedQuestion, normalizedFaqQuestion) > 0.6)
                    {
                        return faq.Answer;
                    }
                }
            }

            var questionKeywords = ExtractKeywords(normalizedQuestion);

            if (ContainsKeywords(normalizedQuestion, WarrantyKeywords))
            {
                return "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație. Pentru detalii suplimentare, vă rugăm să ne contactați.";
            }

            if (ContainsKeywords(normalizedQuestion, ChildrenKeywords))
            {
                return "Acest produs este recomandat pentru adulți. Pentru produse potrivite copiilor, vă recomandăm să consultați categoria dedicată sau să ne contactați pentru recomandări specifice.";
            }

            if (ContainsKeywords(normalizedQuestion, SizeKeywords))
            {
                return "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru mărimi speciale sau consultanță personalizată, vă rugăm să ne contactați.";
            }

            if (ContainsKeywords(normalizedQuestion, RunningKeywords))
            {
                if (product.Category?.Type?.ToLower().Contains("running", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return "Da, acest produs este special proiectat pentru alergare și oferă suport excelent și amortizare pentru activități sportive.";
                }
                return "Acest produs este proiectat pentru uz zilnic și confort. Pentru alergare, recomandăm produsele din categoria Running Shoes care oferă suport specializat.";
            }

            if (ContainsKeywords(normalizedQuestion, CleaningKeywords))
            {
                return "Recomandăm curățarea cu o cârpă umedă și un detergent blând. Evitați mașina de spălat și uscarea la soare direct pentru a menține calitatea produsului.";
            }

            if (ContainsKeywords(normalizedQuestion, ReturnKeywords))
            {
                return "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Pentru detalii despre procesul de returnare, vă rugăm să ne contactați.";
            }

            if (ContainsKeywords(normalizedQuestion, MaterialKeywords))
            {
                var productDescription = product.Description ?? "";
                var materialInfo = ExtractMaterialInfo(productDescription, normalizedQuestion);
                if (!string.IsNullOrEmpty(materialInfo))
                {
                    return materialInfo;
                }
                
                var normalizedDesc = NormalizeText(productDescription);
                if (normalizedDesc.Contains("leather") || normalizedDesc.Contains("piele"))
                {
                    return "Acest produs este confecționat din piele naturală de înaltă calitate, ceea ce asigură durabilitate și confort pe termen lung.";
                }
                if (normalizedDesc.Contains("canvas") || normalizedDesc.Contains("textil"))
                {
                    return "Acest produs este confecționat din materiale textile de calitate, oferind confort și respirabilitate excelentă.";
                }
            }
            var productDescription2 = product.Description ?? "";
            var relevantInfo = ExtractRelevantInfo(productDescription2, normalizedQuestion, questionKeywords);
            if (!string.IsNullOrEmpty(relevantInfo))
            {
                if (relevantInfo.Length > 300)
                {
                    var truncated = relevantInfo[..300];
                    var lastPeriod = truncated.LastIndexOf('.');
                    if (lastPeriod > 200) 
                    {
                        return truncated[..(lastPeriod + 1)];
                    }
                    return truncated.TrimEnd() + "...";
                }
                return relevantInfo;
            }

            return "Momentan nu avem detalii specifice despre acest aspect. Vă recomandăm să ne contactați direct pentru informații suplimentare despre produs. Suntem aici să vă ajutăm!";
        }

        private static readonly Regex DiacriticsRegex = new(@"[ăâîșț]", RegexOptions.Compiled);
        private static readonly Regex NonAlphanumericRegex = new(@"[^a-z0-9\s]", RegexOptions.Compiled);
        private static readonly Regex SentenceSplitRegex = new(@"(?<=[.!?])\s+", RegexOptions.Compiled);

        private static string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            text = text.ToLower();
            text = DiacriticsRegex.Replace(text, m =>
            {
                var map = new Dictionary<string, string>
                {
                    { "ă", "a" }, { "â", "a" }, { "î", "i" },
                    { "ș", "s" }, { "ț", "t" }
                };
                return map.TryGetValue(m.Value, out var value) ? value : m.Value;
            });

            text = NonAlphanumericRegex.Replace(text, "");
            return text.Trim();
        }

        private static readonly string[] StopWords = ["este", "sunt", "are", "cum", "ce", "care", "pentru", "cu", "de", "la", "in", "pe", "si", "sau", "the", "is", "how", "what", "which", "for", "with", "of", "to", "on", "and", "or"];
        private static readonly string[] WarrantyKeywords = ["garanție", "garantie", "warranty", "garantia"];
        private static readonly string[] ChildrenKeywords = ["copii", "children", "child", "kid", "kids"];
        private static readonly string[] SizeKeywords = ["măsură", "masura", "size", "mărime", "marime", "sizes"];
        private static readonly string[] RunningKeywords = ["alergare", "running", "jogging", "sport"];
        private static readonly string[] CleaningKeywords = ["curățare", "curatare", "clean", "washing", "spălare", "spalare"];
        private static readonly string[] ReturnKeywords = ["returnare", "return", "schimb", "exchange"];
        private static readonly string[] MaterialKeywords = ["material", "materials", "leather", "piele", "canvas", "textil", "cauciuc", "rubber"];
        private static readonly char[] WordSeparators = [' ', '\t', '\n'];

        private static string[] ExtractKeywords(string text)
        {
            var words = text.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
            return Array.FindAll(words, w => !StopWords.Contains(w) && w.Length > 2);
        }

        private static bool ContainsKeywords(string text, string[] keywords)
        {
            return keywords.Any(keyword => text.Contains(keyword));
        }

        private static double CalculateSimilarity(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;

            var words1 = str1.Split(' ').Where(w => w.Length > 2).ToArray();
            var words2 = str2.Split(' ').Where(w => w.Length > 2).ToArray();

            var commonWords = words1.Intersect(words2).Count();
            var totalWords = Math.Max(words1.Length, words2.Length);

            return totalWords > 0 ? (double)commonWords / totalWords : 0;
        }

        private static string ExtractRelevantInfo(string originalDescription, string normalizedQuestion, string[] keywords)
        {
            if (string.IsNullOrEmpty(originalDescription) || keywords.Length == 0)
                return "";

            var sentences = SentenceSplitRegex.Split(originalDescription)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (sentences.Count == 0)
                return "";

            var expandedKeywords = ExpandKeywords(keywords);

            var scoredSentences = sentences.Select((sentence, index) =>
            {
                var normalizedSentence = NormalizeText(sentence);
                var originalSentence = sentence;

                double score = 0;
                
                foreach (var keyword in expandedKeywords)
                {
                    if (normalizedSentence.Contains(keyword))
                    {

                        var keywordWeight = keyword.Length > 4 ? 3.0 : (keyword.Length > 2 ? 2.0 : 1.0);
                        score += keywordWeight;

                        var count = Regex.Matches(normalizedSentence, Regex.Escape(keyword)).Count;
                        if (count > 1)
                            score += 0.5;
                    }
                }
 
                var questionWords = normalizedQuestion.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 3)
                    .ToList();
                
                var matchingQuestionWords = questionWords.Count(qw => normalizedSentence.Contains(qw));
                if (matchingQuestionWords > 0)
                {
                    score += matchingQuestionWords * 1.5; 
                }

                if (sentence.Length > 30 && sentence.Length < 200)
                    score += 0.5;
                
                return new { 
                    Sentence = originalSentence, 
                    Normalized = normalizedSentence,
                    Score = score,
                    Index = index
                };
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ToList();

            if (scoredSentences.Count == 0)
                return "";

            var topSentences = scoredSentences
                .Where(x => x.Score >= 1.0) 
                .Take(2) 
                .ToList();

            if (topSentences.Count == 0)
                return "";

            if (topSentences.Count > 1)
            {
                var indices = topSentences.Select(x => x.Index).OrderBy(i => i).ToList();

                if (indices[1] - indices[0] <= 2)
                {
                    var result = string.Join(" ", topSentences.OrderBy(x => x.Index).Select(x => x.Sentence)).Trim();
                    if (!result.EndsWith('.') && !result.EndsWith('!') && !result.EndsWith('?'))
                        result += ".";
                    return result;
                }
            }

            var bestSentence = topSentences.First().Sentence.Trim();
            if (!bestSentence.EndsWith('.') && !bestSentence.EndsWith('!') && !bestSentence.EndsWith('?'))
                bestSentence += ".";
                
            return bestSentence;
        }

        private static string[] ExpandKeywords(string[] keywords)
        {
            var expanded = new List<string>(keywords);

            // variatii si sinonime pentru cuvintele cheie comune
            var synonymMap = new Dictionary<string, string[]>
            {
                { "material", ["material", "materiale", "fabricat", "realizat", "confectionat"] },
                { "piele", ["piele", "leather", "piele naturala", "piele de vita"] },
                { "cauciuc", ["cauciuc", "rubber", "talpa", "talpa exterioara"] },
                { "curatare", ["curatare", "curat", "spalare", "intretinere", "clean", "washing"] },
                { "marime", ["marime", "masura", "size", "sizes", "disponibil"] },
                { "garantie", ["garantie", "garantia", "warranty", "garantie de"] },
                { "livrare", ["livrare", "livrat", "delivery", "shipping", "trimis"] },
                { "returnare", ["returnare", "return", "returnat", "schimb", "exchange"] },
                { "impermeabil", ["impermeabil", "waterproof", "rezistent la apa", "apa"] },
                { "iarna", ["iarna", "winter", "zapada", "frig", "rece"] },
                { "vara", ["vara", "summer", "cald", "caldura"] },
                { "alergare", ["alergare", "running", "jogging", "sport", "maraton"] },
                { "hiking", ["hiking", "trekking", "drumetie", "outdoor", "montan"] }
            };
            
            foreach (var keyword in keywords)
            {
                foreach (var kvp in synonymMap)
                {
                    if (kvp.Value.Any(v => v.Contains(keyword) || keyword.Contains(v)))
                    {
                        expanded.AddRange(kvp.Value);
                    }
                }
            }
            
            return [.. expanded.Distinct()];
        }

        private static string ExtractMaterialInfo(string originalDescription, string normalizedQuestion)
        {
            if (string.IsNullOrEmpty(originalDescription))
                return "";

            var materialKeywords = new[] { "piele", "leather", "canvas", "textil", "cauciuc", "rubber", "material", "materials", "eva", "sintetic", "synthetic", "premium", "calitate", "fabricat", "realizat", "confectionat" };
            
            return ExtractRelevantInfo(originalDescription, normalizedQuestion, materialKeywords);
        }
    }

    public class AskQuestionRequest
    {
        public int ProductId { get; set; }
        public string Question { get; set; } = string.Empty;
    }
}

