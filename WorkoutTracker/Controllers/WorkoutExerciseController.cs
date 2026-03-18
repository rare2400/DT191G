using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Authorize]
    public class WorkoutExerciseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutExerciseController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET: WorkoutExercise
        public async Task<IActionResult> Index(int? exerciseId)
        {
            await PopulateExerciseAndCategoryData(exerciseId);

            if (!exerciseId.HasValue)
            {
                return View(new List<WorkoutExerciseModel>());
            }

            // Get all workout exercises for the current user for the selected exercise 
            var progress = await _context.WorkoutExercises
                .Include(we => we.Workout)
                .Include(we => we.Exercise)
                .Where(we => we.ExerciseId == exerciseId && we.Workout!.UserId == CurrentUserId)
                .OrderByDescending(we => we.Workout!.Date)
                .ToListAsync();

            return View(progress);
        }

        // Populate exercises and categories for dropdowns and filtering
        private async Task PopulateExerciseAndCategoryData(int? selectedExerciseId = null)
        {
            var exercises = await _context.Exercises
                .Where(e => e.UserId == null || e.UserId == CurrentUserId)
                .OrderBy(e => e.Name)
                .ToListAsync();

            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            ViewData["Exercises"] = new SelectList(exercises, "Id", "Name", selectedExerciseId);
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.ExercisesJson = System.Text.Json.JsonSerializer.Serialize(
                exercises.Select(e => new { e.Id, e.Name, e.CategoryId })
            );
        }
    }
}
