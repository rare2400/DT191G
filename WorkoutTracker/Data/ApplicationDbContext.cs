using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkoutModel> Workouts { get; set; } = null!;
    public DbSet<WorkoutTypeModel> WorkoutTypes { get; set; } = null!;
    public DbSet<ExerciseModel> Exercises { get; set; } = null!;
    public DbSet<CategoryModel> Categories { get; set; } = null!;
    public DbSet<WorkoutExerciseModel> WorkoutExercises { get; set; } = null!;
}

