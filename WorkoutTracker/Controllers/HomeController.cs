using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    // Dependency injection of ApplicationDbContext and UserManager
    public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
        }

    // Helper to get the current user's ID
    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    // GET: Home/Index
    public async Task<IActionResult> Index()
    {
        // Check if the user is authenticated and has a valid ID
        if (User.Identity?.IsAuthenticated == true && CurrentUserId != null)
        {
            // Get the current user and pass their first name to the view
            var user = await _userManager.GetUserAsync(User);
            ViewBag.FirstName = user?.FirstName;

            // Fetch user's workouts, including the workout type, and order by date
            var userId = CurrentUserId;
            var workouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .Include(w => w.WorkoutType)
                .OrderByDescending(w => w.Date)
                .ToListAsync();

            // Pass workout stats to the view
            ViewBag.TotalWorkouts = workouts.Count;
            ViewBag.ThisMonth = workouts.Count(w => w.Date.Month == DateTime.Now.Month && w.Date.Year == DateTime.Now.Year);
            ViewBag.LastWorkout = workouts.FirstOrDefault();
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
