using Informatics.Appetite.ViewModels;

namespace Informatics.Appetite.Pages;

public partial class SignInPage : ContentPage
{
    // Constructor now uses DI to inject the SignInViewModel.
    public SignInPage(SignInViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Prevent hardware back button from dismissing the page
    protected override bool OnBackButtonPressed() => true;
}
