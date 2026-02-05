#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Provides detailed information about an Excel workbook structure including all sheets, tables, and columns.

.DESCRIPTION
    This script reads an Excel workbook and provides comprehensive information including:
    - List of all worksheets
    - Sheet-to-table mapping (if available)
    - Column names for each table in each sheet
    - Summary statistics
    Requires the ImportExcel PowerShell module.

.PARAMETER WorkbookPath
    Path to the Excel workbook file.

.PARAMETER DetailedOutput
    When specified, shows additional details like data types and sample values.

.PARAMETER MapSheetName
    Name of the sheet containing the sheet-to-table mapping (default: _SheetToTbleMap).

.PARAMETER MapTableName
    Name of the table containing the sheet-to-table mapping (default: tblSheetToTblNameMap).

.EXAMPLE
    .\Get-TableColumns.ps1
    
.EXAMPLE
    .\Get-TableColumns.ps1 -WorkbookPath ".\Data Operations Inventory Blank.xlsx"

.EXAMPLE
    .\Get-TableColumns.ps1 -WorkbookPath ".\Data Operations Inventory Blank.xlsx" -DetailedOutput
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$WorkbookPath = ".\Data Operations Inventory Blank.xlsx",
    
    [Parameter(Mandatory = $false)]
    [switch]$DetailedOutput,
    
    [Parameter(Mandatory = $false)]
    [string]$MapSheetName = "_SheetToTblNameMap",
    
    [Parameter(Mandatory = $false)]
    [string]$MapTableName = "tblSheetToTblNameMap"
)

# Function to display a section header
function Write-SectionHeader {
    param([string]$Title, [string]$Color = "Cyan")
    Write-Host ""
    Write-Host ("=" * 80) -ForegroundColor DarkGray
    Write-Host $Title -ForegroundColor $Color
    Write-Host ("=" * 80) -ForegroundColor DarkGray
}

# Function to display a subsection header
function Write-SubsectionHeader {
    param([string]$Title)
    Write-Host ""
    Write-Host ("─" * 80) -ForegroundColor DarkGray
    Write-Host $Title -ForegroundColor Yellow
    Write-Host ("─" * 80) -ForegroundColor DarkGray
}

# Check if ImportExcel module is installed
if (-not (Get-Module -ListAvailable -Name ImportExcel)) {
    Write-Host "ImportExcel module is not installed." -ForegroundColor Yellow
    Write-Host "Installing ImportExcel module..." -ForegroundColor Cyan
    try {
        Install-Module -Name ImportExcel -Scope CurrentUser -Force -AllowClobber
        Write-Host "ImportExcel module installed successfully." -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to install ImportExcel module. Error: $_"
        Write-Host "Please install it manually using: Install-Module -Name ImportExcel -Scope CurrentUser" -ForegroundColor Yellow
        exit 1
    }
}

# Import the module
Import-Module ImportExcel

# Resolve the full path
$fullPath = Resolve-Path -Path $WorkbookPath -ErrorAction SilentlyContinue

if (-not $fullPath) {
    Write-Error "Workbook file not found: $WorkbookPath"
    exit 1
}

try {
    Write-SectionHeader "EXCEL WORKBOOK ANALYSIS" "Green"
    Write-Host "File: $fullPath" -ForegroundColor White
    $fileInfo = Get-Item -Path $fullPath
    Write-Host "Size: $([math]::Round($fileInfo.Length / 1KB, 2)) KB" -ForegroundColor Gray
    Write-Host "Last Modified: $($fileInfo.LastWriteTime)" -ForegroundColor Gray
    
    # Get all worksheet names
    $excelPackage = Open-ExcelPackage -Path $fullPath
    $worksheetNames = $excelPackage.Workbook.Worksheets | ForEach-Object { $_.Name }
    Close-ExcelPackage $excelPackage -NoSave
    
    Write-Host ""
    Write-Host "Total Worksheets: $($worksheetNames.Count)" -ForegroundColor Cyan
    
    # Try to read the sheet-to-table mapping
    $sheetTableMap = @{}
    try {
        $mapData = Import-Excel -Path $fullPath -WorksheetName $MapSheetName -DataOnly -ErrorAction SilentlyContinue
        if ($mapData) {
            Write-Host "Found sheet-to-table mapping in '$MapSheetName'" -ForegroundColor Green
            foreach ($row in $mapData) {
                # Look for SheetName and TableName properties
                $sheetProp = $row.PSObject.Properties | Where-Object { $_.Name -eq "SheetName" }
                $tableProp = $row.PSObject.Properties | Where-Object { $_.Name -eq "TableName" }
                
                if ($sheetProp -and $tableProp) {
                    $sheetValue = $sheetProp.Value
                    $tableValue = $tableProp.Value
                    
                    if ($sheetValue -and $tableValue) {
                        $sheetTableMap[$sheetValue] = $tableValue
                        Write-Host "  Mapped: $sheetValue -> $tableValue" -ForegroundColor Gray
                    }
                }
            }
            Write-Host "Loaded $($sheetTableMap.Count) sheet-to-table mappings" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "No sheet-to-table mapping found (this is optional)" -ForegroundColor Gray
    }
    
    # Initialize summary statistics
    $totalTables = 0
    $totalColumns = 0
    $workbookStructure = @()
    
    # Process each worksheet
    Write-SectionHeader "WORKSHEET DETAILS" "Cyan"
    
    foreach ($sheetName in $worksheetNames) {
        Write-SubsectionHeader "Sheet: $sheetName"
        
        # Get table name from mapping or use generic name
        $tableName = if ($sheetTableMap.ContainsKey($sheetName)) {
            $sheetTableMap[$sheetName]
        } else {
            "(No table mapping)"
        }
        
        Write-Host "Table Name: " -NoNewline -ForegroundColor Gray
        Write-Host $tableName -ForegroundColor White
        
        try {
            # Read the Excel data from the sheet
            $sheetData = Import-Excel -Path $fullPath -WorksheetName $sheetName -DataOnly -ErrorAction Stop
            
            if ($null -eq $sheetData -or $sheetData.Count -eq 0) {
                Write-Host "Status: " -NoNewline -ForegroundColor Gray
                Write-Host "Empty or no data" -ForegroundColor Yellow
                continue
            }
            
            # Get column names
            $firstRow = $sheetData | Select-Object -First 1
            $columnNames = $firstRow.PSObject.Properties.Name
            
            $rowCount = if ($sheetData -is [array]) { $sheetData.Count } else { 1 }
            
            Write-Host "Rows: " -NoNewline -ForegroundColor Gray
            Write-Host $rowCount -ForegroundColor White
            Write-Host "Columns: " -NoNewline -ForegroundColor Gray
            Write-Host $columnNames.Count -ForegroundColor White
            
            Write-Host ""
            Write-Host "Column Names:" -ForegroundColor Green
            
            $columnIndex = 1
            foreach ($column in $columnNames) {
                Write-Host "  $columnIndex. " -NoNewline -ForegroundColor DarkGray
                Write-Host $column -ForegroundColor White
                
                # Show detailed info if requested
                if ($DetailedOutput) {
                    # Get sample values (exclude only null and empty strings)
                    $sampleValues = @($sheetData | 
                        ForEach-Object { $_.$column } | 
                        Where-Object { $null -ne $_ -and -not ($_ -is [string] -and $_ -eq "") } | 
                        Select-Object -First 3)
                    
                    if ($sampleValues.Count -gt 0) {
                        Write-Host "     Sample: " -NoNewline -ForegroundColor DarkGray
                        Write-Host ($sampleValues -join ", ") -ForegroundColor DarkYellow
                    }
                }
                
                $columnIndex++
            }
            
            # Update statistics
            $totalTables++
            $totalColumns += $columnNames.Count
            
            # Store structure for summary
            $workbookStructure += [PSCustomObject]@{
                SheetName = $sheetName
                TableName = $tableName
                Columns = $columnNames.Count
                Rows = $rowCount
                ColumnList = $columnNames
            }
        }
        catch {
            Write-Host "Status: " -NoNewline -ForegroundColor Gray
            Write-Host "Error reading sheet - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    # Display summary
    Write-SectionHeader "SUMMARY" "Green"
    Write-Host "Total Worksheets: " -NoNewline -ForegroundColor Gray
    Write-Host $worksheetNames.Count -ForegroundColor White
    Write-Host "Total Tables with Data: " -NoNewline -ForegroundColor Gray
    Write-Host $totalTables -ForegroundColor White
    Write-Host "Total Columns across all tables: " -NoNewline -ForegroundColor Gray
    Write-Host $totalColumns -ForegroundColor White
    
    # Show quick reference table
    Write-Host ""
    Write-Host "Quick Reference:" -ForegroundColor Cyan
    Write-Host ""
    $workbookStructure | Format-Table -Property SheetName, TableName, Columns, Rows -AutoSize
    
    Write-Host ""
    Write-Host ("=" * 80) -ForegroundColor DarkGray
    Write-Host "Analysis complete!" -ForegroundColor Green
    Write-Host ("=" * 80) -ForegroundColor DarkGray
    Write-Host ""
    
    # Return the structure object for further processing if needed
    return $workbookStructure
}
catch {
    Write-Error "Error reading Excel file: $_"
    Write-Error $_.Exception.Message
    Write-Error $_.ScriptStackTrace
    exit 1
}
