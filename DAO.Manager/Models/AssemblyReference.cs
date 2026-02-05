namespace DAO.Manager.Models;

public class AssemblyReference
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public int ProjectId { get; set; }
    public string AssemblyName { get; set; } = string.Empty;
    public string? HintPath { get; set; }
    public string? Version { get; set; }
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public Project Project { get; set; } = null!;
}
