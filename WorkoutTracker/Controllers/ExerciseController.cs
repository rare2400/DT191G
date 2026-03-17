using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PageSize = 10;

        public ExerciseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User)!;

        private IQueryable<ExerciseModel> UserExercises =>
            _context.Exercises.Where(e => e.UserId == CurrentUserId || e.UserId == null);

        // GET: Exercise
        public async Task<IActionResult> Index(int? categoryId, int page = 1)
        {
            IQueryable<ExerciseModel> query = UserExercises.Include(e => e.Category);

            // Filter by category if a categoryId is provided
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId);
            }

            query = query.OrderBy(e => e.UserId == null ? 1: 0).ThenBy(e => e.Name);

            // Pagination
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Keep the selected category in the dropdown
            PopulateCategoryDropdown(categoryId);

            // Pass pagination info to the view
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            var exercises = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return View(exercises);
        }

        // GET: Exercise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseModel = await _context.Exercises
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseModel == null)
            {
                return NotFound();
            }

            return View(exerciseModel);
        }

        // GET: Exercise/Create
        public IActionResult Create()
        {
            PopulateCategoryDropdown();
            return View();
        }

        // POST: Exercise/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CategoryId")] ExerciseModel exerciseModel)
        {
            if (ModelState.IsValid)
            {
                exerciseModel.UserId = CurrentUserId;
                _context.Exercises.Add(exerciseModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategoryDropdown(exerciseModel.CategoryId);
            return View(exerciseModel);
        }

        // GET: Exercise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseModel = await _context.Exercises.FindAsync(id);
            if (exerciseModel == null || exerciseModel.UserId != CurrentUserId)
            {
                return Forbid();
            }

            PopulateCategoryDropdown(exerciseModel.CategoryId);
            return View(exerciseModel);
        }

        // POST: Exercise/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId")] ExerciseModel exerciseModel)
        {
            if (id != exerciseModel.Id)
            {
                return NotFound();
            }

            var existing = await _context.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

            if (existing == null || (existing.UserId != CurrentUserId))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    exerciseModel.UserId = existing.UserId; // Preserve original UserId
                    _context.Update(exerciseModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseModelExists(exerciseModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateCategoryDropdown(exerciseModel.CategoryId);
            return View(exerciseModel);
        }

        // GET: Exercise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseModel = await UserExercises
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseModel == null || exerciseModel.UserId != CurrentUserId)
            {
                return NotFound();
            }

            return View(exerciseModel);
        }

        // POST: Exercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exerciseModel = await _context.Exercises.FindAsync(id);
            if (exerciseModel == null || exerciseModel.UserId != CurrentUserId)
            {
                return Forbid();
            }

            _context.Exercises.Remove(exerciseModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseModelExists(int id)
        {
            return _context.Exercises.Any(e => e.Id == id);
        }

        // Helper method to populate category dropdown
        private void PopulateCategoryDropdown(int? selectedId = null)
        {
            ViewBag.Categories = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", selectedId);
        }
    }
}
