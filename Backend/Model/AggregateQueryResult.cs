using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend.Model
{
    public class AggregateQueryResult
    {
        //[BsonIgnoreIfNull]
        //[BsonIgnoreIfDefault]
        //public ObjectId? Id { get; set; }

        public string? City { get; set; }
        public int? StartDateYear { get; set; }
        public int? StartDateMonth { get; set; }
        public int? StartDateDayOfMonth { get; set; }
        public int? StartDateDayOfWeek { get; set; }
        public int? StartDateHour { get; set; }
        public int? EndDateYear { get; set; }
        public int? EndDateMonth { get; set; }
        public int? EndDateDayOfMonth { get; set; }
        public int? EndDateDayOfWeek { get; set; }
        public int? EndDateHour { get; set; }
        public string? WindDirection { get; set; }
        public string? AverageSpeedRange { get; set; }
        public string? DurationRange { get; set; }
        public string? LengthRange { get; set; }
        public string? TemperatureRange { get; set; }
        public string? HumidityRange { get; set; }
        public string? WindSpeedRange { get; set; }
        public string? PrecipitationRange { get; set; }
        public string? PrecipitationDurationRange { get; set; }
        public string? AirPressureRange { get; set; }
        public string? Region { get; set; }
        public string? ParentRegion { get; set; }
        public string? RegionAreaRange { get; set; }
        public string? RegionPopulationRange { get; set; }
        public string? RegionDensityRange { get; set; }

        public int Count { get; set; }
        public Decimal128 AverageAverageSpeed { get; set; }
        public Decimal128 MinAverageSpeed { get; set; }
        public Decimal128 MaxAverageSpeed { get; set; }
        public Decimal128 AverageDuration { get; set; }
        public Decimal128 MinDuration { get; set; }
        public Decimal128 MaxDuration { get; set; }
        public Decimal128 AverageLength { get; set; }
        public Decimal128 MinLength { get; set; }
        public Decimal128 MaxLength { get; set; }
    }
}
