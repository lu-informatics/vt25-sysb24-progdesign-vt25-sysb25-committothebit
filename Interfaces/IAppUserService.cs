using Informatics.Appetite.Models;

namespace Informatics.Appetite.Interfaces;

public interface IAppUserService
{
    Task<AppUser?> AuthenticateUserAsync(string username, string password);
    Task<AppUser?> CreateUserAsync(string username, string password);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string name);
    Task<AppUser> SaveUserAsync(AppUser appUser);
    Task<bool> DeleteUserByIdAsync(int id);
    Task<bool> DeleteUserByUsernameAsync(string name);
}
