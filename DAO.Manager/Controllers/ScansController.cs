using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAO.Manager.Data;
using DAO.Manager.Services;

namespace DAO.Manager.Controllers;

public class ScansController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RepositoryScannerService _scannerService;
    private readonly ILogger<ScansController> _logger;

    public ScansController(
        ApplicationDbContext context,
        RepositoryScannerService scannerService,
        ILogger<ScansController> logger)
    {
        _context = context;
        _scannerService = scannerService;
        _logger = logger;
    }

    // GET: Scans
    public async Task<IActionResult> Index()
    {
        var scans = await _context.Scans
            .OrderByDescending(s => s.ScanDate)
            .ToListAsync();
        return View(scans);
    }

    // GET: Scans/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans
            .Include(s => s.Solutions)
            .Include(s => s.Projects)
            .Include(s => s.Assemblies)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (scan == null)
        {
            return NotFound();
        }

        return View(scan);
    }

    // GET: Scans/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Scans/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RepositoryPath")] string repositoryPath)
    {
        if (!string.IsNullOrWhiteSpace(repositoryPath))
        {
            try
            {
                var scan = await _scannerService.ScanRepositoryAsync(repositoryPath);
                return RedirectToAction(nameof(Details), new { id = scan.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning repository: {Path}", repositoryPath);
                ModelState.AddModelError("", $"Error scanning repository: {ex.Message}");
            }
        }

        return View();
    }

    // GET: Scans/Solutions/5
    public async Task<IActionResult> Solutions(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans.FindAsync(id);
        if (scan == null)
        {
            return NotFound();
        }

        var solutions = await _context.Solutions
            .Where(s => s.ScanId == id)
            .OrderBy(s => s.Name)
            .ToListAsync();

        ViewBag.Scan = scan;
        return View(solutions);
    }

    // GET: Scans/Projects/5
    public async Task<IActionResult> Projects(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans.FindAsync(id);
        if (scan == null)
        {
            return NotFound();
        }

        var projects = await _context.Projects
            .Where(p => p.ScanId == id)
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Scan = scan;
        return View(projects);
    }

    // GET: Scans/Assemblies/5
    public async Task<IActionResult> Assemblies(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans.FindAsync(id);
        if (scan == null)
        {
            return NotFound();
        }

        var assemblies = await _context.Assemblies
            .Where(a => a.ScanId == id)
            .OrderBy(a => a.Name)
            .ToListAsync();

        ViewBag.Scan = scan;
        return View(assemblies);
    }

    // GET: Scans/Dependencies/5
    public async Task<IActionResult> Dependencies(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans.FindAsync(id);
        if (scan == null)
        {
            return NotFound();
        }

        var dependencies = await _context.Dependencies
            .Include(d => d.SourceSolution)
            .Include(d => d.SourceProject)
            .Include(d => d.TargetProject)
            .Where(d => d.ScanId == id)
            .ToListAsync();

        ViewBag.Scan = scan;
        return View(dependencies);
    }

    // GET: Scans/AssemblyDependencies/5
    public async Task<IActionResult> AssemblyDependencies(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans.FindAsync(id);
        if (scan == null)
        {
            return NotFound();
        }

        var assemblyDependencies = await _context.AssemblyDependencies
            .Include(ad => ad.SourceAssembly)
            .Include(ad => ad.TargetAssembly)
            .Where(ad => ad.ScanId == id)
            .ToListAsync();

        ViewBag.Scan = scan;
        return View(assemblyDependencies);
    }

    // GET: Scans/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var scan = await _context.Scans
            .FirstOrDefaultAsync(m => m.Id == id);

        if (scan == null)
        {
            return NotFound();
        }

        return View(scan);
    }

    // POST: Scans/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var scan = await _context.Scans.FindAsync(id);
        if (scan != null)
        {
            _context.Scans.Remove(scan);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
