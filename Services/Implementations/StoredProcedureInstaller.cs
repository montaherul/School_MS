using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Linq;

namespace SchoolManagementSystem.Services.Implementations
{
    /// <summary>
    /// Hosted service that automatically deploys all stored procedures found in the
    /// Data/StoredProcedures directory on application startup.
    /// </summary>
    public sealed class StoredProcedureInstaller : BackgroundService
    {
        private readonly ILogger<StoredProcedureInstaller> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostEnvironment _env;

        public StoredProcedureInstaller(
            ILogger<StoredProcedureInstaller> logger,
            IServiceProvider serviceProvider,
            IHostEnvironment env)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var startTime = Stopwatch.GetTimestamp();
            _logger.LogInformation("Starting deployment");

            var basePath = Path.Combine(_env.ContentRootPath, "Data", "StoredProcedures");
            if (!Directory.Exists(basePath))
            {
                _logger.LogWarning("Stored procedures directory not found: {BasePath}", basePath);
                return;
            }

            var sqlFiles = Directory.GetFiles(basePath, "*.sql", SearchOption.AllDirectories);
            var totalFiles = sqlFiles.Length;
            var executedCount = 0;
            var skippedCount = 0;
            var failedCount = 0;

            foreach (var file in sqlFiles)
            {
                if (stoppingToken.IsCancellationRequested) break;

                var relativePath = Path.GetRelativePath(basePath, file).Replace('\\', '/');

                string rawContent;
                try
                {
                    rawContent = await File.ReadAllTextAsync(file, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "File failed: {File}. Failed to read file.", relativePath);
                    failedCount++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(rawContent))
                {
                    _logger.LogInformation("Skipping empty or whitespace-only file: {File}", relativePath);
                    skippedCount++;
                    continue;
                }

                // Normalize line endings to LF to ensure consistent hash calculation across platforms
                var normalizedContent = rawContent.Replace("\r\n", "\n").Replace("\r", "\n");
                var parsedName = ParseProcedureName(normalizedContent, file);
                var currentHash = GenerateSHA256Hash(normalizedContent);

                bool shouldDeploy = true;
                StoredProcedureDeploymentHistory? latestDeployment = null;

                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

                    latestDeployment = await dbContext.StoredProcedureDeploymentHistories
                        .Where(h => h.FileName == relativePath)
                        .OrderByDescending(h => h.Id)
                        .FirstOrDefaultAsync(stoppingToken);

                    if (latestDeployment != null && 
                        latestDeployment.Status == "Success" && 
                        latestDeployment.Hash == currentHash)
                    {
                        shouldDeploy = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to check deployment history for {File}. Will attempt deployment anyway.", relativePath);
                }

                if (!shouldDeploy)
                {
                    _logger.LogInformation("Skipping file (hash unchanged): {File}", relativePath);
                    skippedCount++;
                    continue;
                }

                _logger.LogInformation("File executing: {File}", relativePath);

                // Prepare execution SQL by replacing CREATE PROCEDURE / CREATE PROC with CREATE OR ALTER ...
                var executionContent = Regex.Replace(
                    normalizedContent,
                    @"\bCREATE\s+(PROCEDURE|PROC)\b",
                    "CREATE OR ALTER $1",
                    RegexOptions.IgnoreCase);

                try
                {
                    await using var scope = _serviceProvider.CreateAsyncScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

                    // Split execution script by 'GO' statement
                    var batches = Regex.Split(
                        executionContent,
                        @"^\s*GO\s*$",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);

                    foreach (var batch in batches)
                    {
                        var sql = batch.Trim();
                        if (string.IsNullOrWhiteSpace(sql)) continue;

                        await dbContext.Database.ExecuteSqlRawAsync(sql, stoppingToken);
                    }

                    _logger.LogInformation("File success: {File}", relativePath);
                    executedCount++;

                    // Record successful deployment
                    var history = new StoredProcedureDeploymentHistory
                    {
                        ProcedureName = parsedName,
                        FileName = relativePath,
                        Hash = currentHash,
                        DeployedAt = DateTime.UtcNow,
                        Status = "Success",
                        ErrorMessage = null
                    };

                    dbContext.StoredProcedureDeploymentHistories.Add(history);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "File failed: {File}. Error: {Message}", relativePath, ex.Message);
                    failedCount++;

                    // Record failed deployment
                    try
                    {
                        await using var scope = _serviceProvider.CreateAsyncScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

                        var history = new StoredProcedureDeploymentHistory
                        {
                            ProcedureName = parsedName,
                            FileName = relativePath,
                            Hash = currentHash,
                            DeployedAt = DateTime.UtcNow,
                            Status = "Failed",
                            ErrorMessage = ex.ToString()
                        };

                        dbContext.StoredProcedureDeploymentHistories.Add(history);
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception dbEx)
                    {
                        _logger.LogError(dbEx, "Failed to save failed deployment history record for {File}", relativePath);
                    }
                }
            }

            var durationMs = (Stopwatch.GetTimestamp() - startTime) * 1000 / (double)Stopwatch.Frequency;

            // Log details as requested
            _logger.LogInformation("Total files processed: {Count}", totalFiles);
            _logger.LogInformation("Total success: {Count}", executedCount);
            _logger.LogInformation("Total failed: {Count}", failedCount);
            _logger.LogInformation("Execution duration: {Duration:N0} ms", durationMs);

            // Output at startup as requested
            _logger.LogInformation("Total SQL Files: {Total}", totalFiles);
            _logger.LogInformation("Executed: {Executed}", executedCount);
            _logger.LogInformation("Skipped: {Skipped}", skippedCount);
            _logger.LogInformation("Failed: {Failed}", failedCount);
            _logger.LogInformation("Duration: {Duration:N0} ms", durationMs);
        }

        private static string GenerateSHA256Hash(string content)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
        }

        private static string StripComments(string sql)
        {
            // Remove multi-line comments /* ... */
            var noMultiLine = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline);
            // Remove single-line comments -- ...
            var noComments = Regex.Replace(noMultiLine, @"--.*$", "", RegexOptions.Multiline);
            return noComments;
        }

        private static string ParseProcedureName(string content, string filePath)
        {
            try
            {
                var cleanContent = StripComments(content);
                var match = Regex.Match(
                    cleanContent,
                    @"\bCREATE\s+(?:OR\s+ALTER\s+)?(?:PROCEDURE|PROC)\s+(?:\[?[a-zA-Z0-9_]+\]?\.)?\[?([a-zA-Z0-9_]+)\]?",
                    RegexOptions.IgnoreCase);

                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }
            }
            catch
            {
                // Ignore exception during parsing and fall back
            }

            return Path.GetFileNameWithoutExtension(filePath);
        }
    }
}

