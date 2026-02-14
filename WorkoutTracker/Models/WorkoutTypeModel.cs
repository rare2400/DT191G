using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

public class WorkoutTypeModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ange typ av träning.")]
    [StringLength(50, ErrorMessage = "Namnet på övningen får inte vara längre än 50 tecken.")]
    [Display(Name = "Träningstyp")]
    public required string Name { get; set; }

    // Relations
    public List<WorkoutModel> Workouts { get; set; } = [];
}