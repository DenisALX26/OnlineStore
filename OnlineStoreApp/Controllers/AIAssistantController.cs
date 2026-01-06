using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using System.Text.RegularExpressions;

namespace OnlineStoreApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIAssistantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AIAssistantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] AskQuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question) || request.ProductId <= 0)
            {
                return BadRequest(new { error = "Question and ProductId are required." });
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product != null)
            {
                try
                {
                    await _context.Entry(product)
                        .Collection(p => p.FAQs)
                        .LoadAsync();
                }
                catch (MySqlConnector.MySqlException)
                {
                    product.FAQs = new List<Models.FAQ>();
                }
            }

            if (product == null)
            {
                return NotFound(new { error = "Product not found." });
            }

            var normalizedQuestion = NormalizeText(request.Question);
            var answer = await FindAnswer(product, normalizedQuestion, request.Question);

            return Ok(new { answer = answer });
        }

        private async Task<string> FindAnswer(Models.Product product, string normalizedQuestion, string originalQuestion)
        {
            // 1. Search in FAQs first
            if (product.FAQs != null && product.FAQs.Any())
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

            var description = NormalizeText(product.Description ?? "");
            var questionKeywords = ExtractKeywords(normalizedQuestion);

            if (ContainsKeywords(normalizedQuestion, new[] { "garanție", "garantie", "warranty", "garantia" }))
            {
                return "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație. Pentru detalii suplimentare, vă rugăm să ne contactați.";
            }

            if (ContainsKeywords(normalizedQuestion, new[] { "copii", "children", "child", "kid", "kids" }))
            {
                return "Acest produs este recomandat pentru adulți. Pentru produse potrivite copiilor, vă recomandăm să consultați categoria dedicată sau să ne contactați pentru recomandări specifice.";
            }

            if (ContainsKeywords(normalizedQuestion, new[] { "măsură", "masura", "size", "mărime", "marime", "sizes" }))
            {
                return "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru mărimi speciale sau consultanță personalizată, vă rugăm să ne contactați.";
            }

            if (ContainsKeywords(normalizedQuestion, new[] { "alergare", "running", "jogging", "sport" }))
            {
                if (product.Category?.Type?.ToLower().Contains("running") == true)
                {
                    return "Da, acest produs este special proiectat pentru alergare și oferă suport excelent și amortizare pentru activități sportive.";
                }
                return "Acest produs este proiectat pentru uz zilnic și confort. Pentru alergare, recomandăm produsele din categoria Running Shoes care oferă suport specializat.";
            }

            if (ContainsKeywords(normalizedQuestion, new[] { "curățare", "curatare", "clean", "washing", "spălare", "spalare" }))
            {
                return "Recomandăm curățarea cu o cârpă umedă și un detergent blând. Evitați mașina de spălat și uscarea la soare direct pentru a menține calitatea produsului.";
            }

            if (ContainsKeywords(normalizedQuestion, new[] { "returnare", "return", "schimb", "exchange" }))
            {
                return "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Pentru detalii despre procesul de returnare, vă rugăm să ne contactați.";
            }

            if (ContainsKeywords(normalizedQuestion, new[] { "material", "materials", "leather", "piele", "canvas", "textil" }))
            {
                if (description.Contains("leather") || description.Contains("piele"))
                {
                    return "Acest produs este confecționat din piele naturală de înaltă calitate, ceea ce asigură durabilitate și confort pe termen lung.";
                }
                if (description.Contains("canvas") || description.Contains("textil"))
                {
                    return "Acest produs este confecționat din materiale textile de calitate, oferind confort și respirabilitate excelentă.";
                }
            }

            var relevantInfo = ExtractRelevantInfo(description, questionKeywords);
            if (!string.IsNullOrEmpty(relevantInfo))
            {
                return relevantInfo;
            }

            return "Momentan nu avem detalii specifice despre acest aspect. Vă recomandăm să ne contactați direct pentru informații suplimentare despre produs. Suntem aici să vă ajutăm!";
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            text = text.ToLower();
            text = Regex.Replace(text, @"[ăâîșț]", m =>
            {
                var map = new Dictionary<string, string>
                {
                    { "ă", "a" }, { "â", "a" }, { "î", "i" },
                    { "ș", "s" }, { "ț", "t" }
                };
                return map.ContainsKey(m.Value) ? map[m.Value] : m.Value;
            });

            text = Regex.Replace(text, @"[^a-z0-9\s]", "");
            return text.Trim();
        }

        private string[] ExtractKeywords(string text)
        {
            var stopWords = new[] { "este", "sunt", "are", "cum", "ce", "care", "pentru", "cu", "de", "la", "in", "pe", "si", "sau", "the", "is", "are", "how", "what", "which", "for", "with", "of", "to", "in", "on", "and", "or" };
            var words = text.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Where(w => !stopWords.Contains(w) && w.Length > 2).ToArray();
        }

        private bool ContainsKeywords(string text, string[] keywords)
        {
            return keywords.Any(keyword => text.Contains(keyword));
        }

        private double CalculateSimilarity(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;

            var words1 = str1.Split(' ').Where(w => w.Length > 2).ToArray();
            var words2 = str2.Split(' ').Where(w => w.Length > 2).ToArray();

            var commonWords = words1.Intersect(words2).Count();
            var totalWords = Math.Max(words1.Length, words2.Length);

            return totalWords > 0 ? (double)commonWords / totalWords : 0;
        }

        private string ExtractRelevantInfo(string description, string[] keywords)
        {
            if (string.IsNullOrEmpty(description) || keywords.Length == 0)
                return "";

            var sentences = description.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var relevantSentences = sentences.Where(s =>
            {
                var normalizedSentence = NormalizeText(s);
                return keywords.Any(k => normalizedSentence.Contains(k));
            }).Take(2).ToList();

            if (relevantSentences.Any())
            {
                return string.Join(" ", relevantSentences).Trim() + ".";
            }

            return "";
        }
    }

    public class AskQuestionRequest
    {
        public int ProductId { get; set; }
        public string Question { get; set; } = string.Empty;
    }
}

