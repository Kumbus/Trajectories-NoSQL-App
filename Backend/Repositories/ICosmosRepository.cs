using Backend.Model.Cosmos;
using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace Backend.Repositories
{
    public interface ICosmosRepository
    {
        FeedIterator<CosmosTrajectory> GetTrajectories(Expression<Func<CosmosTrajectory,bool>> expression);
    }
}
