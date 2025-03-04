using System;
using Informatics.Appetite.Contexts;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using Microsoft.EntityFrameworkCore;

namespace Informatics.Appetite.Services;

public class RecipeIngredientService : IRecipeIngredientService
{
    private readonly RecipeContext _context;

    public RecipeIngredientService(RecipeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<RecipeIngredient>> GetRecipeIngredientsAsync()
    {
        return await _context.RecipeIngredients
            .AsNoTracking()
            .Include(ri => ri.Recipe)
            .Include(ri => ri.Ingredient)
            .ToListAsync();
    }

public async Task<IEnumerable<Ingredient>> GetIngredientsByRecipeIdAsync(int recipeId)
{
    return await _context.RecipeIngredients
        .AsNoTracking()
        .Include(ri => ri.Ingredient)
        .Where(ri => ri.RecipeId == recipeId)
        .Select(ri => ri.Ingredient)
        .OfType<Ingredient>() // Filter out null values and cast to Ingredient
        .ToListAsync();
}

public async Task<IEnumerable<Recipe>> GetRecipesByIngredientIdAsync(int ingredientId)
{
    return await _context.RecipeIngredients
        .AsNoTracking()
        .Include(ri => ri.Recipe)
        .Where(ri => ri.IngredientId == ingredientId)
        .Select(ri => ri.Recipe)
        .OfType<Recipe>() // Filter out null values and cast to Recipe
        .ToListAsync();
}

    public async Task<RecipeIngredient?> GetRecipeIngredientAsync(int recipeId, int ingredientId)
    {
        return await _context.RecipeIngredients
            .AsNoTracking()
            .Include(ri => ri.Recipe)
            .Include(ri => ri.Ingredient)
            .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId);
    }

    public async Task<RecipeIngredient> SaveRecipeIngredientAsync(RecipeIngredient recipeIngredient)
    {
        if (recipeIngredient.RecipeId == 0 || recipeIngredient.IngredientId == 0)
        {
            throw new ArgumentException("RecipeId and IngredientId must be set.");
        }

        var existingRecipeIngredient = await GetRecipeIngredientAsync(recipeIngredient.RecipeId, recipeIngredient.IngredientId);
        if (existingRecipeIngredient == null)
        {
            _context.RecipeIngredients.Add(recipeIngredient);
        }
        else
        {
            _context.RecipeIngredients.Update(recipeIngredient);
        }

        await _context.SaveChangesAsync();
        return recipeIngredient;
    }

    public async Task<bool> DeleteRecipeIngredientAsync(int recipeId, int ingredientId)
    {
        var recipeIngredient = await GetRecipeIngredientAsync(recipeId, ingredientId);
        if (recipeIngredient == null)
        {
            return false;
        }

        _context.RecipeIngredients.Remove(recipeIngredient);
        await _context.SaveChangesAsync();
        return true;
    }
}
