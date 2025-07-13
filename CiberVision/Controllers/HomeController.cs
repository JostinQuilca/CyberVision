using Microsoft.AspNetCore.Mvc;
using ReconoceImagenWebApp.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using ReconoceImagenWebApp.Services;
using System.IO;
using System;

namespace ReconoceImagenWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConsumeACV _consumeACV;
        private readonly TranslatorService _translatorService;

        public HomeController(ConsumeACV consumeACV, TranslatorService translatorService)
        {
            _consumeACV = consumeACV;
            _translatorService = translatorService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ImageAnalysisViewModel();
            model.SelectedSecondLanguageCode = "fr";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeImage(ImageAnalysisViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ImageUrl) && model.UploadedImageFile == null)
            {
                ModelState.AddModelError("", "Por favor, ingrese una URL o suba una imagen.");
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl) && model.UploadedImageFile != null)
            {
                ModelState.AddModelError("", "Por favor, ingrese una URL o suba una imagen, no ambos.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ImageAnalisysResult resultEnglish;
                    string englishCaption = "No se pudo obtener descripción en inglés.";

                    if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                    {
                        resultEnglish = await _consumeACV.AnalyzeImageAsync(model.ImageUrl, "en");
                        englishCaption = resultEnglish.captionResult?.text ?? englishCaption;
                        model.DisplayImageUrl = model.ImageUrl;
                    }
                    else if (model.UploadedImageFile != null)
                    {
                        if (!model.UploadedImageFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("UploadedImageFile", "Solo se permiten archivos de imagen.");
                            return View("Index", model);
                        }

                        using (var stream = model.UploadedImageFile.OpenReadStream())
                        {
                            resultEnglish = await _consumeACV.AnalyzeImageFromStreamAsync(stream, model.UploadedImageFile.ContentType, "en");
                            englishCaption = resultEnglish.captionResult?.text ?? englishCaption;

                            stream.Seek(0, SeekOrigin.Begin);
                            using (var memoryStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memoryStream);
                                model.DisplayImageUrl = $"data:{model.UploadedImageFile.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
                            }
                        }
                    }
                    else
                    {
                        return View("Index", model);
                    }

                    model.DescriptionSpanish = await _translatorService.TranslateTextAsync(englishCaption, "es");

                    if (!string.IsNullOrWhiteSpace(model.SelectedSecondLanguageCode) && model.AvailableLanguages.Contains(model.SelectedSecondLanguageCode))
                    {
                        model.DescriptionSecondLanguage = await _translatorService.TranslateTextAsync(englishCaption, model.SelectedSecondLanguageCode);
                    }
                    else
                    {
                        model.DescriptionSecondLanguage = "Idioma no válido o no seleccionado.";
                    }

                    return View("Index", model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al analizar o traducir la imagen: {ex.Message}");
                }
            }
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}