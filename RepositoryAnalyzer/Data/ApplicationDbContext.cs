using Microsoft.EntityFrameworkCore;
using RepositoryAnalyzer.Models;

namespace RepositoryAnalyzer.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Scan> Scans { get; set; }
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Assembly> Assemblies { get; set; }
    public DbSet<Dependency> Dependencies { get; set; }
    public DbSet<AssemblyDependency> AssemblyDependencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Scan
        modelBuilder.Entity<Scan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GitCommitHash).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ShortCommitHash).HasMaxLength(20).IsRequired();
            entity.Property(e => e.RepositoryPath).HasMaxLength(500).IsRequired();
            entity.HasIndex(e => e.ScanDate);
        });

        // Configure Solution
        modelBuilder.Entity<Solution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UniqueIdentifier).HasMaxLength(100).IsRequired();
            entity.Property(e => e.VisualStudioGuid).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.GuidDeterminationMethod).HasMaxLength(100);
            
            entity.HasOne(e => e.Scan)
                .WithMany(s => s.Solutions)
                .HasForeignKey(e => e.ScanId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.ScanId);
            entity.HasIndex(e => e.UniqueIdentifier);
        });

        // Configure Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UniqueIdentifier).HasMaxLength(100).IsRequired();
            entity.Property(e => e.VisualStudioGuid).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.GuidDeterminationMethod).HasMaxLength(100);
            entity.Property(e => e.TargetFramework).HasMaxLength(100);
            entity.Property(e => e.ProjectStyle).HasMaxLength(50);
            
            entity.HasOne(e => e.Scan)
                .WithMany(s => s.Projects)
                .HasForeignKey(e => e.ScanId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.ScanId);
            entity.HasIndex(e => e.UniqueIdentifier);
        });

        // Configure Assembly
        modelBuilder.Entity<Assembly>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UniqueIdentifier).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.AssemblyFileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.OutputType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProjectStyle).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TargetFramework).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ProjectFilePath).HasMaxLength(1000).IsRequired();
            
            entity.HasOne(e => e.Scan)
                .WithMany(s => s.Assemblies)
                .HasForeignKey(e => e.ScanId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.ScanId);
            entity.HasIndex(e => e.UniqueIdentifier);
        });

        // Configure Dependency
        modelBuilder.Entity<Dependency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SourceType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TargetType).HasMaxLength(50).IsRequired();
            
            entity.HasOne(e => e.SourceSolution)
                .WithMany(s => s.DependenciesFrom)
                .HasForeignKey(e => e.SourceSolutionId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.SourceProject)
                .WithMany(p => p.DependenciesFrom)
                .HasForeignKey(e => e.SourceProjectId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.TargetProject)
                .WithMany(p => p.DependenciesTo)
                .HasForeignKey(e => e.TargetProjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.ScanId);
        });

        // Configure AssemblyDependency
        modelBuilder.Entity<AssemblyDependency>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.SourceAssembly)
                .WithMany(a => a.DependenciesFrom)
                .HasForeignKey(e => e.SourceAssemblyId)
                .OnDelete(DeleteBehavior.NoAction);
                
            entity.HasOne(e => e.TargetAssembly)
                .WithMany(a => a.DependenciesTo)
                .HasForeignKey(e => e.TargetAssemblyId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.ScanId);
        });
    }
}
