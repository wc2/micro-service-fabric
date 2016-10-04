using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CounterService.Api
{
    public interface IQueryCounter : IService
    {
        Task<int> GetCounterAsync();
    }
}