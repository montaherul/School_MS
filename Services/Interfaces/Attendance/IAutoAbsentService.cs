using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.Entities.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface IAutoAbsentService
    {
        Task<AutoAbsentExecutionLog> RunForDateAsync(DateTime targetDate, string executedBy = "system", CancellationToken ct = default);
        Task<AutoAbsentExecutionLog?> RunForTodayAsync(string executedBy = "system", CancellationToken ct = default);
        Task<List<AutoAbsentExecutionLog>> GetRecentExecutionsAsync(int count, CancellationToken ct = default);
        Task<AutoAbsentExecutionLog?> GetLastExecutionAsync(CancellationToken ct = default);
    }
}
