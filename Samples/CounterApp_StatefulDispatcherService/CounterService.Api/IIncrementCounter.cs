using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CounterService.Api
{
    public interface IIncrementCounter : IService
    {
        Task IncrementCounterAsync(int incrementBy);
    }
}
