using Informatics.Appetite.ViewModels;

namespace Informatics.Appetite.Pages;

public partial class IngredientsPage : ContentPage
{
	public IngredientsPage(IngredientsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var viewModel = BindingContext as IngredientsViewModel;
        viewModel?.RefreshCommand.Execute(null);
    }
}