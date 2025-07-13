using System.Text;
using System.Text.Json; 
using System.Threading.Tasks; 

namespace ReconoceImagenWebApp.Services 
{
    public class TranslatorService
    {
        private readonly string _subscriptionKey;
        private readonly string _endpoint;
        private readonly string _region; 

        public TranslatorService(string subscriptionKey, string endpoint, string region)
        {
            _subscriptionKey = subscriptionKey;
            _endpoint = endpoint;
            _region = region;
        }

        public async Task<string> TranslateTextAsync(string textToTranslate, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(textToTranslate))
            {
                return "";
            }

            var route = $"/translate?api-version=3.0&to={targetLanguage}";

            var uri = string.IsNullOrEmpty(_region) ? _endpoint + route : _endpoint + route + "&from=en"; 

            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonSerializer.Serialize(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                if (!string.IsNullOrEmpty(_region))
                {
                    request.Headers.Add("Ocp-Apim-Subscription-Region", _region);
                }

                var response = await client.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(result))
                    {
                        if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                        {
                            var firstTranslation = doc.RootElement[0].GetProperty("translations")[0];
                            return firstTranslation.GetProperty("text").GetString();
                        }
                    }
                    return "Error de formato de traducción.";
                }
                else
                {
                    throw new Exception($"Error de traducción: {response.ReasonPhrase}. Detalles: {result}");
                }
            }
        }
    }
}