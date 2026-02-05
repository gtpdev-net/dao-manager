namespace DAO.Manager.Models;

public class ScanDetailsViewModel
{
    public Scan Scan { get; set; } = null!;
    public int SolutionsCount { get; set; }
    public int ProjectsCount { get; set; }
    public int AssembliesCount { get; set; }
}
