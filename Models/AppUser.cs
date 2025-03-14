using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace Informatics.Appetite.Models;

[Table("AppUser")]
public class AppUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("AppUserID")]
    public int Id { get; set; }

    [Required]
    [Column("Username")]
    [MaxLength(50)]
    public string? Username { get; set; }

    [Required]
    [Column("PasswordHash")]
    public byte[] PasswordHash { get; set; } // PBKDF2 hashed password

    [Required]
    [Column("Salt")]
    public byte[] Salt { get; set; } // Stored as Base64-encoded string

    public ICollection<UserIngredient> UserIngredients { get; set; } = new List<UserIngredient>();

    private const int SaltSize = 16; // 128-bit salt
    private const int HashSize = 32; // 256-bit hash
    private const int Iterations = 100000; // OWASP recommendation (2024)

    /// <summary>
    /// Generates a new salt and hashes the password.
    /// </summary>
    public void SetPassword(string password)
    {
        byte[] saltBytes = GenerateSalt();
        Salt = saltBytes; // Convert salt to Base64 for storage
        PasswordHash = HashPassword(password, saltBytes);
    }

    /// <summary>
    /// Verifies if the entered password matches the stored hash.
    /// </summary>
    public bool VerifyPassword(string enteredPassword)
    {
        byte[] saltBytes = Salt; // Convert back to bytes
        var hashedInput = HashPassword(enteredPassword, saltBytes);
        return StructuralComparisons.StructuralEqualityComparer.Equals(PasswordHash, hashedInput);
    }

    private static byte[] GenerateSalt()
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(HashSize);
        }
    }
}
