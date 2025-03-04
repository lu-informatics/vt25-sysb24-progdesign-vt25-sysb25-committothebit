using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Informatics.Appetite.ViewModels;

public partial class IngredientDetailsViewModel : BaseViewModel
{
    private readonly IIngredientService _ingredientService;
    private readonly IRecipeIngredientService _recipeIngredientService;

    public ObservableCollection<Recipe> Recipes { get; } = new();
    public ObservableCollection<string> Units { get; } = new() { "g", "kg", "ml", "cl", "dl", "pieces" };
    public ObservableCollection<string> DietTags { get; } = new() { "vegan", "vegetarian", "pescatarian", "omnivore" };

    [ObservableProperty]
    private Ingredient ingredient;

    public IngredientDetailsViewModel(IIngredientService ingredientService, IRecipeIngredientService recipeIngredientService)
    {
        _ingredientService = ingredientService;
        _recipeIngredientService = recipeIngredientService;
        SaveIngredientCommand = new AsyncRelayCommand(SaveIngredientAsync);
        DeleteIngredientCommand = new AsyncRelayCommand(DeleteIngredientAsync);

        // Initialize the ingredient field
        ingredient = new Ingredient();
    }

    public IAsyncRelayCommand SaveIngredientCommand { get; }
    public IAsyncRelayCommand DeleteIngredientCommand { get; }

    public async Task LoadIngredientAsync(int ingredientId)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (ingredientId == -1)
            {
                // If ingredientId is -1, create a new ingredient
                Ingredient = new Ingredient();
            }
            else
            {
                // Otherwise, load existing ingredient
                Ingredient = await _ingredientService.GetIngredientByIdAsync(ingredientId) ?? new Ingredient();
                // Load associated recipes
                var recipes = await _recipeIngredientService.GetRecipesByIngredientIdAsync(ingredientId);
                Recipes.Clear();
                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ingredient: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveIngredientAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _ingredientService.SaveIngredientAsync(Ingredient);
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving ingredient: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteIngredientAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _ingredientService.DeleteIngredientByIdAsync(Ingredient.Id);
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting ingredient: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}