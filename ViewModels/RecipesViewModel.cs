using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using Informatics.Appetite.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Informatics.Appetite.ViewModels;

public partial class RecipesViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;

    public ObservableCollection<Recipe> Recipes { get; }

    public RecipesViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        Recipes = new ObservableCollection<Recipe>();
        Title = "Recipes";

        RefreshCommand = new AsyncRelayCommand(LoadRecipesAsync);
        OpenRecipeDetailsCommand = new RelayCommand<int>(OpenRecipeDetails);
        OpenAddRecipeCommand = new RelayCommand(OpenAddRecipe);
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IRelayCommand<int> OpenRecipeDetailsCommand { get; }
    public IRelayCommand OpenAddRecipeCommand { get; }

    private async Task LoadRecipesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var recipes = await _recipeService.GetRecipesAsync();
            Recipes.Clear();
            foreach (var recipe in recipes)
            {
                Recipes.Add(recipe);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading recipes: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OpenAddRecipe() => OpenRecipeDetails(-1);

    private async void OpenRecipeDetails(int id)
    {
        await Shell.Current.GoToAsync($"{nameof(RecipeDetailsPage)}?recipeId={id}");
    }
}
