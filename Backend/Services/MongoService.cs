using AutoMapper;
using Backend.Model;
using Backend.Model.Mongo;
using Backend.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace Backend.Services
{
    public class MongoService : IMongoService
    {
        private readonly IMongoRepository _mongoRepository;
        private readonly IMapper _mapper;
        public MongoService(IMongoRepository mongoRepository, IMapper mapper) 
        {
            _mongoRepository = mongoRepository;
            _mapper = mapper;
        }

        public async Task<FullSimpleQueryResult> CreateSimpleQuery(SimpleQueryFilter simpleQueryFilter)
        {
            var fullTimer = new Stopwatch();
            var queryTimer = new Stopwatch();
            fullTimer.Start();

            var filter = CreateFilter(simpleQueryFilter);

            queryTimer.Start();
            var trajectories = await _mongoRepository.GetTrajectories(filter);
            queryTimer.Stop();

            var results = _mapper.Map<List<SimpleQueryResult>>(trajectories.Skip((simpleQueryFilter.Page - 1) * 100).Take(100).ToList());

            fullTimer.Stop();

            FullSimpleQueryResult result = new FullSimpleQueryResult
            {
                Name = "MongoDB",
                Results = results,
                FullTime = fullTimer.ElapsedMilliseconds,
                QueryTime = queryTimer.ElapsedMilliseconds,
                Count = trajectories.Count,
            };

            return result;

        }


        public async Task<FullAggregateQueryResult> CreateAggregateQuery(AggregateQueryFilter aggregateQueryFilter)
        {
            var fullTimer = new Stopwatch();
            var queryTimer = new Stopwatch();
            var results = new List<AggregateQueryResult>();

            fullTimer.Start();
            var filter = CreateFilter(aggregateQueryFilter.SimpleQueryFilter ?? new SimpleQueryFilter());
            var groupBy = CreateGroupBy(aggregateQueryFilter.GroupFields ?? new GroupFields(), aggregateQueryFilter.Ranges ?? new Ranges());
            var project = CreateProject(aggregateQueryFilter.GroupFields ?? new GroupFields(), aggregateQueryFilter.Ranges ?? new Ranges());
            var makeUnwind = CheckUnwind(aggregateQueryFilter.GroupFields ?? new GroupFields(), aggregateQueryFilter.Ranges ?? new Ranges());

            
            if (makeUnwind)
            {
                queryTimer.Start();
                results = await _mongoRepository.GetUnwindedAggregations(filter, groupBy, project);
                queryTimer.Stop();
            }
            else
            {
                queryTimer.Start();
                results = await _mongoRepository.GetAggregations(filter, groupBy, project);
                queryTimer.Stop();
            }

            fullTimer.Stop();

            FullAggregateQueryResult result = new FullAggregateQueryResult
            {
                Name = "MongoDB",
                Results = results,
                FullTime = fullTimer.ElapsedMilliseconds,
                QueryTime = queryTimer.ElapsedMilliseconds,
                Count = results.Count
            };

            return result;
        }

        public async Task<List<List<decimal>>> GetPoints(SimpleQueryFilter filter)
        {
            var results =  await _mongoRepository.GetPoints(CreateFilter(filter));
            var points = new List<List<decimal>>();

            foreach (var trajectory in results)
            {
                foreach (var point in trajectory.Points)
                {
                    points.Add(new List<decimal>() { point.Latitude, point.Longitude });
                }
            }

            return points;
        }
        public async Task<MongoTrajectory> GetTrajectory(string id)
        {
            var result = await _mongoRepository.GetTrajectory(id);

            return result;
        }
        public FilterDefinition<MongoTrajectory> CreateFilter(SimpleQueryFilter simpleQueryFilter)
        {
            var filterBuilder = Builders<MongoTrajectory>.Filter;
            var filter = filterBuilder.Empty;

            if (simpleQueryFilter.Cities != null && simpleQueryFilter.Cities.Count > 0)
                filter &= filterBuilder.In(t => t.city, simpleQueryFilter.Cities);

            if (simpleQueryFilter.AverageSpeedFrom != null)
                filter &= filterBuilder.Gte(t => t.AverageSpeed, simpleQueryFilter.AverageSpeedFrom);

            if (simpleQueryFilter.AverageSpeedTo != null)
                filter &= filterBuilder.Lte(t => t.AverageSpeed, simpleQueryFilter.AverageSpeedTo);

            if (simpleQueryFilter.DurationFrom != null)
                filter &= filterBuilder.Gte(t => t.Duration, simpleQueryFilter.DurationFrom);

            if (simpleQueryFilter.DurationTo != null)
                filter &= filterBuilder.Lte(t => t.Duration, simpleQueryFilter.DurationTo);

            if (simpleQueryFilter.LengthFrom != null)
                filter &= filterBuilder.Gte(t => t.Length, simpleQueryFilter.LengthFrom);

            if (simpleQueryFilter.LengthTo != null)
                filter &= filterBuilder.Lte(t => t.Length, simpleQueryFilter.LengthTo);

            if (simpleQueryFilter.StartDateFrom != null)
                filter &= filterBuilder.Gte(t => t.StartDate, simpleQueryFilter.StartDateFrom);

            if (simpleQueryFilter.StartDateTo != null)
                filter &= filterBuilder.Lte(t => t.StartDate, simpleQueryFilter.StartDateTo);

            if (simpleQueryFilter.EndDateFrom != null)
                filter &= filterBuilder.Gte(t => t.EndDate, simpleQueryFilter.EndDateFrom);

            if (simpleQueryFilter.EndDateTo != null)
                filter &= filterBuilder.Lte(t => t.EndDate, simpleQueryFilter.EndDateTo);

            // WeatherFilter conditions
            if (simpleQueryFilter.WeatherFilter != null)
            {
                if (simpleQueryFilter.WeatherFilter.TemperatureFrom != null)
                    filter &= filterBuilder.Gte(t => t.Weather.Temperature, simpleQueryFilter.WeatherFilter.TemperatureFrom);

                if (simpleQueryFilter.WeatherFilter.TemperatureTo != null)
                    filter &= filterBuilder.Lte(t => t.Weather.Temperature, simpleQueryFilter.WeatherFilter.TemperatureTo);

                if (simpleQueryFilter.WeatherFilter.AirPressureFrom != null)
                    filter &= filterBuilder.Gte(t => t.Weather.AirPressure, simpleQueryFilter.WeatherFilter.AirPressureFrom);

                if (simpleQueryFilter.WeatherFilter.AirPressureTo != null)
                    filter &= filterBuilder.Lte(t => t.Weather.AirPressure, simpleQueryFilter.WeatherFilter.AirPressureTo);

                if (simpleQueryFilter.WeatherFilter.HumidityFrom != null)
                    filter &= filterBuilder.Gte(t => t.Weather.Humidity, simpleQueryFilter.WeatherFilter.HumidityFrom);

                if (simpleQueryFilter.WeatherFilter.HumidityTo != null)
                    filter &= filterBuilder.Lte(t => t.Weather.Humidity, simpleQueryFilter.WeatherFilter.HumidityTo);

                if (simpleQueryFilter.WeatherFilter.WindSpeedFrom != null)
                    filter &= filterBuilder.Gte(t => t.Weather.WindSpeed, simpleQueryFilter.WeatherFilter.WindSpeedFrom);

                if (simpleQueryFilter.WeatherFilter.WindSpeedTo != null)
                    filter &= filterBuilder.Lte(t => t.Weather.WindSpeed, simpleQueryFilter.WeatherFilter.WindSpeedTo);

                if (simpleQueryFilter.WeatherFilter.PrecipitationFrom != null)
                    filter &= filterBuilder.Gte(t => t.Weather.Precipitation, simpleQueryFilter.WeatherFilter.PrecipitationFrom);

                if (simpleQueryFilter.WeatherFilter.PrecipitationTo != null)
                    filter &= filterBuilder.Lte(t => t.Weather.Precipitation, simpleQueryFilter.WeatherFilter.PrecipitationTo);

                if (simpleQueryFilter.WeatherFilter.PrecipitationDurationFrom != null)
                    filter &= filterBuilder.Gte(t => t.Weather.PrecipitationDuration, simpleQueryFilter.WeatherFilter.PrecipitationDurationFrom);

                if (simpleQueryFilter.WeatherFilter.PrecipitationDurationTo != null)
                    filter &= filterBuilder.Lte(t => t.Weather.PrecipitationDuration, simpleQueryFilter.WeatherFilter.PrecipitationDurationTo);

                if (simpleQueryFilter.WeatherFilter.WindDirections != null && simpleQueryFilter.WeatherFilter.WindDirections.Count > 0)
                    filter &= filterBuilder.In(t => t.Weather.WindDirection, simpleQueryFilter.WeatherFilter.WindDirections);
            }

            // PointFilter conditions
            if (simpleQueryFilter.PointFilter != null)
            {
                if (simpleQueryFilter.PointFilter.AreaFrom != null)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && p.Region.Area >= simpleQueryFilter.PointFilter.AreaFrom));

                if (simpleQueryFilter.PointFilter.AreaTo != null)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && p.Region.Area <= simpleQueryFilter.PointFilter.AreaTo));

                if (simpleQueryFilter.PointFilter.PopulationFrom != null)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && p.Region.Population >= simpleQueryFilter.PointFilter.PopulationFrom));

                if (simpleQueryFilter.PointFilter.PopulationTo != null)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && p.Region.Population <= simpleQueryFilter.PointFilter.PopulationTo));

                if (simpleQueryFilter.PointFilter.DensityFrom != null)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && p.Region.Density >= simpleQueryFilter.PointFilter.DensityFrom));

                if (simpleQueryFilter.PointFilter.DensityTo != null)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && p.Region.Density <= simpleQueryFilter.PointFilter.DensityTo));

                if (simpleQueryFilter.PointFilter.Names != null && simpleQueryFilter.PointFilter.Names.Count > 0)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && simpleQueryFilter.PointFilter.Names.Contains(p.Region.Name)));

                if (simpleQueryFilter.PointFilter.ParentNames != null && simpleQueryFilter.PointFilter.ParentNames.Count > 0)
                    filter &= filterBuilder.Where(t => t.Points.Any(p => p.Region != null && simpleQueryFilter.PointFilter.ParentNames.Contains(p.Region.ParentName)));
            }

            return filter;
        }

        private bool CheckUnwind(GroupFields groupFields, Ranges ranges)
        {
            if (groupFields.GroupByRegion == true || groupFields.GroupByParentRegion == true ||
                (groupFields.GroupByRegionArea == true && ranges.RegionAreaRanges != null && ranges.RegionAreaRanges.Count > 0) ||
                (groupFields.GroupByRegionPopulation == true && ranges.RegionPopulationRanges != null && ranges.RegionPopulationRanges.Count > 0) ||
                (groupFields.GroupByRegionDensity == true && ranges.RegionDensityRanges != null && ranges.RegionDensityRanges.Count > 0))
            {
                return true;
            }
            else
                return false;
        }

        private BsonDocument CreateGroupBy(GroupFields groupFields, Ranges ranges)
        {
            var groupFieldsDocument = new BsonDocument();
            var groupByDocument = new BsonDocument();


            if(groupFields.GroupByCity == true)
                groupFieldsDocument.Add("City", $"${nameof(MongoTrajectory.city)}");

            if (groupFields.GroupByStartDateYear == true)
                groupFieldsDocument.Add("StartDateYear", new BsonDocument("$year", $"${nameof(MongoTrajectory.StartDate)}"));

            if (groupFields.GroupByStartDateMonth == true)
                groupFieldsDocument.Add("StartDateMonth", new BsonDocument("$month", $"${nameof(MongoTrajectory.StartDate)}"));

            if (groupFields.GroupByStartDateDayOfMonth == true)
                groupFieldsDocument.Add("StartDateDayOfMonth", new BsonDocument("$dayOfMonth", $"${nameof(MongoTrajectory.StartDate)}"));

            if (groupFields.GroupByStartDateHour == true)
                groupFieldsDocument.Add("StartDateHour", new BsonDocument("$hour", $"${nameof(MongoTrajectory.StartDate)}"));

            if (groupFields.GroupByStartDateDayOfWeek == true)
                groupFieldsDocument.Add("StartDateDayOfWeek", new BsonDocument("$dayOfWeek", $"${nameof(MongoTrajectory.StartDate)}"));

            if (groupFields.GroupByEndDateYear == true)
                groupFieldsDocument.Add("EndDateYear", new BsonDocument("$year", $"${nameof(MongoTrajectory.EndDate)}"));

            if (groupFields.GroupByEndDateMonth == true)
                groupFieldsDocument.Add("EndDateMonth", new BsonDocument("$month", $"${nameof(MongoTrajectory.EndDate)}"));

            if (groupFields.GroupByEndDateDayOfMonth == true)
                groupFieldsDocument.Add("EndDateDayOfMonth", new BsonDocument("$dayOfMonth", $"${nameof(MongoTrajectory.EndDate)}"));

            if (groupFields.GroupByEndDateHour == true)
                groupFieldsDocument.Add("EndDateHour", new BsonDocument("$hour", $"${nameof(MongoTrajectory.EndDate)}"));

            if (groupFields.GroupByEndDateDayOfWeek == true)
                groupFieldsDocument.Add("EndDateDayOfWeek", new BsonDocument("$dayOfWeek", $"${nameof(MongoTrajectory.EndDate)}"));

            if (groupFields.GroupByWindDirection == true)
                groupFieldsDocument.Add("WindDirection", $"${nameof(MongoTrajectory.Weather)}.{nameof(MongoTrajectory.Weather.WindDirection)}");

            if (groupFields.GroupByAverageSpeed == true && ranges.AverageSpeedRanges != null && ranges.AverageSpeedRanges.Count > 0)
                groupFieldsDocument.Add("AverageSpeed", CreateRangeGroupBy(ranges.AverageSpeedRanges, "AverageSpeed", "AverageSpeed"));   

            if (groupFields.GroupByDuration == true && ranges.DurationRanges != null && ranges.DurationRanges.Count > 0)
                groupFieldsDocument.Add("Duration", CreateRangeGroupBy(ranges.DurationRanges, "Duration", "Duration"));

            if (groupFields.GroupByLength == true && ranges.LengthRanges != null && ranges.LengthRanges.Count > 0)
                groupFieldsDocument.Add("Length", CreateRangeGroupBy(ranges.LengthRanges, "Length", "Length"));

            if (groupFields.GroupByTemperature == true && ranges.TemperatureRanges != null && ranges.TemperatureRanges.Count > 0)
                groupFieldsDocument.Add("Temperature", CreateRangeGroupBy(ranges.TemperatureRanges, "Weather.Temperature", "Temperature"));

            if (groupFields.GroupByHumidity == true && ranges.HumidityRanges != null && ranges.HumidityRanges.Count > 0)
                groupFieldsDocument.Add("Humidity", CreateRangeGroupBy(ranges.HumidityRanges, "Weather.Humidity", "Humidity"));

            if (groupFields.GroupByWindSpeed == true && ranges.WindSpeedRanges != null && ranges.WindSpeedRanges.Count > 0)
                groupFieldsDocument.Add("WindSpeed", CreateRangeGroupBy(ranges.WindSpeedRanges, "Weather.WindSpeed", "WindSpeed"));

            if (groupFields.GroupByPrecipitation == true && ranges.PrecipitationRanges != null && ranges.PrecipitationRanges.Count > 0)
                groupFieldsDocument.Add("Precipitation", CreateRangeGroupBy(ranges.PrecipitationRanges, "Weather.Precipitation", "Precipitation"));

            if (groupFields.GroupByPrecipitationDuration == true && ranges.PrecipitationDurationRanges != null && ranges.PrecipitationDurationRanges.Count > 0)
                groupFieldsDocument.Add("PrecipitationDuration", CreateRangeGroupBy(ranges.PrecipitationDurationRanges, "Weather.PrecipitationDuration", "PrecipitationDuration"));

            if (groupFields.GroupByAirPressure == true && ranges.AirPressureRanges != null && ranges.AirPressureRanges.Count > 0)
                groupFieldsDocument.Add("AirPressure", CreateRangeGroupBy(ranges.AirPressureRanges, "Weather.AirPressure", "AirPressure"));

            if (groupFields.GroupByRegion == true)
                groupFieldsDocument.Add("Region", "$Points.Region.Name");

            if (groupFields.GroupByParentRegion == true)
                groupFieldsDocument.Add("ParentRegion", "$Points.Region.ParentName");

            if (groupFields.GroupByRegionArea == true && ranges.RegionAreaRanges != null && ranges.RegionAreaRanges.Count > 0)
                groupFieldsDocument.Add("RegionArea", CreateRangeGroupBy(ranges.RegionAreaRanges, "Points.Region.Area", "RegionArea"));

            if (groupFields.GroupByRegionPopulation == true && ranges.RegionPopulationRanges != null && ranges.RegionPopulationRanges.Count > 0)
                groupFieldsDocument.Add("RegionPopulation", CreateRangeGroupBy(ranges.RegionPopulationRanges, "Points.Region.Population", "RegionPopulation"));

            if (groupFields.GroupByRegionDensity == true && ranges.RegionDensityRanges != null && ranges.RegionDensityRanges.Count > 0)
                groupFieldsDocument.Add("RegionDensity", CreateRangeGroupBy(ranges.RegionDensityRanges, "Points.Region.Density", "RegionDensity"));


            groupByDocument.Add("_id", groupFieldsDocument);
            groupByDocument.Add("Count", new BsonDocument("$sum", 1));
            groupByDocument.Add("AverageAverageSpeed", new BsonDocument("$avg", $"${nameof(MongoTrajectory.AverageSpeed)}"));
            groupByDocument.Add("MinAverageSpeed", new BsonDocument("$min", $"${nameof(MongoTrajectory.AverageSpeed)}"));
            groupByDocument.Add("MaxAverageSpeed", new BsonDocument("$max", $"${nameof(MongoTrajectory.AverageSpeed)}"));
            groupByDocument.Add("AverageDuration", new BsonDocument("$avg", $"${nameof(MongoTrajectory.Duration)}"));
            groupByDocument.Add("MinDuration", new BsonDocument("$min", $"${nameof(MongoTrajectory.Duration)}"));
            groupByDocument.Add("MaxDuration", new BsonDocument("$max", $"${nameof(MongoTrajectory.Duration)}"));
            groupByDocument.Add("AverageLength", new BsonDocument("$avg", $"${nameof(MongoTrajectory.Length)}"));
            groupByDocument.Add("MinLength", new BsonDocument("$min", $"${nameof(MongoTrajectory.Length)}"));
            groupByDocument.Add("MaxLength", new BsonDocument("$max", $"${nameof(MongoTrajectory.Length)}"));

            return groupByDocument;
        }

        private BsonDocument CreateProject(GroupFields groupFields, Ranges ranges)
        {
            var projectDocument = new BsonDocument("_id", 0);


            if (groupFields.GroupByCity == true)
                projectDocument.Add("City", $"$_id.{nameof(AggregateQueryResult.City)}");

            if (groupFields.GroupByStartDateYear == true)
                projectDocument.Add("StartDateYear", "$_id.StartDateYear");

            if (groupFields.GroupByStartDateMonth == true)
                projectDocument.Add("StartDateMonth", "$_id.StartDateMonth");

            if (groupFields.GroupByStartDateDayOfMonth == true)
                projectDocument.Add("StartDateDayOfMonth", "$_id.StartDateDayOfMonth");

            if (groupFields.GroupByStartDateHour == true)
                projectDocument.Add("StartDateHour", "$_id.StartDateHour");

            if (groupFields.GroupByStartDateDayOfWeek == true)
                projectDocument.Add("StartDateDayOfWeek", "$_id.StartDateDayOfWeek");

            if (groupFields.GroupByEndDateYear == true)
                projectDocument.Add("EndDateYear", "$_id.EndDateYear");

            if (groupFields.GroupByEndDateMonth == true)
                projectDocument.Add("EndDateMonth", "$_id.EndDateMonth");

            if (groupFields.GroupByEndDateDayOfMonth == true)
                projectDocument.Add("EndDateDayOfMonth", "$_id.EndDateDayOfMonth");

            if (groupFields.GroupByEndDateHour == true)
                projectDocument.Add("EndDateHour", "$_id.EndDateHour");

            if (groupFields.GroupByEndDateDayOfWeek == true)
                projectDocument.Add("EndDateDayOfWeek", "$_id.EndDateDayOfWeek");

            if (groupFields.GroupByWindDirection == true)
                projectDocument.Add("WindDirection", "$_id.WindDirection");

            if (groupFields.GroupByAverageSpeed != null && ranges.AverageSpeedRanges != null && ranges.AverageSpeedRanges.Count > 0)
                projectDocument.Add("AverageSpeedRange", "$_id.AverageSpeed");

            if (groupFields.GroupByDuration != null && ranges.DurationRanges != null && ranges.DurationRanges.Count > 0)
                projectDocument.Add("DurationRange", "$_id.Duration");

            if (groupFields.GroupByLength != null && ranges.LengthRanges != null && ranges.LengthRanges.Count > 0)
                projectDocument.Add("LengthRange", "$_id.Length");

            if (groupFields.GroupByTemperature != null && ranges.TemperatureRanges != null && ranges.TemperatureRanges.Count > 0)
                projectDocument.Add("TemperatureRange", "$_id.Temperature");

            if (groupFields.GroupByHumidity != null && ranges.HumidityRanges != null && ranges.HumidityRanges.Count > 0)
                projectDocument.Add("HumidityRange", "$_id.Humidity");

            if (groupFields.GroupByWindSpeed != null && ranges.WindSpeedRanges != null && ranges.WindSpeedRanges.Count > 0)
                projectDocument.Add("WindSpeedRange", "$_id.WindSpeed");

            if (groupFields.GroupByPrecipitation != null && ranges.PrecipitationRanges != null && ranges.PrecipitationRanges.Count > 0)
                projectDocument.Add("PrecipitationRange", "$_id.Precipitation");

            if (groupFields.GroupByPrecipitationDuration != null && ranges.PrecipitationDurationRanges != null && ranges.PrecipitationDurationRanges.Count > 0)
                projectDocument.Add("PrecipitationDurationRange", "$_id.PrecipitationDuration");

            if (groupFields.GroupByAirPressure != null && ranges.AirPressureRanges != null && ranges.AirPressureRanges.Count > 0)
                projectDocument.Add("AirPressureRange", "$_id.AirPressure");

            if (groupFields.GroupByRegion == true)
                projectDocument.Add("Region", "$_id.Region");

            if (groupFields.GroupByParentRegion == true)
                projectDocument.Add("ParentRegion", "$_id.ParentRegion");

            if (groupFields.GroupByRegionArea != null && ranges.RegionAreaRanges != null && ranges.RegionAreaRanges.Count > 0)
                projectDocument.Add("RegionAreaRange", "$_id.RegionArea");

            if (groupFields.GroupByRegionPopulation != null && ranges.RegionPopulationRanges != null && ranges.RegionPopulationRanges.Count > 0)
                projectDocument.Add("RegionPopulationRange", "$_id.RegionPopulation");

            if (groupFields.GroupByRegionDensity != null && ranges.RegionDensityRanges != null && ranges.RegionDensityRanges.Count > 0)
                projectDocument.Add("RegionDensityRange", "$_id.RegionDensity");


            projectDocument.Add("Count", "$Count");
            projectDocument.Add("AverageAverageSpeed", "$AverageAverageSpeed");
            projectDocument.Add("MinAverageSpeed", "$MinAverageSpeed");
            projectDocument.Add("MaxAverageSpeed", "$MaxAverageSpeed");
            projectDocument.Add("AverageDuration", "$AverageDuration");
            projectDocument.Add("MinDuration", "$MinDuration");
            projectDocument.Add("MaxDuration", "$MaxDuration");
            projectDocument.Add("AverageLength", "$AverageLength");
            projectDocument.Add("MinLength", "$MinLength");
            projectDocument.Add("MaxLength", "$MaxLength");

            return projectDocument;
        }

        private BsonDocument CreateRangeGroupBy(List<decimal> ranges, string name, string prettyName)
        {
            ranges.Sort();
            BsonArray branchesDocument = new BsonArray();
            for (int i = 0; i <= ranges.Count; i++)
            {
                BsonDocument caseDocument = new BsonDocument();

                if (i != ranges.Count)
                    caseDocument.Add("$lt", new BsonArray
                    {
                        $"${name}",
                        ranges[i]
                    });
                else
                    caseDocument.Add("$gte", new BsonArray
                    {
                        $"${name}",
                        ranges[i - 1]
                    });

                BsonDocument branchDocument = new BsonDocument("case", caseDocument);
                if (i == 0)
                    branchDocument.Add("then", $"{prettyName} less than {ranges[i]}");
                else if (i == ranges.Count)
                    branchDocument.Add("then", $"{prettyName} more than {ranges[i - 1]}");
                else
                    branchDocument.Add("then", $"{prettyName} between {ranges[i - 1]}-{ranges[i]}");

                branchesDocument.Add(branchDocument);
            }
            return new BsonDocument("$switch", new BsonDocument("branches", branchesDocument));
        }
    }
}
