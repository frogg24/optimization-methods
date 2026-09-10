using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ru-RU")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US", "ru-RU");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Принудительно устанавливаем en-US для чисел (точка как разделитель)
    options.RequestCultureProviders.Insert(0,
        new CustomRequestCultureProvider(async context =>
        {
            return new ProviderCultureResult("en-US", "ru-RU");
        }));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization();

app.UseAuthorization();

app.MapRazorPages();

app.Run();