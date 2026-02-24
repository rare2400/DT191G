using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models;

public class WorkoutModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Välj datum")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    [Display(Name = "Datum")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Ange träningstid")]
    [Range(1, int.MaxValue, ErrorMessage = "Träningstid måste vara minst 1 minut.")]
    [Display(Name = "Träningstid (i minuter)")]
    public int Duration { get; set; } // Duration in minutes

    [Display(Name = "Kommentar")]
    [StringLength(200, ErrorMessage = "Träningskommentar får inte vara längre än 200 tecken.")]
    public string? Notes { get; set; } // Optional notes about the workout

    // Foreign keys
    [Required(ErrorMessage = "Välj träningstyp")]
    [Display(Name = "Träningstyp")]
    public int WorkoutTypeId { get; set; }
    
    public WorkoutTypeModel ? WorkoutType { get; set; }

    // Identity user
    public string? UserId { get; set; }

    // Relations
    [Display(Name = "Övningar")]
    public List<WorkoutExerciseModel> WorkoutExercises { get; set; } = [];
}