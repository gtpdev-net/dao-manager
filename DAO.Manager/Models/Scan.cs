namespace DAO.Manager.Models;

public class Scan
{
    public int Id { get; set; }
    public DateTime ScanDate { get; set; }
    public string GitCommitHash { get; set; } = string.Empty;
    public string ShortCommitHash { get; set; } = string.Empty;
    public string RepositoryPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Assembly> Assemblies { get; set; } = new List<Assembly>();
    public ICollection<PackageReference> PackageReferences { get; set; } = new List<PackageReference>();
    public ICollection<AssemblyReference> AssemblyReferences { get; set; } = new List<AssemblyReference>();
    public ICollection<SolutionProject> SolutionProjects { get; set; } = new List<SolutionProject>();
    public ICollection<ProjectDependency> ProjectDependencies { get; set; } = new List<ProjectDependency>();
    public ICollection<AssemblyDependency> AssemblyDependencies { get; set; } = new List<AssemblyDependency>();
}
