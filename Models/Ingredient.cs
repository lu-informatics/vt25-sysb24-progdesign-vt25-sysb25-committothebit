using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Informatics.Appetite.Models;

[Table("Ingredient")]
public class Ingredient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ingredientId")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string? Name { get; set; }

    [Required]
    [Column("Category")]
    [MaxLength(100)]
    public string? Category { get; set; }

    [Required]
    [Column("Unit")]
    [MaxLength(50)]
    public string? Unit { get; set; }

    [Required]
    [Column("DietTag")]
    [MaxLength(255)]
    public string? DietTag { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();


}
