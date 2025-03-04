using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Informatics.Appetite.ViewModels;

public partial class RecipeDetailsViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly IRecipeIngredientService _recipeIngredientService;

    public ObservableCollection<Ingredient> Ingredients { get; } = new();

    [ObservableProperty]
    private Recipe recipe;

    public RecipeDetailsViewModel(IRecipeService recipeService, IRecipeIngredientService recipeIngredientService)
    {
        _recipeService = recipeService;
        _recipeIngredientService = recipeIngredientService;
        SaveRecipeCommand = new AsyncRelayCommand(SaveRecipeAsync);
        DeleteRecipeCommand = new AsyncRelayCommand(DeleteRecipeAsync);
    }

    public IAsyncRelayCommand SaveRecipeCommand { get; }
    public IAsyncRelayCommand DeleteRecipeCommand { get; }

    public async Task LoadRecipeAsync(int recipeId)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (recipeId == -1)
            {
                // If recipeId is -1, create a new recipe
                Recipe = new Recipe();
            }
            else
            {
                // Otherwise, load existing recipe
                Recipe = await _recipeService.GetRecipeByIdAsync(recipeId) ?? new Recipe();
                // Load associated ingredients
                var ingredients = await _recipeIngredientService.GetIngredientsByRecipeIdAsync(recipeId);
                Ingredients.Clear();
                foreach (var ingredient in ingredients)
                {
                    Ingredients.Add(ingredient);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading recipe: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveRecipeAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _recipeService.SaveRecipeAsync(Recipe);
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving recipe: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteRecipeAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _recipeService.DeleteRecipeByIdAsync(Recipe.Id);
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting recipe: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}