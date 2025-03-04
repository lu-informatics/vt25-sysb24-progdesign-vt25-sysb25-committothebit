using System;
using Informatics.Appetite.Models;

namespace Informatics.Appetite.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<Recipe>> GetRecipesAsync();
    Task<Recipe?> GetRecipeByIdAsync(int id);
    Task<Recipe?> GetRecipeByNameAsync(string name);
    Task<Recipe> SaveRecipeAsync(Recipe recipe);
    Task<bool> DeleteRecipeByIdAsync(int id);
    Task<bool> DeleteRecipeByNameAsync(string name);
}
