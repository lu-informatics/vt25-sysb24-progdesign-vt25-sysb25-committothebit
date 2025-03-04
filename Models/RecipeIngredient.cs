using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Informatics.Appetite.Models;

[Table("RecipeIngredient")]
public class RecipeIngredient
{
    [Required]
    [Column("recipeId")]
    public int RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    [Required]
    [Column("ingredientId")]
    public int IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }

    [Required]
    [Column("amount", TypeName = "decimal(10,2)")]
    public double Amount { get; set; }
}
