using System.Collections.Concurrent;

namespace SchoolManagementSystem.Services.Implementations.Routine;

public class RoutineGenerationQueue
{
    private readonly ConcurrentQueue<(int AcademicYearId, string CreatedBy)> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void Enqueue(int academicYearId, string createdBy)
    {
        _queue.Enqueue((academicYearId, createdBy));
        _signal.Release();
    }

    public async Task<(int AcademicYearId, string CreatedBy)?> DequeueAsync(CancellationToken ct)
    {
        await _signal.WaitAsync(ct);
        if (_queue.TryDequeue(out var item))
            return item;
        return null;
    }
}
