using Informatics.Appetite.ViewModels;
using Microsoft.Maui.Controls;

namespace Informatics.Appetite.Pages;

public partial class RecipesPage : ContentPage
{
    public RecipesPage(RecipesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var viewModel = BindingContext as RecipesViewModel;
        viewModel?.RefreshCommand.Execute(null);
    }
}