using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces
{
    public interface IStoredProcedureInstaller
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
