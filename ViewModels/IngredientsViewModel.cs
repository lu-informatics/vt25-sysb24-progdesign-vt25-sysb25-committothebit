using System;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Informatics.Appetite.Pages;

namespace Informatics.Appetite.ViewModels;

public partial class IngredientsViewModel : BaseViewModel
{
    private readonly IIngredientService _ingredientService;
    public ObservableCollection<Ingredient> Ingredients { get; }
    public IngredientsViewModel(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
        Ingredients = new ObservableCollection<Ingredient>();
        Title = "Ingredients";

        RefreshCommand = new AsyncRelayCommand(LoadIngredientsAsync);
        OpenIngredientDetailsCommand = new RelayCommand<int>(OpenIngredientDetails);
        OpenAddIngredientCommand = new RelayCommand(OpenAddIngredient);
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IRelayCommand<int> OpenIngredientDetailsCommand { get; }
    public IRelayCommand OpenAddIngredientCommand { get; }

    private async Task LoadIngredientsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var ingredients = await _ingredientService.GetIngredientsAsync();
            Ingredients.Clear();
            foreach(var ingredient in ingredients)
            {
                Ingredients.Add(ingredient);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading ingredients: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OpenAddIngredient() => OpenIngredientDetails(-1);
    
    private async void OpenIngredientDetails(int id)
    {
        await Shell.Current.GoToAsync($"{nameof(IngredientDetailsPage)}?ingredientId={id}");
    }
}
