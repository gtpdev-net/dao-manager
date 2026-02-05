namespace DAO.Manager.Models;

public class Assembly
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AssemblyFileName { get; set; } = string.Empty;
    public string OutputType { get; set; } = string.Empty;
    public string ProjectStyle { get; set; } = string.Empty;
    public string TargetFramework { get; set; } = string.Empty;
    public string ProjectFilePath { get; set; } = string.Empty;
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public ICollection<AssemblyDependency> DependenciesFrom { get; set; } = new List<AssemblyDependency>();
    public ICollection<AssemblyDependency> DependenciesTo { get; set; } = new List<AssemblyDependency>();
}
