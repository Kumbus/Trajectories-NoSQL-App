using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend.Model
{
    public class SimpleQueryResult
    {
        public string IdInFile { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AverageSpeed { get; set; }
        public decimal Duration { get; set; }
        public decimal Length { get; set; }
        public string City { get; set; }
        public WeatherResult Weather { get; set; }
        public FuelpriceResult FuelPrice { get; set; }
        public CountryPopulationResult CountryPopulation { get; set; }
        public EconomicResult Economic { get; set; }
        public EmissionsResult Emissions { get; set; }
    }

    public class WeatherResult
    {
        public decimal Temperature { get; set; }
        public decimal AirPressure { get; set; }
        public decimal Humidity { get; set; }
        public decimal WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public decimal Precipitation { get; set; }
        public decimal PrecipitationDuration { get; set; }
    }

    public class FuelpriceResult
    {
        public decimal GasolinePrice { get; set; }
        public decimal DieselPrice { get; set; }
    }

    public class CountryPopulationResult
    {
        public int Total { get; set; }
        public decimal MalePercentage { get; set; }
        public decimal FemalePercentage { get; set; }
        public decimal Density { get; set; }
        public decimal UrbanPercentage { get; set; }
        public decimal RuralPercentage { get; set; }
        public decimal Age014Percentage { get; set; }
        public decimal Age1564Percentage { get; set; }
        public decimal Age65Percentage { get; set; }
        public decimal MortalityInRoadTrafficPer100000 { get; set; }
    }

    public class EconomicResult
    {
        public decimal GDPPerCapitaConstantLCU { get; set; }
        public decimal GDPPerCapitaCurrentLCU { get; set; }
        public decimal Inflation { get; set; }
        public decimal UneploymentNationalEstimate { get; set; }
        public decimal UneploymentModeledILOEstimate { get; set; }
        public int DeparturesInInternationalTourism { get; set; }
        public int ArrivalsInInternationalTourism { get; set; }
        public long PrivateInvestmentsInTransport { get; set; }
    }

    public class EmissionsResult
    {
        public decimal CO2EmissionsFromTransportPercentage { get; set; }
        public decimal CO2EmissionsFromGaseousFuelConsumptionPercentage { get; set; }
    }
}
