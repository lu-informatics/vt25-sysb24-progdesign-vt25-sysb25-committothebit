using Informatics.Appetite.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Informatics.Appetite.Pages;

[QueryProperty(nameof(IngredientId), "ingredientId")]
public partial class IngredientDetailsPage : ContentPage
{
    private readonly IngredientDetailsViewModel _viewModel;

    private int _ingredientId;
    public int IngredientId
    {
        get => _ingredientId;
        set
        {
            _ingredientId = value;
            LoadIngredientAsync(_ingredientId);
        }
    }

    public IngredientDetailsPage(IngredientDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_ingredientId > 0)
        {
            await LoadIngredientAsync(_ingredientId);
        }
    }

    private async Task LoadIngredientAsync(int ingredientId)
    {
        if (_viewModel != null)
        {
            await _viewModel.LoadIngredientAsync(ingredientId);
        }
    }
}