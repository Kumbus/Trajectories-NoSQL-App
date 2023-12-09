namespace Backend.Model
{
    public class SimpleQueryFilter
    {
        public List<string>? Cities { get; set; }
        public decimal? AverageSpeedFrom { get; set; }
        public decimal? AverageSpeedTo { get; set; }
        public decimal? DurationFrom { get; set; }
        public decimal? DurationTo { get; set; }
        public decimal? LengthFrom { get; set; }
        public decimal? LengthTo { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public WeatherFilter? WeatherFilter { get; set; }
        public PointFilter? PointFilter { get; set; }

        public int Page {  get; set; } = 1;

    }

    public class WeatherFilter
    {
        public decimal? TemperatureFrom { get; set; }
        public decimal? TemperatureTo { get; set; }
        public decimal? AirPressureFrom { get; set; }
        public decimal? AirPressureTo { get; set; }
        public decimal? HumidityFrom { get; set; }
        public decimal? HumidityTo { get; set; }
        public decimal? WindSpeedFrom { get; set; }
        public decimal? WindSpeedTo { get; set; }
        public decimal? PrecipitationFrom { get; set; }
        public decimal? PrecipitationTo { get; set; }
        public decimal? PrecipitationDurationFrom { get; set; }
        public decimal? PrecipitationDurationTo { get; set; }
        public List<string>? WindDirections { get; set; }
    }

    public class PointFilter
    {
        public decimal? AreaFrom { get; set; }
        public decimal? AreaTo { get; set; }
        public decimal? PopulationFrom { get; set; }
        public decimal? PopulationTo { get; set; }
        public decimal? DensityFrom { get; set; }
        public decimal? DensityTo { get; set; }
        public List<string>? Names { get; set; }
        public List<string>? ParentNames { get; set; }
    }
}
