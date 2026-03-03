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
        public async Task<IActionResult> Index(int? workoutTypeId)
        {
            PopulateDropdowns(workoutTypeId);

            IQueryable<WorkoutModel> workoutsQuery = UserWorkouts;

            if (workoutTypeId.HasValue)
            {
                workoutsQuery = workoutsQuery.Where(w => w.WorkoutTypeId == workoutTypeId);
            }

            var workouts = await workoutsQuery
            .Include(w => w.WorkoutType)
            .OrderByDescending(w => w.Date)
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
                    existing.Duration = incoming.Duration;
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
                        Duration = incoming.Duration
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
                query = query.Include(w => w.WorkoutExercises).ThenInclude(we => we.Exercise);
            }

            if (includeType)
            {
                query = query.Include(w => w.WorkoutType);
            }

            return await query.FirstOrDefaultAsync(w => w.Id == id);
        }

        private void PopulateDropdowns(int? selectedId = null)
        {
            ViewData["WorkoutTypeId"] = new SelectList(_context.WorkoutTypes, "Id", "Name", selectedId);
            ViewData["Exercises"] = new SelectList(_context.Exercises, "Id", "Name", selectedId);
        }
    }
}
