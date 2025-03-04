using System.Collections.Generic;
using System.Threading.Tasks;
using Informatics.Appetite.Models;

namespace Informatics.Appetite.Interfaces;

public interface IRecipeIngredientService
{
    Task<IEnumerable<RecipeIngredient>> GetRecipeIngredientsAsync();
    Task<IEnumerable<Ingredient>> GetIngredientsByRecipeIdAsync(int recipeId);
    Task<IEnumerable<Recipe>> GetRecipesByIngredientIdAsync(int ingredientId);
    Task<RecipeIngredient?> GetRecipeIngredientAsync(int recipeId, int ingredientId);
    Task<RecipeIngredient> SaveRecipeIngredientAsync(RecipeIngredient recipeIngredient);
    Task<bool> DeleteRecipeIngredientAsync(int recipeId, int ingredientId);
}