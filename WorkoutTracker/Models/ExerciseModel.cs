using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

public class ExerciseModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ange ett namn för övningen.")]
    [StringLength(100, ErrorMessage = "Namnet på övningen får inte vara längre än 100 tecken.")]
    [Display(Name = "Namn på övning")]
    public required string Name { get; set; }

    // Foreign keys
    [Required]
    [Display(Name = "Kategori")]
    public int CategoryId { get; set; }
    public CategoryModel ? Category { get; set; }

    // Relations
    public List<WorkoutExerciseModel> WorkoutExercises { get; set; } = [];
}