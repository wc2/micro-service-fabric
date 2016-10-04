using System;
using System.Threading.Tasks;
using System.Web.Http;
using CounterService.Api;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CounterWebApi.Controllers
{
    public class CounterController : ApiController
    {
        private readonly IQueryCounter _queryCounter;
        private readonly IIncrementCounter _incrementCounter;

        public CounterController()
        {
            var serviceUri = new Uri("fabric:/CounterApp/CounterService");
            var partitionKey = new ServicePartitionKey(0);

            _queryCounter = ServiceProxy.Create<IQueryCounter>(serviceUri, partitionKey);
            _incrementCounter = ServiceProxy.Create<IIncrementCounter>(serviceUri, partitionKey);
        }

        public async Task<IHttpActionResult> Get()
        {
            var counter = await _queryCounter.GetCounterAsync().ConfigureAwait(false);
            return Ok(counter);
        }

        public async Task<IHttpActionResult> Post([FromBody] int incrementCounterBy)
        {
            await _incrementCounter.IncrementCounterAsync(incrementCounterBy).ConfigureAwait(false);
            return Ok();
        }
    }
}
