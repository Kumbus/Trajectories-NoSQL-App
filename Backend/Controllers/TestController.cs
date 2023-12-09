using Backend.Model;
using Backend.Model.Cosmos;
using Backend.Model.Mongo;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using Weather = Backend.Model.Cosmos.Weather;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMongoService mongoService;
        private readonly ICosmosService cosmosService;
        public TestController(ICosmosService cosmosService, IMongoService mongoService) 
        {
            this.mongoService = mongoService;
            this.cosmosService = cosmosService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateQuery(AggregateQueryFilter query)
        {
            //BsonClassMap.RegisterClassMap<MongoResult>(classMap =>
            //{
            //    classMap.AutoMap();
            //    classMap.MapMember(p => p.IdInFile);
            //    classMap.MapMember(p => p.StartDate);
            //    classMap.MapMember(p => p.EndDate);
            //    classMap.MapMember(p => p.AverageSpeed);
            //    classMap.MapMember(p => p.Duration);
            //});
            var watch = new Stopwatch();
            string connectionString = "mongodb://localhost:27017";

            // Create a MongoClient
            var mongoClient = new MongoClient(connectionString);

            // Access the database and collection
            var mongoDatabase = mongoClient.GetDatabase("Trajectories");
            //var collection = mongoDatabase.GetCollection<MongoResultTest>("Test");
            var collection = mongoDatabase.GetCollection<MongoTrajectory>("Trajectory");
            //var groupFields = new List<ProjectionDefinition<MongoTrajectory>>();
            //groupFields.Add(Builders<MongoTrajectory>.Projection.Include(x => x.city));
            //groupFields.Add(Builders<MongoTrajectory>.Projection.Include(x => x.Weather.WindDirection));

            var groupFields = new BsonDocument();
            var groupBy = new BsonDocument();

            groupFields.Add("City", "$city");
            groupFields.Add("WindDirection", $"$Weather.WindDirection");

            groupBy.Add("_id", groupFields);

            groupBy.Add("count", new BsonDocument("$sum", 1));

            var project = new BsonDocument
            {
                 //Exclude the _id field from the result
                { "_id", 0 }, // Replace with the actual field you want to return
                { "City", "$_id.City" }, // Replace with the actual field you want to return
                { "WindDirection", "$_id.WindDirection" }, // Replace with the actual field you want to return
                { "Count", "$count" },
            };
            //Expression<Func<MongoTrajectory, MongoGroupModel>> a 
            //var aggregate = collection.Aggregate().Match(mongoService.CreateFilter(query.SimpleQueryFilter)).Group<MongoTrajectory>(groupBy).Project<MongoAggregateResult>(project);


            //aggregate = aggregate.Group(Builders<MongoTrajectory>.Projection.Combine(groupFields), group => new MongoGroupModel
            //{
            //    City = group.Key,
            //    Count = group.Count(),
            //    // Add other group calculations as needed
            //});
            // Match Stage
            //var a = aggregate.Match(filter);

            //aggregate.AppendStage(a);
            //// Add other match conditions based on your filter

            //// Group Stage
            //var b = aggregate.Group(x => x.city, group => new MongoGroupModel
            //{
            //    City = group.Key,
            //    Count = group.Count(),
            //    // Add other group calculations as needed
            //});



            // Project Stage
            //aggregate = (IAggregateFluent<MongoTrajectory>)aggregate.Project(result => new SimpleQueryResult
            //{
            //    City = result.city// Replace with the actual field you want to return
            //    //Count = result.YourField, // Example: Include the count field
            //                          // Add other fields you want to return in the result
            //});

            // Execute the aggregation pipeline
            //var result = await aggregate.ToListAsync();

            string endpoint = "https://localhost:8081";
            string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            // Create a Cosmos DB client
            using var cosmosClient = new CosmosClient(endpoint, key);

            // Access the database and container
            var cosmosDatabase = cosmosClient.GetDatabase("Trajectories");
            var container = cosmosDatabase.GetContainer("Trajectories");


            await container.DeleteItemAsync<CosmosTrajectory>("000-20090403011657", new PartitionKey("city"));
            // SQL-like query with GROUP BY
            var sqlQueryText = "SELECT DISTINCT c.IdInFile AS TrajectoryId, c.Duration, c.Length, c.StartDate, c.EndDate FROM c JOIN p IN c.Points WHERE c.city = 'Hannover' AND p.Region.Density < 1000";

            // Set the query definition
            var queryDefinition = new QueryDefinition(sqlQueryText);

            // Execute the query

            watch = new Stopwatch();
            watch.Start();
            //var queryResultSetIterator = container.GetItemQueryIterator<ResultCosmos>(queryDefinition);

            // Retrieve the result
            //container.all
            var queryCosmos = container.GetItemLinqQueryable<CosmosTrajectory>(true).Where(t => (t.city == "Hannover" || t.city=="Beijing") && t.AverageSpeed!= null && t.CountryPopulation.Density!=null).Select(t=>new CosmosTrajectory {city= t.city,Weather = t.Weather }).ToFeedIterator();

            var results = new List<CosmosTrajectory>();

            while (queryCosmos.HasMoreResults)
            {
                var response = await queryCosmos.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            List<Expression> expressions = new List<Expression>();
            ParameterExpression parameter = Expression.Parameter(typeof(CosmosTrajectory), "item");
            var returnValue = Expression.New(typeof(Test1));


            //var initValues = from p in typeof(Test1).GetProperties()
            //                 where p.HasAttribute()
            //                 let value = Expression.Property(parameter, p)
            //                 select Expression.Bind(p, value);

            var initValue = Expression.Bind(typeof(Test1).GetProperty("City"), Expression.Property(parameter, typeof(CosmosTrajectory).GetProperty("city")));

            var body =Expression.MemberInit(returnValue, initValue);

            var propLambda = Expression.Lambda(body, parameter);

            //Expression keyExpression = parameter;
            //ConstructorInfo property = typeof(Test1).GetConstructor;
            //PropertyInfo property = typeof(Test1).GetProperty("City");
            //keyExpression = Expression.Property(parameter, property);
            ////expressions.Add(keyExpression);
            ////property = typeof(Test1).GetProperty("WindDirection");
            ////keyExpression = Expression.Property(parameter, property);
            ////expressions.Add(keyExpression);
            //Test1 test1 = new Test1(;
            //LambdaExpression lambda = Expression.Lambda(typeof(Func<Test1, object>), keyExpression, parameter);


            var groupedResults = results.GroupBy(item => new 
            {
                City = item.city,
                WindDirection = query.GroupFields.GroupByWindDirection.Value ? item.Weather.WindDirection : null,
            })
                /*(Func<CosmosTrajectory, Test1>)propLambda.Compile()*///)
            .Select(group => new Test2
             {
                 City = group.Key.City,
                 WindDirection = group.Key.WindDirection,
                 Count = group.Count(),
                 MinDuration = group.Min(item => item.Duration),
                 MaxDuration = group.Max(item => item.Duration),
                 AvgDuration = group.Average(item => item.Duration),
             }).ToList();

            //foreach (var result in groupedResults)
            //{
            //    //Console.WriteLine($"City: {result.City}, WindDirection: {result.WindDirection}, Year: {result.Year}");
            //    Console.WriteLine($"Count: {result.Count}");
            //    Console.WriteLine($"Duration - Min: {result.MinDuration}, Max: {result.MaxDuration}, Avg: {result.AvgDuration}");
            //    Console.WriteLine("------------------------------------------------");
            //}
            

            return Ok(groupedResults);

        }

        public class Test1
        {
            //public Test1(string city, string windDirection, int startDateYar) 
            //{
            //    City = city;
            //    WindDirection = windDirection;
            //    StartDateYar = startDateYar;
            //}
            public string? City { get; set; }
            public string WindDirection { get; set; }
            public int StartDateYar { get; set; }
        }

        public class Test2
        {
            public string? City { get; set; }
            public string WindDirection { get; set; }
            public int Year { get; set; }
            public int Count { get; set; }
            public decimal MinDuration { get; set; }
            public decimal MaxDuration { get; set; }
            public decimal AvgDuration { get; set; }

        }
    }
}



//foreach (var result in queryCosmos.ToList())
//{
//    Console.WriteLine($"City: {result.City}, WindDirection: {result.WindDirection}, Year: {result.Year}");
//    Console.WriteLine($"Count: {result.Count}");
//    Console.WriteLine($"Duration - Min: {result.MinDuration}, Max: {result.MaxDuration}, Avg: {result.AvgDuration}");
//    //Console.WriteLine($"Length - Min: {result.MinLength}, Max: {result.MaxLength}, Avg: {result.AvgLength}");
//    //Console.WriteLine($"Speed - Min: {result.MinSpeed}, Max: {result.MaxSpeed}, Avg: {result.AvgSpeed}");
//    Console.WriteLine("------------------------------------------------");
//}

//List<dynamic> items = new();

//while (queryCosmos.HasMoreResults)
//{
//    var response = await queryCosmos.ReadNextAsync();
//    foreach (var item in response)
//    {
//        items.Add(item);
//    }
//}
//watch.Stop();
