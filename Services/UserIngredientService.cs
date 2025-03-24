using Informatics.Appetite.Contexts;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Informatics.Appetite.Services;

public class UserIngredientService : IUserIngredientService
{
    private readonly RecipeContext _context;

    public UserIngredientService(RecipeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<UserIngredient>> GetUserIngredientsAsync()
    {
        return await _context.UserIngredients
            .AsNoTracking()
            .Include(ui => ui.AppUser)
            .Include(ui => ui.Ingredient)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserIngredient>> GetUserIngredientsByUserIdAsync(int userId)
    {
        try
        {
            return await _context.UserIngredients
                .Include(ui => ui.Ingredient)  // Include the Ingredient navigation property
                .Where(ui => ui.AppUserId == userId)
                .AsNoTracking()  // Add this to prevent tracking issues
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserIngredientsByUserIdAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<AppUser>> GetUsersByIngredientIdAsync(int ingredientId)
    {
        return await _context.UserIngredients
            .AsNoTracking()
            .Include(ui => ui.AppUser)
            .Where(ui => ui.IngredientId == ingredientId)
            .Select(ui => ui.AppUser)
            .OfType<AppUser>() // Filter out null values and cast to AppUser
            .ToListAsync();
    }

    public async Task<UserIngredient?> GetUserIngredientAsync(int userId, int ingredientId)
    {
        Debug.WriteLine($"GetUserIngredientAsync: Looking for User={userId}, Ingredient={ingredientId}");
        
        var result = await _context.UserIngredients
            .AsNoTracking()
            .Include(ui => ui.AppUser)
            .Include(ui => ui.Ingredient)
            .FirstOrDefaultAsync(ui => ui.AppUserId == userId && ui.IngredientId == ingredientId);
        
        Debug.WriteLine($"GetUserIngredientAsync: Found? {result != null}");
        
        return result;
    }

    public async Task<UserIngredient> SaveUserIngredientAsync(UserIngredient userIngredient)
    {
        Debug.WriteLine($"SaveUserIngredientAsync: Starting with User={userIngredient.AppUserId}, Ingredient={userIngredient.IngredientId}");
        try
        {
            // Clear any tracked entity first
            var localEntity = _context.UserIngredients.Local
                .FirstOrDefault(ui => ui.AppUserId == userIngredient.AppUserId && ui.IngredientId == userIngredient.IngredientId);
            if (localEntity != null)
            {
                _context.Entry(localEntity).State = EntityState.Detached;
            }

            // Check if entity exists in database
            var existingUserIngredient = await _context.UserIngredients
                .AsNoTracking()
                .FirstOrDefaultAsync(ui => ui.AppUserId == userIngredient.AppUserId && ui.IngredientId == userIngredient.IngredientId);

            if (existingUserIngredient != null)
            {
                Debug.WriteLine($"SaveUserIngredientAsync: Updating existing entry");
                _context.UserIngredients.Attach(userIngredient);
                _context.Entry(userIngredient).State = EntityState.Modified;
            }
            else
            {
                Debug.WriteLine($"SaveUserIngredientAsync: Adding new entry");
                _context.UserIngredients.Add(userIngredient);
            }

            await _context.SaveChangesAsync();
            Debug.WriteLine($"SaveUserIngredientAsync: Changes saved to database");

            return await GetUserIngredientAsync(userIngredient.AppUserId, userIngredient.IngredientId);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SaveUserIngredientAsync ERROR: {ex.Message}");
            throw;
        }
    }


    public async Task<bool> DeleteUserIngredientAsync(int userId, int ingredientId)
    {
        Debug.WriteLine($"DeleteUserIngredientAsync: Starting with User={userId}, Ingredient={ingredientId}");

        // First, check if an entity with the same key is already tracked locally.
        var trackedUserIngredient = _context.UserIngredients.Local
            .FirstOrDefault(ui => ui.AppUserId == userId && ui.IngredientId == ingredientId);

        UserIngredient userIngredient;
        if (trackedUserIngredient != null)
        {
            userIngredient = trackedUserIngredient;
            Debug.WriteLine("DeleteUserIngredientAsync: Found tracked entity.");
        }
        else
        {
            // Not in local tracker, so retrieve it from the database.
            userIngredient = await _context.UserIngredients
                .AsNoTracking()
                .FirstOrDefaultAsync(ui => ui.AppUserId == userId && ui.IngredientId == ingredientId);
            if (userIngredient == null)
            {
                Debug.WriteLine("DeleteUserIngredientAsync: Nothing to delete, returning false.");
                return false;
            }
            // Attach the untracked entity so it becomes tracked.
            _context.UserIngredients.Attach(userIngredient);
            Debug.WriteLine("DeleteUserIngredientAsync: Attached untracked entity.");
        }

        // Remove the entity.
        _context.UserIngredients.Remove(userIngredient);
        Debug.WriteLine("DeleteUserIngredientAsync: Removing from context.");

        await _context.SaveChangesAsync();
        Debug.WriteLine("DeleteUserIngredientAsync: Changes saved to database.");

        return true;
    }

}
