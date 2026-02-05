namespace RepositoryAnalyzer.Models;

public class Project
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string? VisualStudioGuid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string GuidDeterminationMethod { get; set; } = string.Empty;
    public int NumberOfReferencedProjects { get; set; }
    public string? TargetFramework { get; set; }
    public string ProjectStyle { get; set; } = string.Empty;
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public ICollection<Dependency> DependenciesFrom { get; set; } = new List<Dependency>();
    public ICollection<Dependency> DependenciesTo { get; set; } = new List<Dependency>();
}
