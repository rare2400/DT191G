using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

public class CategoryModel
{
    // Properties
    public int Id { get; set; }

    [Required(ErrorMessage = "Ange ett kategorinamn.")]
    [StringLength(50, ErrorMessage = "Kategorinamnet får inte vara längre än 50 tecken.")]
    [Display(Name = "Kategori")]
    public required string Name { get; set; }

    // Relations
    public List<ExerciseModel> Exercises { get; set; } = [];
}