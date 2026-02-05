namespace RepositoryAnalyzer.Models;

public class Solution
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string? VisualStudioGuid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string GuidDeterminationMethod { get; set; } = string.Empty;
    public int NumberOfReferencedProjects { get; set; }
    public bool IsSingleProjectSolution { get; set; }
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public ICollection<Dependency> DependenciesFrom { get; set; } = new List<Dependency>();
}
