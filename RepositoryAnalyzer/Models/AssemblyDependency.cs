namespace RepositoryAnalyzer.Models;

public class AssemblyDependency
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public int SourceAssemblyId { get; set; }
    public int TargetAssemblyId { get; set; }
    
    // Navigation properties
    public Assembly SourceAssembly { get; set; } = null!;
    public Assembly TargetAssembly { get; set; } = null!;
}
