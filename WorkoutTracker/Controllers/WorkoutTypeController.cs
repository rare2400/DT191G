using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    public class WorkoutTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkoutTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkoutType
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkoutTypes.ToListAsync());
        }

        // GET: WorkoutType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutTypeModel = await _context.WorkoutTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workoutTypeModel == null)
            {
                return NotFound();
            }

            return View(workoutTypeModel);
        }

        // GET: WorkoutType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkoutType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] WorkoutTypeModel workoutTypeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workoutTypeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workoutTypeModel);
        }

        // GET: WorkoutType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutTypeModel = await _context.WorkoutTypes.FindAsync(id);
            if (workoutTypeModel == null)
            {
                return NotFound();
            }
            return View(workoutTypeModel);
        }

        // POST: WorkoutType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] WorkoutTypeModel workoutTypeModel)
        {
            if (id != workoutTypeModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workoutTypeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutTypeModelExists(workoutTypeModel.Id))
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
            return View(workoutTypeModel);
        }

        // GET: WorkoutType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workoutTypeModel = await _context.WorkoutTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workoutTypeModel == null)
            {
                return NotFound();
            }

            return View(workoutTypeModel);
        }

        // POST: WorkoutType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workoutTypeModel = await _context.WorkoutTypes.FindAsync(id);
            if (workoutTypeModel != null)
            {
                _context.WorkoutTypes.Remove(workoutTypeModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutTypeModelExists(int id)
        {
            return _context.WorkoutTypes.Any(e => e.Id == id);
        }
    }
}
