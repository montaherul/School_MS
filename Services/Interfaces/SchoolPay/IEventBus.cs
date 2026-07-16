namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
    void Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : class;
}
