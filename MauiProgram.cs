using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Informatics.Appetite.Contexts;
using Microsoft.EntityFrameworkCore;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Services;
using Informatics.Appetite.ViewModels;
using Informatics.Appetite.Pages;
using Informatics.Appetite;

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

		//Add appsettings.json as configuration source
		var assembly = Assembly.GetExecutingAssembly();
		string appsettingsFileName = "Informatics.Appetite.appsettings.json";
		using (var stream = assembly.GetManifestResourceStream(appsettingsFileName))
		{
			if (stream != null)
			{
				builder.Configuration.AddJsonStream(stream);
			}
		}

		//Add database context to the service collection
		builder.Services.AddDbContext<RecipeContext>(options =>
		{
			var connectionString =
				builder.Configuration.GetConnectionString("AppetiteDatabase");
			options.UseSqlServer(connectionString);
		});

		builder.Services.AddScoped<IIngredientService, IngredientService>();
		builder.Services.AddScoped<IRecipeService, RecipeService>();
		builder.Services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();

		//Use Singleton for ViewModels that manage ToListAsync
		builder.Services.AddSingleton<IngredientsViewModel>();
		builder.Services.AddSingleton<RecipesViewModel>();

		//Use Transient that manage single detail Pages
		builder.Services.AddTransient<IngredientDetailsViewModel>();
		builder.Services.AddTransient<RecipeDetailsViewModel>();

		//Register the pages
		builder.Services.AddSingleton<IngredientsPage>();
		builder.Services.AddTransient<IngredientDetailsPage>();
		builder.Services.AddSingleton<RecipesPage>();
		builder.Services.AddTransient<RecipeDetailsPage>();



#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
