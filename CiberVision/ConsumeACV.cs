using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.IO; 
namespace ReconoceImagenWebApp
{
    public class ConsumeACV
    {
        private readonly string _subscriptionKey;
        private readonly string _computerVisionEndpoint;
        private readonly string _features;

        public ConsumeACV(string subscriptionKey, string computerVisionEndpoint, string features)
        {
            _subscriptionKey = subscriptionKey;
            _computerVisionEndpoint = computerVisionEndpoint;
            _features = features;
        }

        public async Task<ImageAnalisysResult> AnalyzeImageAsync(string imageUrl, string languageCode)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                var uri = $"{_computerVisionEndpoint}/computervision/imageanalysis:analyze?model-version=latest&language={languageCode}&api-version=2024-02-01&features={_features}";

                var ojbectContent = new { url = imageUrl };
                var jsonContent = JsonSerializer.Serialize(ojbectContent);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ImageAnalisysResult>(jsonResponse);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error: {response.ReasonPhrase}. Details: {errorContent}");
                }
            }
        }

        public async Task<ImageAnalisysResult> AnalyzeImageFromStreamAsync(Stream imageStream, string contentType, string languageCode)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                var uri = $"{_computerVisionEndpoint}/computervision/imageanalysis:analyze?model-version=latest&language={languageCode}&api-version=2024-02-01&features={_features}";

                var content = new StreamContent(imageStream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ImageAnalisysResult>(jsonResponse);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al analizar imagen desde stream: {response.ReasonPhrase}. Detalles: {errorContent}");
                }
            }
        }
    }
}