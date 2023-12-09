namespace Backend.Model.Cosmos
{
    public class CosmosTrajectory
    {
        public string id {  get; set; }
        public string IdInFile { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }       
        public decimal AverageSpeed { get; set; }        
        public decimal Duration { get; set; }       
        public decimal Length { get; set; }
        public string city { get; set; }
        public Weather Weather { get; set; }
        public Fuelprice FuelPrice { get; set; }
        public Countrypopulation CountryPopulation { get; set; }
        public Economic Economic { get; set; }
        public Emissions Emissions { get; set; }
        public Point[] Points { get; set; }
    }

    public class Weather
    {       
        public decimal? Temperature { get; set; }   
        public decimal? AirPressure { get; set; }     
        public decimal? Humidity { get; set; }       
        public decimal? WindSpeed { get; set; }
        public string? WindDirection { get; set; }       
        public decimal? Precipitation { get; set; }        
        public decimal? PrecipitationDuration { get; set; }
    }

    public class Fuelprice
    {       
        public decimal GasolinePrice { get; set; }       
        public decimal DieselPrice { get; set; }
    }

    public class Countrypopulation
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

    public class Economic
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

    public class Emissions
    {
        public decimal CO2EmissionsFromTransportPercentage { get; set; }
        public decimal CO2EmissionsFromGaseousFuelConsumptionPercentage { get; set; }
    }

    public class Point
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public Region Region { get; set; }
    }

    public class Region
    {
        public string Name { get; set; }
        public decimal? Area { get; set; }
        public int? Population { get; set; }
        public decimal? Density { get; set; }
        public string? ParentName { get; set; }
    }
}
