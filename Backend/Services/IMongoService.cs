using Backend.Model;
using Backend.Model.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Services
{
    public interface IMongoService
    {
        Task<FullSimpleQueryResult> CreateSimpleQuery(SimpleQueryFilter simpleQueryFilter);
        Task<FullAggregateQueryResult> CreateAggregateQuery(AggregateQueryFilter aggregateQueryFilter);
        FilterDefinition<MongoTrajectory> CreateFilter(SimpleQueryFilter simpleQueryFilter);

        Task<List<List<decimal>>> GetPoints(SimpleQueryFilter filter);
        public Task<MongoTrajectory> GetTrajectory(string id);
    }
}
