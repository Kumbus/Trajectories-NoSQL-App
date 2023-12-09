using Backend.Model;
using Backend.Model.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Repositories
{
    public interface IMongoRepository
    {
        Task<List<MongoTrajectory>> GetTrajectories(FilterDefinition<MongoTrajectory> filter);

        Task<List<AggregateQueryResult>> GetAggregations(FilterDefinition<MongoTrajectory> filter,
            ProjectionDefinition<MongoTrajectory, MongoTrajectory> groupBy, ProjectionDefinition<MongoTrajectory, AggregateQueryResult> project);

        Task<List<AggregateQueryResult>> GetUnwindedAggregations(FilterDefinition<MongoTrajectory> filter,
            ProjectionDefinition<BsonDocument, MongoTrajectory> groupBy, ProjectionDefinition<MongoTrajectory, AggregateQueryResult> project);

        Task<List<MongoTrajectory>> GetPoints(FilterDefinition<MongoTrajectory> filter);
        Task<MongoTrajectory> GetTrajectory(string id);
    }
}
