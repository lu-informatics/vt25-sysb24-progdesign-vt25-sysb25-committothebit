using Informatics.Appetite.Pages;
namespace Informatics.Appetite;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(RecipeDetailsPage), typeof(RecipeDetailsPage));
        Routing.RegisterRoute(nameof(IngredientDetailsPage), typeof(IngredientDetailsPage));
    }
}