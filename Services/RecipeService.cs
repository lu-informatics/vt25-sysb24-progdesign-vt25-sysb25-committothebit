using System;
using Informatics.Appetite.Models;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Informatics.Appetite.Services;

public class RecipeService : IRecipeService
{
    private readonly RecipeContext _context;

    public RecipeService(RecipeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Recipe>> GetRecipesAsync()
    {
        return await _context.Recipes
            .AsNoTracking()
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .ToListAsync();
    }

    public async Task<Recipe?> GetRecipeByIdAsync(int id)
    {
        return await _context.Recipes
            .AsNoTracking()
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe?> GetRecipeByNameAsync(string name)
    {
        return await _context.Recipes
            .AsNoTracking()
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Recipe> SaveRecipeAsync(Recipe recipe)
    {
        if (recipe.Id == 0)
        {
            _context.Recipes.Add(recipe);
        }
        else
        {
            _context.Recipes.Update(recipe);
        }

        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<bool> DeleteRecipeByIdAsync(int id)
    {
        var recipe = await GetRecipeByIdAsync(id);
        if (recipe == null)
        {
            return false;
        }

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRecipeByNameAsync(string name)
    {
        var recipe = await GetRecipeByNameAsync(name);
        if (recipe == null)
        {
            return false;
        }

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return true;
    }


}
