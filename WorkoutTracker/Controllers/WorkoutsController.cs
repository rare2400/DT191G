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
        public async Task<IActionResult> Index()
        {
            var workouts = await UserWorkouts
                .Include(w => w.WorkoutType)
                .ToListAsync();

            return View(workouts);
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var workout = await UserWorkouts
                .Include(w => w.WorkoutType)
                .FirstOrDefaultAsync(w => w.Id == id);

            return workout == null ? NotFound() : View(workout);
        }

        // GET: Workouts/Create
        public IActionResult Create()
        {
            PopulateWorkoutTypes();
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Duration,Notes,WorkoutTypeId")] WorkoutModel workout)
        {
            if (!ModelState.IsValid)
            {
                PopulateWorkoutTypes(workout.WorkoutTypeId);
                return View(workout);
            }

            workout.UserId = CurrentUserId;

            _context.Add(workout);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var workout = await UserWorkouts.FirstOrDefaultAsync(w => w.Id == id);

            if (workout == null)
            {
                return NotFound();
            }

            PopulateWorkoutTypes(workout.WorkoutTypeId);
            return View(workout);
        }

        // POST: Workouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Duration,Notes,WorkoutTypeId")] WorkoutModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var workout = await UserWorkouts.FirstOrDefaultAsync(w => w.Id == id);

            if (workout == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateWorkoutTypes(model.WorkoutTypeId);
                return View(model);
            }

            workout.Date = model.Date;
            workout.Duration = model.Duration;
            workout.Notes = model.Notes;
            workout.WorkoutTypeId = model.WorkoutTypeId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var workout = await UserWorkouts
            .Include(w => w.WorkoutType)
            .FirstOrDefaultAsync(w => w.Id == id);

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

        private void PopulateWorkoutTypes(int? selectedId = null)
        {
            ViewData["WorkoutTypeId"] = new SelectList(_context.WorkoutTypes, "Id", "Name", selectedId);
        }
    }
}
