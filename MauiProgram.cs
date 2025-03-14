using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Informatics.Appetite.Contexts;
using Microsoft.EntityFrameworkCore;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Services;
using Informatics.Appetite.ViewModels;
using Informatics.Appetite.Pages;

namespace Informatics.Appetite;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add appsettings.json as configuration source.
        var assembly = Assembly.GetExecutingAssembly();
        string appsettingsFileName = "Informatics.Appetite.appsettings.json";
        using (var stream = assembly.GetManifestResourceStream(appsettingsFileName))
        {
            if (stream != null)
            {
                builder.Configuration.AddJsonStream(stream);
            }
        }

        // Register the RecipeContext with an in-memory database for testing.
        builder.Services.AddDbContext<RecipeContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("AppetiteDatabase");
            options.UseSqlServer(connectionString);
            //options.UseInMemoryDatabase("TestDatabase");        });
        });

        // Register services.
        builder.Services.AddScoped<IAppUserService, AppUserService>();
        builder.Services.AddScoped<IIngredientService, IngredientService>();
        builder.Services.AddScoped<IRecipeService, RecipeService>();
        builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
        builder.Services.AddScoped<IUserIngredientService, UserIngredientService>();

        // Register ViewModels.
        builder.Services.AddSingleton<IngredientsViewModel>();
        builder.Services.AddSingleton<RecipesViewModel>();
        builder.Services.AddScoped<SignInViewModel>();
        builder.Services.AddTransient<IngredientDetailsViewModel>();
        builder.Services.AddTransient<RecipeDetailsViewModel>();

        // Register Pages.
        builder.Services.AddSingleton<AppShell>();  // Register AppShell here
        builder.Services.AddSingleton<SignInPage>();
        builder.Services.AddSingleton<IngredientsPage>();
        builder.Services.AddTransient<IngredientDetailsPage>();
        builder.Services.AddSingleton<RecipesPage>();
        builder.Services.AddTransient<RecipeDetailsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        // Assign the DI container to our static property.
        App.Container = app.Services;
        return app;
    }
}
