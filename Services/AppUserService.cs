using System.Security.Cryptography;
using System.Text;
using Informatics.Appetite.Contexts;
using Informatics.Appetite.Interfaces;
using Informatics.Appetite.Models;
using Microsoft.EntityFrameworkCore;

namespace Informatics.Appetite.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly RecipeContext _context;
        private AppUser? _currentUser;
        private int _currentUserId;
        private bool _isAuthenticated;
        
        // Constants for PBKDF2
        private const int SaltSize = 32;      // Size in bytes
        private const int HashSize = 32;      // Size in bytes (256 bits)
        private const int Iterations = 10000;  // Number of iterations

        public AppUserService(RecipeContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            if (_currentUser == null)
            {
                // Just fetch the existing admin user - don't try to create it again
                var adminUser = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Username == "admin");
                
                if (adminUser != null)
                {
                    // Store the user in memory
                    _currentUser = adminUser;
                    _currentUserId = adminUser.Id;
                    _isAuthenticated = true;
                }
                else
                {
                    Console.WriteLine("Warning: Admin user not found in database. This should be created by AppShell.");
                    throw new InvalidOperationException("Admin user not found. Ensure it's created at startup.");
                }
            }
            
            return _currentUser;
        }

        public AppUser GetCurrentUser()
        {
            if (_currentUser == null)
            {
                throw new InvalidOperationException("Current user not initialized. Call GetCurrentUserAsync first.");
            }
            return _currentUser;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.AppUsers
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string name)
        {
            return await _context.AppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Username == name);
        }

        public async Task<AppUser> SaveUserAsync(AppUser user)
        {
            if (user.Id == 0)
            {
                _context.AppUsers.Add(user);
            }
            else
            {
                _context.AppUsers.Update(user);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserByUsernameAsync(string name)
        {
            var user = await GetUserByUsernameAsync(name);
            if (user == null)
            {
                return false;
            }

            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Authenticate user using PBKDF2: compare the hash of the supplied password with the stored hash.
        public async Task<AppUser?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;

            // Compute hash for the provided password using the stored salt.
            var hashedInput = HashPassword(password, user.Salt, Iterations, HashSize);
            if (user.PasswordHash.SequenceEqual(hashedInput))
            {
                SetCurrentUser(user);
                return user;
            }
            return null;
        }

        // Create a new user, storing a salted and hashed password.
        public async Task<AppUser?> CreateUserAsync(string username, string password)
        {
            // Check if the username is already taken.
            var existingUser = await GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                return null;
            }

            // Generate a salt and hash the password with it.
            var salt = GenerateSalt(SaltSize);
            var hash = HashPassword(password, salt, Iterations, HashSize);

            var user = new AppUser
            {
                Username = username,
                Salt = salt,
                PasswordHash = hash
            };

            await SaveUserAsync(user);
            return user;
        }

        // Generates a cryptographically secure random salt.
        private static byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Hashes a password with a given salt using PBKDF2.
        private static byte[] HashPassword(string password, byte[] salt, int iterations, int hashByteSize)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(hashByteSize);
            }
        }

        public bool IsAuthenticated() => _isAuthenticated;
        
        public void SignOut()
        {
            _currentUser = null;
            _currentUserId = 0;
            _isAuthenticated = false;
        }

        public void SetCurrentUser(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            
            _currentUser = user;
            _currentUserId = user.Id;
            _isAuthenticated = true;
        }
    }
}
