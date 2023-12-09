using AutoMapper;
using Backend.Model;
using Backend.Model.Cosmos;
using Backend.Model.Mongo;
using Backend.Repositories;
using LinqKit;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using System.Diagnostics;

namespace Backend.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly ICosmosRepository _cosmosRepository;
        private readonly IMapper _mapper;
        public CosmosService(ICosmosRepository cosmosRepository, IMapper mapper) 
        { 
            _cosmosRepository = cosmosRepository;
            _mapper = mapper;
        }
        public async Task<FullSimpleQueryResult> CreateSimpleQuery(SimpleQueryFilter simpleQueryFilter)
        {
            var fullTimer = new Stopwatch();
            var queryTimer = new Stopwatch();
            fullTimer.Start();

            var predicate = CreateFilter(simpleQueryFilter);

            queryTimer.Start();
            var iterator = _cosmosRepository.GetTrajectories(predicate);
            

            List<CosmosTrajectory> trajectories = new();

            while (iterator.HasMoreResults)
            {
                FeedResponse<CosmosTrajectory> response = await iterator.ReadNextAsync();
                foreach (CosmosTrajectory trajectory in response)
                {
                    trajectories.Add(trajectory);
                }
            }
            queryTimer.Stop();
            var results = _mapper.Map<List<SimpleQueryResult>>(trajectories.Skip((simpleQueryFilter.Page - 1) * 100).Take(100).ToList());

            fullTimer.Stop();

            FullSimpleQueryResult result = new FullSimpleQueryResult
            {
                Name= "CosmosDB",
                Results = results,
                FullTime = fullTimer.ElapsedMilliseconds,
                QueryTime = queryTimer.ElapsedMilliseconds,
                Count = trajectories.Count
            };

            return result;
        }

        public async Task<FullAggregateQueryResult> CreateAggregateQuery(AggregateQueryFilter aggregateQueryFilter)
        {
            var fullTimer = new Stopwatch();
            var queryTimer = new Stopwatch();

            fullTimer.Start();
            
            var filter = CreateFilter(aggregateQueryFilter.SimpleQueryFilter ?? new SimpleQueryFilter());

            queryTimer.Start();
            var queryResults = _cosmosRepository.GetTrajectories(filter);
            

            var results = new List<CosmosTrajectory>();

            while (queryResults.HasMoreResults)
            {
                var response = await queryResults.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            aggregateQueryFilter.GroupFields = aggregateQueryFilter.GroupFields != null ? aggregateQueryFilter.GroupFields : new GroupFields();
            aggregateQueryFilter.Ranges = aggregateQueryFilter.Ranges != null ? aggregateQueryFilter.Ranges : new Ranges();

            var groupedResults = results.GroupBy(t => new
            {
                City = aggregateQueryFilter.GroupFields.GroupByCity == true ? t.city : null,
                WindDirection = aggregateQueryFilter.GroupFields.GroupByWindDirection == true ? t.Weather.WindDirection : null,
                StartDateYear = aggregateQueryFilter.GroupFields.GroupByStartDateYear == true ? (int?)t.StartDate.Year : null,
                StartDateMonth = aggregateQueryFilter.GroupFields.GroupByStartDateMonth == true ? (int?)t.StartDate.Month : null,
                StartDateDayOfMonth = aggregateQueryFilter.GroupFields.GroupByStartDateDayOfMonth == true ? (int?)t.StartDate.Day : null,
                StartDateDayOfWeek = aggregateQueryFilter.GroupFields.GroupByStartDateDayOfWeek == true ? (int?)t.StartDate.DayOfWeek: null,
                StartDateHour = aggregateQueryFilter.GroupFields.GroupByStartDateHour == true ? (int?)t.StartDate.Hour : null,
                EndDateYear = aggregateQueryFilter.GroupFields.GroupByEndDateYear == true ? (int?)t.EndDate.Year : null,
                EndDateMonth = aggregateQueryFilter.GroupFields.GroupByEndDateMonth == true ? (int?)t.EndDate.Month : null,
                EndDateDayOfMonth = aggregateQueryFilter.GroupFields.GroupByEndDateDayOfMonth == true ? (int?)t.EndDate.Day : null,
                EndDateDayOfWeek = aggregateQueryFilter.GroupFields.GroupByEndDateDayOfWeek == true ? (int?)t.EndDate.DayOfWeek : null,
                EndDateHour = aggregateQueryFilter.GroupFields.GroupByEndDateHour == true ? (int?)t.EndDate.Hour : null,
                //Region = aggregateQueryFilter.GroupFields.GroupByRegion == true ? t.Points.MaxBy(p => p.Region?.Name)?.Region?.Name : null,
                DurationRange = aggregateQueryFilter.GroupFields.GroupByDuration == true && aggregateQueryFilter.Ranges.DurationRanges != null ? GetRange(t.Duration, aggregateQueryFilter.Ranges.DurationRanges, "Duration") : null,
                LengthRange = aggregateQueryFilter.GroupFields.GroupByLength == true && aggregateQueryFilter.Ranges.LengthRanges != null ? GetRange(t.Length, aggregateQueryFilter.Ranges.LengthRanges, "Length") : null,
                AverageSpeedRange = aggregateQueryFilter.GroupFields.GroupByAverageSpeed == true && aggregateQueryFilter.Ranges.AverageSpeedRanges != null ? GetRange(t.AverageSpeed, aggregateQueryFilter.Ranges.AverageSpeedRanges, "Average Speed") : null,
                TemperatureRange = aggregateQueryFilter.GroupFields.GroupByTemperature == true && aggregateQueryFilter.Ranges.TemperatureRanges != null ? GetRange(t.Weather.Temperature, aggregateQueryFilter.Ranges.TemperatureRanges, "Temperature") : null,
                HumidityRange = aggregateQueryFilter.GroupFields.GroupByHumidity == true && aggregateQueryFilter.Ranges.HumidityRanges != null ? GetRange(t.Length, aggregateQueryFilter.Ranges.HumidityRanges, "Humidity") : null,
                WindSpeedRange = aggregateQueryFilter.GroupFields.GroupByWindSpeed == true && aggregateQueryFilter.Ranges.WindSpeedRanges != null ? GetRange(t.Length, aggregateQueryFilter.Ranges.WindSpeedRanges, "Wind Speed") : null,
                PrecipitationRange = aggregateQueryFilter.GroupFields.GroupByPrecipitation == true && aggregateQueryFilter.Ranges.PrecipitationRanges != null ? GetRange(t.Length, aggregateQueryFilter.Ranges.PrecipitationRanges, "Precipitation") : null,
                PrecipitationDurationRange = aggregateQueryFilter.GroupFields.GroupByPrecipitationDuration == true && aggregateQueryFilter.Ranges.PrecipitationDurationRanges != null ? GetRange(t.Length, aggregateQueryFilter.Ranges.PrecipitationDurationRanges, "Precipitation duration") : null,
                AirPressureRange = aggregateQueryFilter.GroupFields.GroupByAirPressure == true && aggregateQueryFilter.Ranges.AirPressureRanges != null ? GetRange(t.Length, aggregateQueryFilter.Ranges.AirPressureRanges, "Air pressure") : null,
                
            }).Select(group => new AggregateQueryResult
            {
                City = group.Key.City,
                WindDirection = group.Key.WindDirection,
                StartDateYear = group.Key.StartDateYear,
                StartDateMonth = group.Key.StartDateMonth,
                StartDateDayOfMonth = group.Key.StartDateDayOfMonth,
                StartDateDayOfWeek = group.Key.StartDateDayOfWeek + 1,
                StartDateHour = group.Key.StartDateHour,
                EndDateYear = group.Key.EndDateYear,
                EndDateMonth = group.Key.EndDateMonth,
                EndDateDayOfMonth = group.Key.EndDateDayOfMonth,
                EndDateDayOfWeek = group.Key.EndDateDayOfWeek + 1,
                EndDateHour = group.Key.EndDateHour,
                //Region = group.Key.Region,
                DurationRange = group.Key.DurationRange,
                LengthRange = group.Key.LengthRange,
                AverageSpeedRange = group.Key.AverageSpeedRange,
                TemperatureRange = group.Key.TemperatureRange,
                HumidityRange = group.Key.HumidityRange,
                WindSpeedRange = group.Key.WindSpeedRange,
                PrecipitationRange = group.Key.PrecipitationRange,
                PrecipitationDurationRange = group.Key.PrecipitationDurationRange,
                AirPressureRange = group.Key.AirPressureRange,
                Count = group.Count(),
                MinDuration = group.Min(item => item.Duration),
                MaxDuration = group.Max(item => item.Duration),
                AverageDuration = group.Average(item => item.Duration),
                MinLength = group.Min(item => item.Length),
                MaxLength = group.Max(item => item.Length),
                AverageLength = group.Average(item => item.Length),
                MinAverageSpeed = group.Min(item => item.AverageSpeed),
                MaxAverageSpeed = group.Max(item => item.AverageSpeed),
                AverageAverageSpeed = group.Average(item => item.AverageSpeed)
            }).ToList();
            queryTimer.Stop();
            fullTimer.Stop();

            FullAggregateQueryResult result = new FullAggregateQueryResult
            {
                Name = "CosmosDB",
                Results = groupedResults,
                FullTime = fullTimer.ElapsedMilliseconds,
                QueryTime = queryTimer.ElapsedMilliseconds,
                Count = groupedResults.Count()
            };

            return result;
        }

        private string GetRange(decimal? value, List<decimal> ranges, string name)
        {
            ranges.Sort();
            for (int i = 0; i < ranges.Count; i++) 
            {
                if (value < ranges[i])
                {
                    if (i == 0)
                        return $"{name} less than {ranges[i]}";

                    return $"{name} between {ranges[i - 1]}-{ranges[i]}";
                }
            }

            return $"{name} more than {ranges.Last()}";
        }

        public ExpressionStarter<CosmosTrajectory> CreateFilter(SimpleQueryFilter simpleQueryFilter)
        {
            var predicate = PredicateBuilder.New<CosmosTrajectory>(true);

            if (simpleQueryFilter.Cities != null && simpleQueryFilter.Cities.Count > 0)
                predicate = predicate.And(t => simpleQueryFilter.Cities.Contains(t.city));

            if (simpleQueryFilter.AverageSpeedFrom != null)
                predicate = predicate.And(t => t.AverageSpeed >= simpleQueryFilter.AverageSpeedFrom);

            if (simpleQueryFilter.AverageSpeedTo != null)
                predicate = predicate.And(t => t.AverageSpeed <= simpleQueryFilter.AverageSpeedTo);

            if (simpleQueryFilter.DurationFrom != null)
                predicate = predicate.And(t => t.Duration >= simpleQueryFilter.DurationFrom);

            if (simpleQueryFilter.DurationTo != null)
                predicate = predicate.And(t => t.Duration <= simpleQueryFilter.DurationTo);

            if (simpleQueryFilter.LengthFrom != null)
                predicate = predicate.And(t => t.Length >= simpleQueryFilter.LengthFrom);

            if (simpleQueryFilter.LengthTo != null)
                predicate = predicate.And(t => t.Length <= simpleQueryFilter.LengthTo);

            if (simpleQueryFilter.StartDateFrom != null)
                predicate = predicate.And(t => t.StartDate >= simpleQueryFilter.StartDateFrom);

            if (simpleQueryFilter.StartDateTo != null)
                predicate = predicate.And(t => t.StartDate <= simpleQueryFilter.StartDateTo);

            if (simpleQueryFilter.EndDateFrom != null)
                predicate = predicate.And(t => t.EndDate >= simpleQueryFilter.EndDateFrom);

            if (simpleQueryFilter.EndDateTo != null)
                predicate = predicate.And(t => t.EndDate <= simpleQueryFilter.EndDateTo);

            // Weather conditions
            if (simpleQueryFilter.WeatherFilter != null)
            {
                if (simpleQueryFilter.WeatherFilter.TemperatureFrom != null)
                    predicate = predicate.And(t => t.Weather.Temperature >= simpleQueryFilter.WeatherFilter.TemperatureFrom);

                if (simpleQueryFilter.WeatherFilter.TemperatureTo != null)
                    predicate = predicate.And(t => t.Weather.Temperature <= simpleQueryFilter.WeatherFilter.TemperatureTo);

                if (simpleQueryFilter.WeatherFilter.AirPressureFrom != null)
                    predicate = predicate.And(t => t.Weather.AirPressure >= simpleQueryFilter.WeatherFilter.AirPressureFrom);

                if (simpleQueryFilter.WeatherFilter.AirPressureTo != null)
                    predicate = predicate.And(t => t.Weather.AirPressure <= simpleQueryFilter.WeatherFilter.AirPressureTo);

                if (simpleQueryFilter.WeatherFilter.HumidityFrom != null)
                    predicate = predicate.And(t => t.Weather.Humidity >= simpleQueryFilter.WeatherFilter.HumidityFrom);

                if (simpleQueryFilter.WeatherFilter.HumidityTo != null)
                    predicate = predicate.And(t => t.Weather.Humidity <= simpleQueryFilter.WeatherFilter.HumidityTo);

                if (simpleQueryFilter.WeatherFilter.WindSpeedFrom != null)
                    predicate = predicate.And(t => t.Weather.WindSpeed >= simpleQueryFilter.WeatherFilter.WindSpeedFrom);

                if (simpleQueryFilter.WeatherFilter.WindSpeedTo != null)
                    predicate = predicate.And(t => t.Weather.WindSpeed <= simpleQueryFilter.WeatherFilter.WindSpeedTo);

                if (simpleQueryFilter.WeatherFilter.PrecipitationFrom != null)
                    predicate = predicate.And(t => t.Weather.Precipitation >= simpleQueryFilter.WeatherFilter.PrecipitationFrom);

                if (simpleQueryFilter.WeatherFilter.PrecipitationTo != null)
                    predicate = predicate.And(t => t.Weather.Precipitation <= simpleQueryFilter.WeatherFilter.PrecipitationTo);

                if (simpleQueryFilter.WeatherFilter.PrecipitationDurationFrom != null)
                    predicate = predicate.And(t => t.Weather.PrecipitationDuration >= simpleQueryFilter.WeatherFilter.PrecipitationDurationFrom);

                if (simpleQueryFilter.WeatherFilter.PrecipitationDurationTo != null)
                    predicate = predicate.And(t => t.Weather.PrecipitationDuration <= simpleQueryFilter.WeatherFilter.PrecipitationDurationTo);

                if (simpleQueryFilter.WeatherFilter.WindDirections != null && simpleQueryFilter.WeatherFilter.WindDirections.Count > 0)
                    predicate = predicate.And(t => t.Weather.WindDirection != null && simpleQueryFilter.WeatherFilter.WindDirections.Contains(t.Weather.WindDirection));
            }

            // PointWeather conditions
            if (simpleQueryFilter.PointFilter != null)
            {
                if (simpleQueryFilter.PointFilter.AreaFrom != null)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.Area >= simpleQueryFilter.PointFilter.AreaFrom));

                if (simpleQueryFilter.PointFilter.AreaTo != null)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.Area <= simpleQueryFilter.PointFilter.AreaTo));

                if (simpleQueryFilter.PointFilter.PopulationFrom != null)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.Population >= simpleQueryFilter.PointFilter.PopulationFrom));

                if (simpleQueryFilter.PointFilter.PopulationTo != null)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.Population <= simpleQueryFilter.PointFilter.PopulationTo));

                if (simpleQueryFilter.PointFilter.DensityFrom != null)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.Density >= simpleQueryFilter.PointFilter.DensityFrom));

                if (simpleQueryFilter.PointFilter.DensityTo != null)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.Density <= simpleQueryFilter.PointFilter.DensityTo));

                if (simpleQueryFilter.PointFilter.Names != null && simpleQueryFilter.PointFilter.Names.Count > 0)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && simpleQueryFilter.PointFilter.Names.Contains(p.Region.Name)));

                if (simpleQueryFilter.PointFilter.ParentNames != null && simpleQueryFilter.PointFilter.ParentNames.Count > 0)
                    predicate = predicate.And(t => t.Points.Any(p => p.Region != null && p.Region.ParentName != null && simpleQueryFilter.PointFilter.ParentNames.Contains(p.Region.ParentName)));
            }

            return predicate;
        }
    }
}
