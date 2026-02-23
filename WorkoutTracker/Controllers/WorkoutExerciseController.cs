using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {

            var workoutExercises = _context.WorkoutExercises
                .Include(w => w.Exercise)
                .Include(w => w.Workout)
                .Where(w => w.Workout.UserId == CurrentUserId)
                .ToListAsync();

            return View(await workoutExercises);
        }

        // GET: WorkoutExercise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await GetUserWorkoutExercise(id.Value);
            if (workoutExercise == null)
            {
                return NotFound();
            }

            return View(workoutExercise);
        }

        // GET: WorkoutExercise/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: WorkoutExercise/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkoutId,ExerciseId,Weight,Sets,Reps,Distance,Duration")] WorkoutExerciseModel workoutExerciseModel)
        {
            if (!await UserWorkout(workoutExerciseModel.WorkoutId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Add(workoutExerciseModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(workoutExerciseModel);
            return View(workoutExerciseModel);
        }

        // GET: WorkoutExercise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExercise = await GetUserWorkoutExercise(id.Value);

            if (workoutExercise == null)
            {
                return NotFound();
            }

            await PopulateDropdowns(workoutExercise);
            return View(workoutExercise);
        }

        // POST: WorkoutExercise/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkoutId,ExerciseId,Weight,Sets,Reps,Distance,Duration")] WorkoutExerciseModel workoutExerciseModel)
        {
            if (id != workoutExerciseModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(workoutExerciseModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            await PopulateDropdowns(workoutExerciseModel);
            return View(workoutExerciseModel);
        }


        // GET: WorkoutExercise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutExerciseModel = await GetUserWorkoutExercise(id.Value);
            if (workoutExerciseModel == null)
            {
                return NotFound();
            }

            return View(workoutExerciseModel);
        }

        // POST: WorkoutExercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutExercise = await GetUserWorkoutExercise(id);
            
            if (workoutExercise == null)
            {
                return NotFound();
            }

            _context.WorkoutExercises.Remove(workoutExercise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<WorkoutExerciseModel?> GetUserWorkoutExercise(int id)
        {
            return await _context.WorkoutExercises
                .Include(w => w.Exercise)
                .Include(w => w.Workout)
                .FirstOrDefaultAsync(w =>
                    w.Id == id && w.Workout.UserId == CurrentUserId);
        }

        private async Task<bool> UserWorkout(int workoutId)
        {
            return await _context.Workouts
                .AnyAsync(w =>
                    w.Id == workoutId && w.UserId == CurrentUserId);
        }

        private async Task PopulateDropdowns(WorkoutExerciseModel? model = null)
        {
            var userWorkouts = await _context.Workouts
                .Where(w => w.UserId == CurrentUserId)
                .ToListAsync();

            ViewData["ExerciseId"] = new SelectList(_context.Exercises, "Id", "Name", model?.ExerciseId);
            ViewData["WorkoutId"] = new SelectList(userWorkouts, "Id", "Name", model?.WorkoutId);
        }
    }
}
