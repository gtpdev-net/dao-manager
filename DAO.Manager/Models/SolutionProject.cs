namespace DAO.Manager.Models;

public class SolutionProject
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public int SolutionId { get; set; }
    public int ProjectId { get; set; }
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public Solution Solution { get; set; } = null!;
    public Project Project { get; set; } = null!;
}
