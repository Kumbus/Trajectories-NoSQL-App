using Backend.Config;
using Backend.Model.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Backend.Repositories
{
    public class CosmosRepository : ICosmosRepository
    {
        private readonly Container container;
        public CosmosRepository(IOptions<CosmosDbSettings> databaseSettings)
        {
            var cosmosClient = new CosmosClient(databaseSettings.Value.ConnectionString, databaseSettings.Value.Key);
            container = cosmosClient.GetContainer(databaseSettings.Value.DatabaseName, databaseSettings.Value.ContainerName);
        }
        public FeedIterator<CosmosTrajectory> GetTrajectories(Expression<Func<CosmosTrajectory, bool>> query)
        {
            IOrderedQueryable<CosmosTrajectory> queryable = container.GetItemLinqQueryable<CosmosTrajectory>();
            var matches = queryable.Where(query).Select(t => new CosmosTrajectory
            {
                IdInFile = t.IdInFile,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                AverageSpeed = t.AverageSpeed,
                Duration = t.Duration,
                Length  = t.Length,
                city = t.city,
                Weather = t.Weather,
                FuelPrice = t.FuelPrice,
                CountryPopulation = t.CountryPopulation,
                Economic = t.Economic,
                Emissions = t.Emissions
            });
            return matches.ToFeedIterator();
        }
    }
}
