using System;
using Informatics.Appetite.Contexts;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using Microsoft.EntityFrameworkCore;

namespace Informatics.Appetite.Services;

public class IngredientService : IIngredientService
{
    private readonly RecipeContext _context;

    public IngredientService(RecipeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Ingredient>> GetIngredientsAsync()
    {
        return await _context.Ingredients
            .AsNoTracking()
            .Include(i => i.RecipeIngredients)
            .ToListAsync();
    }

    public async Task<Ingredient?> GetIngredientByIdAsync(int id)
    {
        return await _context.Ingredients
            .AsNoTracking()
            .Include(i => i.RecipeIngredients)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Ingredient?> GetIngredientByNameAsync(string name)
    {
        return await _context.Ingredients
            .AsNoTracking()
            .Include(i => i.RecipeIngredients)
            .FirstOrDefaultAsync(i => i.Name == name);
    }

    public async Task<Ingredient> SaveIngredientAsync(Ingredient ingredient)
    {
        if (ingredient.Id == 0)
        {
            _context.Ingredients.Add(ingredient);
        }
        else
        {
            _context.Ingredients.Update(ingredient);
        }

        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<bool> DeleteIngredientByIdAsync(int id)
    {
        var ingredient = await GetIngredientByIdAsync(id);
        if (ingredient == null)
        {
            return false;
        }

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteIngredientByNameAsync(string name)
    {
        var ingredient = await GetIngredientByNameAsync(name);
        if (ingredient == null)
        {
            return false;
        }

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
        return true;
    }
}
