using Backend.Config;
using Backend.Model;
using Backend.Model.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Repositories
{
    public class MongoRepository : IMongoRepository
    {
        private readonly IMongoCollection<MongoTrajectory> _trajectoryCollection;

        public MongoRepository(IOptions<MongoDbSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _trajectoryCollection = mongoDatabase.GetCollection<MongoTrajectory>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<AggregateQueryResult>> GetAggregations(FilterDefinition<MongoTrajectory> filter,
            ProjectionDefinition<MongoTrajectory, MongoTrajectory> groupBy, ProjectionDefinition<MongoTrajectory, AggregateQueryResult> project)
        {
            return await _trajectoryCollection.Aggregate().Match(filter).Group(groupBy).Project(project).ToListAsync();
        }

        public async Task<List<AggregateQueryResult>> GetUnwindedAggregations(FilterDefinition<MongoTrajectory> filter,
            ProjectionDefinition<BsonDocument, MongoTrajectory> groupBy, ProjectionDefinition<MongoTrajectory, AggregateQueryResult> project)
        {
            return await _trajectoryCollection.Aggregate().Match(filter).Unwind(t => t.Points).Group(groupBy).Project(project).ToListAsync();

        }

        public async Task<List<MongoTrajectory>> GetTrajectories(FilterDefinition<MongoTrajectory> filter)
        {
            return await _trajectoryCollection.Find(filter).Project(t => new MongoTrajectory
            {
                IdInFile = t.IdInFile,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                AverageSpeed = t.AverageSpeed,
                Duration = t.Duration,
                Length = t.Length,
                city = t.city,
                Weather = t.Weather,
                FuelPrice = t.FuelPrice,
                CountryPopulation = t.CountryPopulation,
                Economic = t.Economic,
                Emissions = t.Emissions,
            }).ToListAsync();
        }

        public async Task<List<MongoTrajectory>> GetPoints(FilterDefinition<MongoTrajectory> filter)
        {
            return await _trajectoryCollection.Find(filter).Project(t => new MongoTrajectory
            {
                Points = t.Points
            }).ToListAsync();
        }

        public async Task<MongoTrajectory> GetTrajectory(string id)
        {
            return await _trajectoryCollection.Find(t => t.IdInFile == id).FirstOrDefaultAsync();
        }
    }
}
