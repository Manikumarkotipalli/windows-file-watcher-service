using System.IO;
namespace FileWatcherService;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private FileSystemWatcher _watcher;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string path = @"C:\FileWatcher\Input";

        _watcher = new FileSystemWatcher(path);
        _watcher.Filter = "*.*";
        _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

        _watcher.Created += OnFileCreated;
        _watcher.EnableRaisingEvents = true;

        _logger.LogInformation("Watching folder: {path}", path);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation($"New file detected: {e.FullPath}");
        ProcessFile(e.FullPath);
    }

    private void ProcessFile(string filePath)
    {
        try
        {
            Thread.Sleep(500);

            var content = File.ReadAllText(filePath);
            _logger.LogInformation($"File content: {content}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing file: {ex.Message}");
        }
    }
}