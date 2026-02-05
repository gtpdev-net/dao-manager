using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAO.Manager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GitCommitHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortCommitHash = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RepositoryPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assemblies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanId = table.Column<int>(type: "int", nullable: false),
                    UniqueIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AssemblyFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OutputType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetFramework = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectFilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assemblies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assemblies_Scans_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Scans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanId = table.Column<int>(type: "int", nullable: false),
                    UniqueIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VisualStudioGuid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    GuidDeterminationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberOfReferencedProjects = table.Column<int>(type: "int", nullable: false),
                    TargetFramework = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProjectStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Scans_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Scans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanId = table.Column<int>(type: "int", nullable: false),
                    UniqueIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VisualStudioGuid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    GuidDeterminationMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberOfReferencedProjects = table.Column<int>(type: "int", nullable: false),
                    IsSingleProjectSolution = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_Scans_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Scans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyDependencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanId = table.Column<int>(type: "int", nullable: false),
                    SourceAssemblyId = table.Column<int>(type: "int", nullable: false),
                    TargetAssemblyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssemblyDependencies_Assemblies_SourceAssemblyId",
                        column: x => x.SourceAssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssemblyDependencies_Assemblies_TargetAssemblyId",
                        column: x => x.TargetAssemblyId,
                        principalTable: "Assemblies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dependencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanId = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SourceSolutionId = table.Column<int>(type: "int", nullable: true),
                    SourceProjectId = table.Column<int>(type: "int", nullable: true),
                    TargetProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dependencies_Projects_SourceProjectId",
                        column: x => x.SourceProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dependencies_Projects_TargetProjectId",
                        column: x => x.TargetProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dependencies_Solutions_SourceSolutionId",
                        column: x => x.SourceSolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_ScanId",
                table: "Assemblies",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Assemblies_UniqueIdentifier",
                table: "Assemblies",
                column: "UniqueIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyDependencies_ScanId",
                table: "AssemblyDependencies",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyDependencies_SourceAssemblyId",
                table: "AssemblyDependencies",
                column: "SourceAssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyDependencies_TargetAssemblyId",
                table: "AssemblyDependencies",
                column: "TargetAssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencies_ScanId",
                table: "Dependencies",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencies_SourceProjectId",
                table: "Dependencies",
                column: "SourceProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencies_SourceSolutionId",
                table: "Dependencies",
                column: "SourceSolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencies_TargetProjectId",
                table: "Dependencies",
                column: "TargetProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ScanId",
                table: "Projects",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UniqueIdentifier",
                table: "Projects",
                column: "UniqueIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Scans_ScanDate",
                table: "Scans",
                column: "ScanDate");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_ScanId",
                table: "Solutions",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_UniqueIdentifier",
                table: "Solutions",
                column: "UniqueIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssemblyDependencies");

            migrationBuilder.DropTable(
                name: "Dependencies");

            migrationBuilder.DropTable(
                name: "Assemblies");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "Scans");
        }
    }
}
