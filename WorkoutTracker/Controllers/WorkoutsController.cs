using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Authorize]
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int PageSize = 10;

        public WorkoutsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

        // Current user's ID for filtering workouts
        private string CurrentUserId =>
            _userManager.GetUserId(User)!;

        // Filter the current user's workouts
        private IQueryable<WorkoutModel> UserWorkouts =>
            _context.Workouts
                .Where(w => w.UserId == CurrentUserId);

        // GET: Workouts
        public async Task<IActionResult> Index(int? workoutTypeId, int? month, string? sort = "desc", int page = 1)
        {
            PopulateDropdowns(workoutTypeId);

            // Get months from the user's workouts for the month filter dropdown
            var availableMonths = await UserWorkouts
                .Select(w => w.Date.Month)
                .Distinct()
                .OrderByDescending(m => m)
                .ToListAsync();

            // Create a SelectList for the month dropdown with month names
            ViewBag.Months = new SelectList(availableMonths.Select(m => new
            {
                Value = m,
                Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
            }),
                "Value", "Text", month);

            ViewBag.SelectedMonth = month;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.SelectedWorkoutTypeId = workoutTypeId;

            // Start with the current user's workouts and apply filters
            IQueryable<WorkoutModel> workoutsQuery = UserWorkouts;

            if (workoutTypeId.HasValue)
            {
                workoutsQuery = workoutsQuery.Where(w => w.WorkoutTypeId == workoutTypeId);
            }

            if (month.HasValue)
            {
                workoutsQuery = workoutsQuery.Where(w => w.Date.Month == month);
            }

            // Sorting based on sort order (ascending or descending)
            workoutsQuery = sort == "asc" ? workoutsQuery.OrderBy(w => w.Date) : workoutsQuery.OrderByDescending(w => w.Date);

            // Calculate total items and pages for pagination
            int totalItems = await workoutsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            // Include workout type in index view and apply pagination
            var workouts = await workoutsQuery
            .Include(w => w.WorkoutType)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

            return View(workouts);
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await GetWorkoutAsync(id.Value, true, true);

            return workout == null ? NotFound() : View(workout);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            PopulateExerciseJson();
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutModel model)
        {
            model.UserId = CurrentUserId;

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"FIELD: {entry.Key} | ERROR: {error.ErrorMessage}");
                    }
                }

                PopulateDropdowns(model.WorkoutTypeId);
                return View(model);
            }


            _context.Workouts.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await GetWorkoutAsync(id.Value, true, true);

            if (workout == null)
            {
                return NotFound();
            }

            PopulateDropdowns(workout.WorkoutTypeId);
            PopulateExerciseJson();
            return View(workout);
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // Load existing workout with exercises for the current user
            var workout = await GetWorkoutAsync(id, includeExercises: true);

            if (workout == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.WorkoutTypeId);
                return View(model);
            }

            // Update workout properties
            workout.Date = model.Date;
            workout.Duration = model.Duration;
            workout.Notes = model.Notes;
            workout.WorkoutTypeId = model.WorkoutTypeId;

            // Remove exercises that were deleted in the form
            var existingExercises = workout.WorkoutExercises.ToList();

            foreach (var existing in existingExercises)
            {
                if (!model.WorkoutExercises.Any(we => we.Id == existing.Id))
                {
                    _context.WorkoutExercises.Remove(existing);
                }
            }

            // Update existing exercises and add new ones from the form
            foreach (var incoming in model.WorkoutExercises)
            {
                var existing = workout.WorkoutExercises.FirstOrDefault(we => we.Id == incoming.Id);

                if (existing != null)
                {
                    existing.ExerciseId = incoming.ExerciseId;
                    existing.Weight = incoming.Weight;
                    existing.Sets = incoming.Sets;
                    existing.Reps = incoming.Reps;
                    existing.Distance = incoming.Distance;
                    existing.ExerciseDetails = incoming.ExerciseDetails;
                }
                else
                {
                    workout.WorkoutExercises.Add(new WorkoutExerciseModel
                    {
                        ExerciseId = incoming.ExerciseId,
                        Weight = incoming.Weight,
                        Sets = incoming.Sets,
                        Reps = incoming.Reps,
                        Distance = incoming.Distance,
                        ExerciseDetails = incoming.ExerciseDetails
                    });
                }
            }

            await _context.SaveChangesAsync();

            // After saving, redirect to the details page to show the updated workout
            return RedirectToAction(nameof(Details), new { id = workout.Id });
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await GetWorkoutAsync(id.Value, true, true);

            return workout == null ? NotFound() : View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await UserWorkouts.FirstOrDefaultAsync(w => w.Id == id);

            if (workout == null)
            {
                return NotFound();
            }

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<WorkoutModel?> GetWorkoutAsync(
            int id, bool includeExercises = false, bool includeType = false)
        {
            IQueryable<WorkoutModel> query = UserWorkouts;

            if (includeExercises)
            {
                // Include exercises and their details for the workout
                query = query.Include(w => w.WorkoutExercises).ThenInclude(we => we.Exercise);
            }

            if (includeType)
            {
                // Include the workout type for the workout
                query = query.Include(w => w.WorkoutType);
            }

            // Return the current user's workout with the specified ID
            return await query.FirstOrDefaultAsync(w => w.Id == id);
        }

        // Populate dropdowns for workout types and exercises
        private void PopulateDropdowns(int? selectedId = null)
        {
            ViewData["WorkoutTypeId"] = new SelectList(_context.WorkoutTypes, "Id", "Name", selectedId);
            ViewData["Exercises"] = new SelectList(_context.Exercises, "Id", "Name", selectedId);
        }

        // Populate exercises as JSON to filter exercises by category in the edit form using JavaScript
        private void PopulateExerciseJson()
        {
            var exercises = _context.Exercises
                .Select(e => new { e.Id, e.Name, e.CategoryId })
                .ToList();

            // Send exercises as JSON for cascading dropdown in the edit form
            ViewBag.ExercisesJson = System.Text.Json.JsonSerializer.Serialize(exercises);

            // Load exercise categories for the dropdown in the edit form
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name");
        }
    }
}
