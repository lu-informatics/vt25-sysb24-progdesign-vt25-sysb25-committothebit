using Informatics.Appetite.Pages;


namespace Informatics.Appetite;


public partial class MainPage : ContentPage
{
   public MainPage()
   {
       InitializeComponent();
   }


   private async void OnExploreRecipesClicked(object sender, EventArgs e)
   {
       await Shell.Current.GoToAsync(nameof(RecipesPage));


   }


   private async void OnMyIngredientsClicked(object sender, EventArgs e)
   {
       await Shell.Current.GoToAsync(nameof(IngredientsPage));
   }


   private async void OnMealPlanClicked(object sender, EventArgs e)
   {
       // Lägg till en måltidsplaneringssida när du implementerar den
       await DisplayAlert("Kommer snart!", "Funktionen för måltidsplanering är under utveckling.", "OK");
   }
}
