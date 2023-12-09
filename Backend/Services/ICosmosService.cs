using Backend.Model;
using Backend.Model.Cosmos;

namespace Backend.Services
{
    public interface ICosmosService
    {
        Task<FullSimpleQueryResult> CreateSimpleQuery(SimpleQueryFilter simpleQueryFilter);
        Task<FullAggregateQueryResult> CreateAggregateQuery(AggregateQueryFilter aggregateQueryFilter);
    }
}
