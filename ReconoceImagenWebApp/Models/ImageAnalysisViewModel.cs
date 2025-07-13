using Microsoft.AspNetCore.Http; 
using System.ComponentModel.DataAnnotations; 

namespace ReconoceImagenWebApp.Models 
{
    public class ImageAnalysisViewModel
    {
        [Url(ErrorMessage = "Por favor, ingrese una URL válida.")]
        public string? ImageUrl { get; set; }

        public IFormFile? UploadedImageFile { get; set; } 

        public string? DescriptionSpanish { get; set; }
        public string? DescriptionSecondLanguage { get; set; }
        public string SelectedSecondLanguageCode { get; set; }
        // Puedes personalizar esta lista de idiomas si quieres
        public List<string> AvailableLanguages { get; set; } = new List<string> { "es", "en", "fr", "de", "it", "zh-Hans" };

        public string? DisplayImageUrl { get; set; } 
    }
}