using Informatics.Appetite.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Informatics.Appetite;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Use the DI container stored in the static Container property.
        //MainPage = new NavigationPage(Container.GetRequiredService<SignInPage>());
    }

    // Static property to hold the DI container.
    public static IServiceProvider Container { get; set; } = null!;

    protected override Window CreateWindow(IActivationState? activationState)
    {
        //return new Window(new NavigationPage(Container.GetRequiredService<SignInPage>()));
        return new Window(App.Container.GetRequiredService<AppShell>());
    }
}
