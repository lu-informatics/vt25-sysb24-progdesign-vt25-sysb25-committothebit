
using Informatics.Appetite.Pages;
namespace Informatics.Appetite;


public partial class AppShell : Shell
{
   public AppShell()
   {
       InitializeComponent();
       Routing.RegisterRoute(nameof(RecipesPage), typeof(RecipesPage));
       Routing.RegisterRoute(nameof(IngredientsPage), typeof(IngredientsPage));
       Routing.RegisterRoute(nameof(RecipeDetailsPage), typeof(RecipeDetailsPage));
       Routing.RegisterRoute(nameof(IngredientDetailsPage), typeof(IngredientDetailsPage));
   }


private async void OnExploreRecipesClicked(object sender, EventArgs e)
{
   await Shell.Current.GoToAsync(nameof(RecipesPage));
}


private async void OnMyIngredientsClicked(object sender, EventArgs e)
{
   await Shell.Current.GoToAsync(nameof(IngredientsPage));
}


}
