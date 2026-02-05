namespace DAO.Manager.Services;

public interface IProgressReporter
{
    Task ReportProgress(string phase, string message, int percentComplete);
    Task ReportComplete(bool success, string message);
    Task ReportError(string message);
}
