namespace DAO.Manager.Models;

public class Project
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public string UniqueIdentifier { get; set; } = string.Empty;
    public string? VisualStudioGuid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string GuidDeterminationMethod { get; set; } = string.Empty;
    public string? TargetFramework { get; set; }
    public string ProjectStyle { get; set; } = string.Empty;
    
    // Navigation properties
    public Scan Scan { get; set; } = null!;
    public ICollection<SolutionProject> SolutionProjects { get; set; } = new List<SolutionProject>();
    public ICollection<Assembly> Assemblies { get; set; } = new List<Assembly>();
    public ICollection<PackageReference> PackageReferences { get; set; } = new List<PackageReference>();
    public ICollection<AssemblyReference> AssemblyReferences { get; set; } = new List<AssemblyReference>();
    public ICollection<ProjectDependency> DependenciesFrom { get; set; } = new List<ProjectDependency>();
    public ICollection<ProjectDependency> DependenciesTo { get; set; } = new List<ProjectDependency>();
}
