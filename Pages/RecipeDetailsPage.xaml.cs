using Informatics.Appetite.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Informatics.Appetite.Pages;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeDetailsPage : ContentPage
{
    private readonly RecipeDetailsViewModel _viewModel;

    private int _recipeId;
    public int RecipeId
    {
        get => _recipeId;
        set
        {
            _recipeId = value;
            LoadRecipeAsync(_recipeId);
        }
    }

    public RecipeDetailsPage(RecipeDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_recipeId > 0)
        {
            await LoadRecipeAsync(_recipeId);
        }
    }

    private async Task LoadRecipeAsync(int recipeId)
    {
        if (_viewModel != null)
        {
            await _viewModel.LoadRecipeAsync(recipeId);
        }
    }
}