namespace Backend.Model
{
    public class AggregateQueryFilter
    {
        public GroupFields? GroupFields { get; set; }
        public Ranges? Ranges { get; set; }
        public SimpleQueryFilter? SimpleQueryFilter { get; set; }
    }

    public class GroupFields
    {
        public bool? GroupByCity { get; set; }
        public bool? GroupByStartDateYear { get; set; }
        public bool? GroupByStartDateMonth { get; set; }
        public bool? GroupByStartDateDayOfMonth { get; set; }
        public bool? GroupByStartDateHour { get; set; }
        public bool? GroupByStartDateDayOfWeek { get; set; }
        public bool? GroupByEndDateYear { get; set; }
        public bool? GroupByEndDateMonth { get; set; }
        public bool? GroupByEndDateDayOfMonth { get; set; }
        public bool? GroupByEndDateHour { get; set; }
        public bool? GroupByEndDateDayOfWeek { get; set; }
        public bool? GroupByWindDirection { get; set; }
        public bool? GroupByRegion { get; set; }
        public bool? GroupByParentRegion { get; set; }
        public bool? GroupByAverageSpeed { get; set; }
        public bool? GroupByLength { get; set; }
        public bool? GroupByDuration { get; set; }
        public bool? GroupByTemperature { get; set; }
        public bool? GroupByHumidity { get; set; }
        public bool? GroupByWindSpeed { get; set; }
        public bool? GroupByPrecipitation { get; set; }
        public bool? GroupByPrecipitationDuration { get; set; }
        public bool? GroupByAirPressure { get; set; }
        public bool? GroupByRegionArea { get; set; }
        public bool? GroupByRegionPopulation { get; set; }
        public bool? GroupByRegionDensity { get; set; }
    }

    public class Ranges
    {
        public List<decimal>? AverageSpeedRanges { get; set; }
        public List<decimal>? LengthRanges { get; set; }
        public List<decimal>? DurationRanges { get; set; }
        public List<decimal>? TemperatureRanges { get; set; }
        public List<decimal>? HumidityRanges { get; set; }
        public List<decimal>? WindSpeedRanges { get; set; }
        public List<decimal>? PrecipitationRanges { get; set; }
        public List<decimal>? PrecipitationDurationRanges { get; set; }
        public List<decimal>? AirPressureRanges { get; set; }
        public List<decimal>? RegionAreaRanges { get; set; }
        public List<decimal>? RegionPopulationRanges { get; set; }
        public List<decimal>? RegionDensityRanges { get; set; }

    }
}
