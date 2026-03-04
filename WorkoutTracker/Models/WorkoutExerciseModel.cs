using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

public class WorkoutExerciseModel
{
    public int Id { get; set; }

    // Foreign keys
    [Required]
    public int WorkoutId { get; set; }
    public WorkoutModel? Workout { get; set; }

    [Required]
    [Display(Name = "Övning")]
    public int ExerciseId { get; set; }
    public ExerciseModel? Exercise { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Vikt måste vara 0 eller högre.")]
    [Display(Name = "Vikt (kg)")]
    public double? Weight { get; set; }

    [Range(0, 50, ErrorMessage = "Antal set måste vara 0 eller högre.")]
    [Display(Name = "Antal set")]
    public int? Sets { get; set; }

    [Range(0, 1000, ErrorMessage = "Antal reps måste vara 0 eller högre.")]
    [Display(Name = "Antal repetitioner")]
    public int? Reps { get; set; }

    [Range(0, 1000, ErrorMessage = "Distansen måste vara 0 eller högre.")]
    [Display(Name = "Distans (km)")]
    public double? Distance { get; set; }

    [StringLength(100, ErrorMessage = "Namnet på övningen får inte vara längre än 100 tecken.")]
    [Display(Name = "Detaljer")]
    public string? ExerciseDetails { get; set; }
}