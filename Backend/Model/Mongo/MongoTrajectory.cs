using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Model.Mongo
{
    public class MongoTrajectory
    {
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public ObjectId? Id { get; set; }
        public string IdInFile { get; set; }
        [BsonIgnoreIfNull]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal AverageSpeed { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Duration { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Length { get; set; }
        public string city { get; set; }
        public Weather Weather { get; set; }
        [BsonIgnoreIfNull]
        public Fuelprice FuelPrice { get; set; }
        public Countrypopulation CountryPopulation { get; set; }
        public Economic Economic { get; set; }
        public Emissions Emissions { get; set; }
        [BsonIgnoreIfNull]
        public Point[] Points { get; set; }
    }

    public class Weather
    {
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Temperature { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal AirPressure { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Humidity { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal WindSpeed { get; set; }
        public string WindDirection { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Precipitation { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal PrecipitationDuration { get; set; }
    }

    public class Fuelprice
    {
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal GasolinePrice { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal DieselPrice { get; set; }
    }

    public class Countrypopulation
    {
        public int Total { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MalePercentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal FemalePercentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Density { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal UrbanPercentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal RuralPercentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement("Age0-14Percentage")]
        public decimal Age014Percentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement("Age15-64Percentage")]
        public decimal Age1564Percentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement("Age65+Percentage")]
        public decimal Age65Percentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal MortalityInRoadTrafficPer100000 { get; set; }
    }

    public class Economic
    {
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal GDPPerCapitaConstantLCU { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal GDPPerCapitaCurrentLCU { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Inflation { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal UneploymentNationalEstimate { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal UneploymentModeledILOEstimate { get; set; }
        public int DeparturesInInternationalTourism { get; set; }
        public int ArrivalsInInternationalTourism { get; set; }
        public long PrivateInvestmentsInTransport { get; set; }
    }

    public class Emissions
    {
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal CO2EmissionsFromTransportPercentage { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal CO2EmissionsFromGaseousFuelConsumptionPercentage { get; set; }
    }

    public class Point
    {
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Latitude { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public Region Region { get; set; }
    }

    public class Region
    {
        public string Name { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Area { get; set; }
        public int Population { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Density { get; set; }
        public string ParentName { get; set; }
    }
}
