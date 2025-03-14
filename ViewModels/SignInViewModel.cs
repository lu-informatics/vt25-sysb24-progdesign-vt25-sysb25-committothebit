using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Pages;

namespace Informatics.Appetite.ViewModels
{
    public partial class SignInViewModel : BaseViewModel
    {
        private readonly IAppUserService _appUserService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool isErrorVisible;

        [ObservableProperty]
        private bool isCreateAccount;

        public SignInViewModel(IAppUserService appUserService)
        {
            _appUserService = appUserService;
            SignInCommand = new AsyncRelayCommand(SignInAsync);
        }

        public IAsyncRelayCommand SignInCommand { get; }

        // Computed property for the button text
        public string SignInButtonText => IsCreateAccount ? "Create Account" : "Sign In";

        // This method is automatically called when IsCreateAccount changes,
        // ensuring the UI updates the SignInButtonText binding.
        partial void OnIsCreateAccountChanged(bool oldValue, bool newValue)
        {
            OnPropertyChanged(nameof(SignInButtonText));
        }

        private async Task SignInAsync()
        {
            IsErrorVisible = false;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required.";
                IsErrorVisible = true;
                return;
            }

            if (IsCreateAccount)
            {
                var createdUser = await _appUserService.CreateUserAsync(Username, Password);
                if (createdUser != null)
                {
                    // Dismiss the sign-in modal.
                    await Shell.Current.Navigation.PopModalAsync();
                    // Show the TabBar.
                    //Shell.SetTabBarIsVisible(Shell.Current, true);
                }
                else
                {
                    ErrorMessage = "Account creation failed. Username may already be in use.";
                    IsErrorVisible = true;
                }
            }
            else
            {
                var user = await _appUserService.AuthenticateUserAsync(Username, Password);
                if (user != null)
                {
                    // Dismiss the sign-in modal.
                    await Shell.Current.Navigation.PopModalAsync();
                    // Show the TabBar.
                    //Shell.SetTabBarIsVisible(Shell.Current, true);
                    // Optionally, navigate to RecipesPage if needed.
                    // await Shell.Current.GoToAsync($"//RecipesPage");
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                    IsErrorVisible = true;
                }
            }
        }


    }
}
