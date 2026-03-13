using Microsoft.Data.SqlClient;

namespace FileWatcherService;

public class DatabaseWorker : BackgroundService
{
    private readonly ILogger<DatabaseWorker> _logger;
    private readonly IConfiguration _configuration;

    public DatabaseWorker(ILogger<DatabaseWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string sourceConnection =
        "Server=localhost;Database=Mani;Trusted_Connection=True;TrustServerCertificate=True;";

        string targetConnection =
        "Server=localhost;Database=Mani;Trusted_Connection=True;TrustServerCertificate=True;";

        var tablesToMonitor = _configuration
            .GetSection("TablesToMonitor")
            .Get<List<string>>();

        if (tablesToMonitor == null || tablesToMonitor.Count == 0)
        {
            _logger.LogWarning("No tables configured for monitoring.");
            return;
        }

        _logger.LogInformation("Database monitoring service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var table in tablesToMonitor)
            {
                try
                {
                    await ProcessTable(sourceConnection, targetConnection, table);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing table {table}: {ex.Message}");
                }
            }

            await Task.Delay(10000, stoppingToken);
        }
    }

    private async Task ProcessTable(string sourceConn, string targetConn, string tableName)
    {
        using var sourceConnection = new SqlConnection(sourceConn);
        await sourceConnection.OpenAsync();

        string query = $"SELECT * FROM {tableName} WHERE Processed = 0";

        using var command = new SqlCommand(query, sourceConnection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int id = reader.GetInt32(0);

            _logger.LogInformation($"Processing row {id} from table {tableName}");

            await InsertIntoTarget(targetConn, tableName, id);

            await MarkAsProcessed(sourceConn, tableName, id);
        }
    }

    private async Task InsertIntoTarget(string connectionString, string tableName, int id)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string insertQuery =
        @"INSERT INTO ProcessedData (TableName, RowId, ProcessedTime)
          VALUES (@TableName, @RowId, GETDATE())";

        using var cmd = new SqlCommand(insertQuery, connection);

        cmd.Parameters.AddWithValue("@TableName", tableName);
        cmd.Parameters.AddWithValue("@RowId", id);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task MarkAsProcessed(string connectionString, string tableName, int id)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string updateQuery =
        $"UPDATE {tableName} SET Processed = 1 WHERE Id = @Id";

        using var cmd = new SqlCommand(updateQuery, connection);

        cmd.Parameters.AddWithValue("@Id", id);

        await cmd.ExecuteNonQueryAsync();
    }
}