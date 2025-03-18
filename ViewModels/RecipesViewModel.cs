using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using Informatics.Appetite.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace Informatics.Appetite.ViewModels;

public partial class RecipesViewModel : BaseViewModel
{
    private readonly IRecipeService _recipeService;
    private readonly IUserIngredientService _userIngredientService;
    private readonly IAppUserService _appUserService;
    private string _searchText;
    private IEnumerable<UserIngredient> _userIngredients;

    public ObservableCollection<Recipe> Recipes { get; }
    public ObservableCollection<Recipe> FilteredRecipes { get; }

    public ObservableCollection<string> Difficulties { get; } = new ObservableCollection<string>();
    public ObservableCollection<string> CookingTimes { get; } = new ObservableCollection<string>();

    public ObservableCollection<string> DietTags { get; } = new ObservableCollection<string>();

    public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>();

    private string _selectedDifficulty = "Difficulty";
    public string SelectedDifficulty
    {
        get => _selectedDifficulty;
        set
        {
            if (SetProperty(ref _selectedDifficulty, value))
            {
                FilterRecipes();
            }
        }
    }

    private string _selectedCookingTime = "Cooking time";
    public string SelectedCookingTime
    {
        get => _selectedCookingTime;
        set
        {
            if (SetProperty(ref _selectedCookingTime, value))
            {
                FilterRecipes();
            }
        }
    }
    private string _selectedCategory = "Category";
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                FilterRecipes();
            }
        }
    }

    private string _selectedDietTag = "Diet tag";
        public string SelectedDietTag
        {
            get => _selectedDietTag;
            set
            {
                if (SetProperty(ref _selectedDietTag, value))
                {
                    FilterRecipes();
                }
            }
}
    public IAsyncRelayCommand RefreshCommand { get; }
    public IRelayCommand<int> OpenRecipeDetailsCommand { get; }
    public IRelayCommand OpenAddRecipeCommand { get; }

    public RecipesViewModel(IRecipeService recipeService, IUserIngredientService userIngredientService, IAppUserService appUserService)
    {
        _recipeService = recipeService;
        _userIngredientService = userIngredientService;
        _appUserService = appUserService;
        Recipes = new ObservableCollection<Recipe>();
        FilteredRecipes = new ObservableCollection<Recipe>();
        Title = "Recipes";

        RefreshCommand = new AsyncRelayCommand(LoadRecipesAsync);
        OpenRecipeDetailsCommand = new RelayCommand<int>(OpenRecipeDetails);
        OpenAddRecipeCommand = new RelayCommand(OpenAddRecipe);
    }
    public async Task InitializeAsync()
    {
        await LoadDifficultyLevelsAsync();
        await LoadCookingTimesAsync();
        await LoadDietTagsAsync();
        await LoadCategoriesAsync();
        await LoadRecipesAsync();

        await LoadRecipesAsync();
    }

    public async Task LoadRecipesAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Debug.WriteLine("LoadRecipesAsync: Starting to load data");

            // Load user ingredients first
            Debug.WriteLine("LoadRecipesAsync: Getting current user");
            AppUser appUser = await _appUserService.GetCurrentUserAsync();
            int appUserId = appUser.Id;
            Debug.WriteLine($"LoadRecipesAsync: Got user with ID {appUserId}");
            
            Debug.WriteLine("LoadRecipesAsync: Getting user ingredients");
            _userIngredients = await _userIngredientService.GetUserIngredientsByUserIdAsync(appUserId);
            Debug.WriteLine($"LoadRecipesAsync: Got {_userIngredients?.Count() ?? 0} user ingredients");

            // Load the main recipes
            Debug.WriteLine("LoadRecipesAsync: Getting recipes");
            var recipes = await _recipeService.GetRecipesAsync();
            Debug.WriteLine($"LoadRecipesAsync: Got {recipes?.Count() ?? 0} recipes");
            
            Recipes.Clear();
            
            if (recipes != null)
            {
                foreach (var recipe in recipes)
                {
                    recipe.HasAllIngredients = RecipeHasAllIngredients(recipe);
                    Recipes.Add(recipe);
                }
                Debug.WriteLine($"LoadRecipesAsync: Added {Recipes.Count} recipes to collection");
            }
            else
            {
                Debug.WriteLine("ERROR: recipes is null!");
            }

            FilterRecipes();
            Debug.WriteLine($"LoadRecipesAsync: After filtering: {FilteredRecipes.Count} recipes");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR loading recipes: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        finally
        {
            IsBusy = false;
            Debug.WriteLine("LoadRecipesAsync: Completed");
        }

        //// Also load Difficulty & CookingTime pickers here, awaited
        //await LoadDifficultyLevelsAsync();
        //await LoadCookingTimesAsync();
        //await LoadDietTagsAsync();
        //await LoadCategoriesAsync();
    }

    private void FilterRecipes()
    {
        var filtered = Recipes.Where(r =>
        {
            // Existing filters: search, difficulty, cooking time, and diet
            bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) || r.Name.ToLower().Contains(SearchText.ToLower());
            bool matchesDifficulty = string.IsNullOrWhiteSpace(SelectedDifficulty) || SelectedDifficulty == "Difficulty" ||
                (r.DifficultyLevel != null && r.DifficultyLevel.Equals(SelectedDifficulty, StringComparison.OrdinalIgnoreCase));
            bool matchesCookingTime = string.IsNullOrWhiteSpace(SelectedCookingTime) || SelectedCookingTime == "Cooking time" ||
                r.CookingTime.ToString() == SelectedCookingTime;

            bool matchesDiet = true;
            if (!string.IsNullOrWhiteSpace(SelectedDietTag) && SelectedDietTag != "Diet tag")
            {
                var recipeDietTags = r.RecipeIngredients
                    .Select(ri => ri.Ingredient?.DietTag)
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .Distinct()
                    .ToList();

                if (SelectedDietTag.Equals("Non-Vegetarian", StringComparison.OrdinalIgnoreCase))
                {
                    matchesDiet = true;
                }
                else if (SelectedDietTag.Equals("Pescatarian", StringComparison.OrdinalIgnoreCase))
                {
                    matchesDiet = !recipeDietTags.Any(tag => tag.Equals("Non-Vegetarian", StringComparison.OrdinalIgnoreCase));
                }
                else if (SelectedDietTag.Equals("Vegetarian", StringComparison.OrdinalIgnoreCase))
                {
                    matchesDiet = !recipeDietTags.Any(tag =>
                        tag.Equals("Non-Vegetarian", StringComparison.OrdinalIgnoreCase) ||
                        tag.Equals("Pescatarian", StringComparison.OrdinalIgnoreCase));
                }
                else if (SelectedDietTag.Equals("Vegan", StringComparison.OrdinalIgnoreCase))
                {
                    matchesDiet = !recipeDietTags.Any(tag =>
                        tag.Equals("Non-Vegetarian", StringComparison.OrdinalIgnoreCase) ||
                        tag.Equals("Pescatarian", StringComparison.OrdinalIgnoreCase) ||
                        tag.Equals("Vegetarian", StringComparison.OrdinalIgnoreCase));
                }
            }

            // New filter: Category filtering
            bool matchesCategory = true;
            if (!string.IsNullOrWhiteSpace(SelectedCategory) && SelectedCategory != "Category")
            {
                // Check if any ingredient in the recipe has the selected category.
                matchesCategory = r.RecipeIngredients.Any(ri =>
                    ri.Ingredient != null &&
                    ri.Ingredient.Category.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            return matchesSearch && matchesDifficulty && matchesCookingTime && matchesDiet && matchesCategory;
        }).ToList();

        FilteredRecipes.Clear();
        foreach (var recipe in filtered)
        {
            FilteredRecipes.Add(recipe);
        }
    }

    private async Task LoadDifficultyLevelsAsync()
    {
        var difficultyLevels = await _recipeService.GetDifficultyLevelsAsync();
        Difficulties.Clear();
        Difficulties.Add("Difficulty");
        foreach (var level in difficultyLevels)
        {
            Difficulties.Add(level);
        }
        SelectedDifficulty = "Difficulty";
    }

    private async Task LoadCookingTimesAsync()
    {
        var cookingTimes = await _recipeService.GetCookingTimesAsync();
        CookingTimes.Clear();
        CookingTimes.Add("Cooking time");
        foreach (var time in cookingTimes)
        {
            CookingTimes.Add(time);
        }
        SelectedCookingTime = "Cooking time";
    }

    private async Task LoadDietTagsAsync()
    {
        var tags = await _recipeService.GetDietTagsAsync();
        DietTags.Clear();
        DietTags.Add("Diet Tag"); // Placeholder option
        foreach (var tag in tags)
        {
            DietTags.Add(tag);
        }
        SelectedDietTag = "Diet Tag";
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _recipeService.GetCategoriesAsync();
        Categories.Clear();
        Categories.Add("Category");  // Placeholder option
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
        SelectedCategory = "Category";
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterRecipes();
        }
    }

    private void OpenAddRecipe() => OpenRecipeDetails(-1);

    private async void OpenRecipeDetails(int id)
    {
        await Shell.Current.GoToAsync($"{nameof(RecipeDetailsPage)}?recipeId={id}");
    }

    public bool RecipeHasAllIngredients(Recipe recipe)
    {
        IEnumerable<RecipeIngredient> RecipeIngredients = recipe.RecipeIngredients;

        if (RecipeIngredients == null || !RecipeIngredients.Any())
            return false;

        foreach(RecipeIngredient recipeIngredient in RecipeIngredients)
        {
            bool hasIngredient = false;
            bool hasAmount = false;
            foreach(UserIngredient userIngredient in _userIngredients)
            {
                if (userIngredient.IngredientId == recipeIngredient.IngredientId)
                {
                    hasIngredient = true;
                    if (userIngredient.Amount >= recipeIngredient.Amount)
                    {
                        hasAmount = true;
                    }
                }
            }
            if (!hasIngredient || !hasAmount)
            {
                return false;
            }
        }
        return true;
    }
}
