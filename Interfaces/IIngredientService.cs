using System;
using Informatics.Appetite.Models;

namespace Informatics.Appetite.Interfaces;

public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetIngredientsAsync();
    Task<Ingredient?> GetIngredientByIdAsync(int id);
    Task<Ingredient?> GetIngredientByNameAsync(string name);
    Task<Ingredient> SaveIngredientAsync(Ingredient ingredient);
    Task<bool> DeleteIngredientByIdAsync(int id);
    Task<bool> DeleteIngredientByNameAsync(string name);
}
