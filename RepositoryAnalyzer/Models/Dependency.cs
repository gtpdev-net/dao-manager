namespace RepositoryAnalyzer.Models;

public class Dependency
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public string SourceType { get; set; } = string.Empty; // "Solution" or "Project"
    public string TargetType { get; set; } = string.Empty; // "Project"
    public int? SourceSolutionId { get; set; }
    public int? SourceProjectId { get; set; }
    public int TargetProjectId { get; set; }
    
    // Navigation properties
    public Solution? SourceSolution { get; set; }
    public Project? SourceProject { get; set; }
    public Project TargetProject { get; set; } = null!;
}
