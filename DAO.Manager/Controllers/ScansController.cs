using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using DAO.Manager.Data;
using DAO.Manager.Services;
using DAO.Manager.Hubs;
using DAO.Manager.Models;

namespace DAO.Manager.Controllers;

public class ScansController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RepositoryScannerService _scannerService;
    private readonly ILogger<ScansController> _logger;
    private readonly IHubContext<ScanProgressHub> _hubContext;

    public ScansController(
        ApplicationDbContext context,
        RepositoryScannerService scannerService,
        ILogger<ScansController> logger,
        IHubContext<ScanProgressHub> hubContext)
    {
        _context = context;
        _scannerService = scannerService;
        _logger = logger;
        _hubContext = hubContext;
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
            .FirstOrDefaultAsync(m => m.Id == id);

        if (scan == null)
        {
            return NotFound();
        }

        // Get counts separately instead of loading all data
        var viewModel = new ScanDetailsViewModel
        {
            Scan = scan,
            SolutionsCount = await _context.Solutions.CountAsync(s => s.ScanId == id),
            ProjectsCount = await _context.Projects.CountAsync(p => p.ScanId == id),
            AssembliesCount = await _context.Assemblies.CountAsync(a => a.ScanId == id)
        };

        return View(viewModel);
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
                // Validate path exists
                if (!Directory.Exists(repositoryPath))
                {
                    ModelState.AddModelError("", $"Directory not found: {repositoryPath}");
                    return View();
                }

                // Create a placeholder scan record to get an ID
                var scan = new Scan
                {
                    ScanDate = DateTime.UtcNow,
                    GitCommitHash = "Scanning...",
                    ShortCommitHash = "Scanning...",
                    RepositoryPath = repositoryPath
                };
                
                _context.Scans.Add(scan);
                await _context.SaveChangesAsync();

                var scanId = scan.Id;

                // Start scan in background
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var progressReporter = new SignalRProgressReporter(_hubContext, scanId.ToString());
                        
                        // Remove the placeholder scan
                        using var scope = HttpContext.RequestServices.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var scannerService = scope.ServiceProvider.GetRequiredService<RepositoryScannerService>();
                        
                        var scanToRemove = await dbContext.Scans.FindAsync(scanId);
                        if (scanToRemove != null)
                        {
                            dbContext.Scans.Remove(scanToRemove);
                            await dbContext.SaveChangesAsync();
                        }

                        // Perform the actual scan
                        await scannerService.ScanRepositoryAsync(repositoryPath, progressReporter);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in background scan for {Path}", repositoryPath);
                        var progressReporter = new SignalRProgressReporter(_hubContext, scanId.ToString());
                        await progressReporter.ReportError($"Scan failed: {ex.Message}");
                    }
                });

                // Return scan progress view
                return View("ScanProgress", scanId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting repository scan: {Path}", repositoryPath);
                ModelState.AddModelError("", $"Error starting scan: {ex.Message}");
            }
        }
        else
        {
            ModelState.AddModelError("", "Repository path is required");
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
