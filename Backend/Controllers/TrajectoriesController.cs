using Backend.Model;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrajectoriesController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        private readonly ICosmosService _cosmosService;
        public TrajectoriesController(ICosmosService cosmosService, IMongoService mongoService)
        {
            _mongoService = mongoService;
            _cosmosService = cosmosService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSimpleQuery(SimpleQueryFilter query)
        {
            var mongoResult = await _mongoService.CreateSimpleQuery(query);
            var cosmosResult = await _cosmosService.CreateSimpleQuery(query);

            return Ok(new Tuple<FullSimpleQueryResult, FullSimpleQueryResult>(mongoResult, cosmosResult));

        }

        [HttpPost("aggregation")]
        public async Task<IActionResult> CreateAggregateQuery(AggregateQueryFilter query)
        {
            var mongoResult = await _mongoService.CreateAggregateQuery(query);
            var cosmosResult = await _cosmosService.CreateAggregateQuery(query);

            return Ok(new Tuple<FullAggregateQueryResult, FullAggregateQueryResult>(mongoResult, cosmosResult));

        }

        [HttpPost("points")]
        public async Task<IActionResult> GetPoints(SimpleQueryFilter query)
        {
            var points = await _mongoService.GetPoints(query);

            return Ok(points);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrajectory(string id)
        {
            var trajectory = await _mongoService.GetTrajectory(id);

            return Ok(trajectory);

        }
    }
}
