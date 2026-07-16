using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class GatewayFactory
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GatewayFactory> _logger;
    private readonly Dictionary<string, Type> _providerTypes;

    public GatewayFactory(IServiceScopeFactory scopeFactory, ILogger<GatewayFactory> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _providerTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
    }

    public void RegisterProvider<T>(string code) where T : IPaymentGatewayProvider
    {
        _providerTypes[code] = typeof(T);
        _logger.LogInformation("Registered payment provider: {Code} -> {Type}", code, typeof(T).Name);
    }

    public IPaymentGatewayProvider? GetProvider(string code)
    {
        if (!_providerTypes.TryGetValue(code, out var type))
        {
            _logger.LogWarning("No provider registered for code: {Code}", code);
            return null;
        }
        using var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService(type) as IPaymentGatewayProvider;
    }

    public IPaymentGatewayProvider? GetDefaultProvider()
    {
        if (_providerTypes.Count == 0) return null;
        var first = _providerTypes.First();
        return GetProvider(first.Key);
    }

    public List<IPaymentGatewayProvider> GetAllProviders()
    {
        var providers = new List<IPaymentGatewayProvider>();
        using var scope = _scopeFactory.CreateScope();
        foreach (var kvp in _providerTypes)
        {
            var provider = scope.ServiceProvider.GetRequiredService(kvp.Value) as IPaymentGatewayProvider;
            if (provider != null) providers.Add(provider);
        }
        return providers;
    }

    public List<string> GetRegisteredCodes()
    {
        return _providerTypes.Keys.ToList();
    }
}
