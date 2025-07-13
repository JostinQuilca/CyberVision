using ReconoceImagenWebApp; 
using Microsoft.Extensions.Configuration;  
using ReconoceImagenWebApp.Services;  

var builder = WebApplication.CreateBuilder(args);

 builder.Services.AddControllersWithViews();

 var visionSubscriptionKey = builder.Configuration["AzureVision:SubscriptionKey"];
var visionEndpoint = builder.Configuration["AzureVision:Endpoint"];
var visionFeatures = builder.Configuration["AzureVision:Features"];

 builder.Services.AddSingleton(new ConsumeACV(visionSubscriptionKey, visionEndpoint, visionFeatures));

 var translatorSubscriptionKey = builder.Configuration["AzureTranslator:SubscriptionKey"];
var translatorEndpoint = builder.Configuration["AzureTranslator:Endpoint"];
var translatorRegion = builder.Configuration["AzureTranslator:Region"]; 

 builder.Services.AddSingleton(new TranslatorService(translatorSubscriptionKey, translatorEndpoint, translatorRegion));
 

var app = builder.Build();

 if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
     app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();