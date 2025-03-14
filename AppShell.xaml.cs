using Informatics.Appetite.Pages;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Informatics.Appetite;

public partial class AppShell : Shell
{
    private const bool DeveloperModeEnabled = false; // Set to true to skip login

    public AppShell()
    {
        InitializeComponent();

        // Register routes.
        Routing.RegisterRoute(nameof(SignInPage), typeof(SignInPage));
        Routing.RegisterRoute(nameof(RecipeDetailsPage), typeof(RecipeDetailsPage));
        Routing.RegisterRoute(nameof(IngredientDetailsPage), typeof(IngredientDetailsPage));

        // Initially hide the TabBar.
        Shell.SetTabBarIsVisible(this, false);

        // Check authentication.
        CheckAuthentication();
    }

    private async void CheckAuthentication()
    {
        await Task.Delay(100); // Ensures Shell is fully loaded before navigation

        if (!DeveloperModeEnabled && !Preferences.Get("IsLoggedIn", false))
        {
            // Resolve the SignInPage from the DI container.
            var signInPage = App.Container.GetRequiredService<SignInPage>();
            await this.Navigation.PushModalAsync(new NavigationPage(signInPage));
        }
    }

}
