using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

public class WorkoutExerciseModel
{
    public int Id { get; set; }

    // Foreign keys
    [Required]
    public int WorkoutId { get; set; }
    public WorkoutModel Workout { get; set; } = null!;

    [Required]
    [Display(Name = "Övning")]
    public int ExerciseId { get; set; }
    public ExerciseModel Exercise { get; set; } = null!;

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

    [Range(0, 1440, ErrorMessage = "Tiden måste vara 0 eller högre.")]
    [Display(Name = "Tid (minuter)")]
    public int? Duration { get; set; } // Duration in minutes
}