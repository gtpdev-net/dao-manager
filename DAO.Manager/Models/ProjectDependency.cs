namespace DAO.Manager.Models;

public class ProjectDependency
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public int SourceProjectId { get; set; }
    public int TargetProjectId { get; set; }
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public Project SourceProject { get; set; } = null!;
    public Project TargetProject { get; set; } = null!;
}
