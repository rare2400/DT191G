using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WorkoutTracker.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Ange ditt förnamn.")]
    [StringLength(50, ErrorMessage = "Förnamnet får inte vara längre än 50 tecken.")]
    [Display(Name = "Förnamn")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Ange ditt efternamn.")]
    [StringLength(50, ErrorMessage = "Efternamnet får inte vara längre än 50 tecken.")]
    [Display(Name = "Efternamn")]
    public required string LastName { get; set; }
}