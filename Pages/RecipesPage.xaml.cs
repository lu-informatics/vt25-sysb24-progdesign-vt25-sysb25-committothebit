using Informatics.Appetite.ViewModels;

namespace Informatics.Appetite.Pages;

public partial class RecipesPage : ContentPage
{
    public RecipesPage(RecipesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnDifficultyIconTapped(object sender, EventArgs e)
    {
        DifficultyPicker.Focus();
    }
     private void OnCookingTimeIconTapped(object sender, EventArgs e)
    {
       CookingTimePicker.Focus();
    }

    private void OnDietTagIconTapped(object sender, EventArgs e)
    {
        DietTagPicker.Focus();
    }

    private void OnCategoryIconTapped(object sender, EventArgs e)
    {
        CategoryPicker.Focus();
    }



    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is RecipesViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }


}