using AutoMapper;
using Backend.Model;
using Backend.Model.Cosmos;
using Backend.Model.Mongo;

namespace Backend.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MongoTrajectory, SimpleQueryResult>()                
                .ForMember(dest => dest.Weather, opt => opt.MapFrom(src => src.Weather))
                .ForMember(dest => dest.FuelPrice, opt => opt.MapFrom(src => src.FuelPrice))
                .ForMember(dest => dest.CountryPopulation, opt => opt.MapFrom(src => src.CountryPopulation))
                .ForMember(dest => dest.Economic, opt => opt.MapFrom(src => src.Economic))
                .ForMember(dest => dest.Emissions, opt => opt.MapFrom(src => src.Emissions));

            CreateMap<Model.Mongo.Weather, WeatherResult>();
            CreateMap<Model.Mongo.Fuelprice, FuelpriceResult>();
            CreateMap<Model.Mongo.Countrypopulation, CountryPopulationResult>();
            CreateMap<Model.Mongo.Economic, EconomicResult>();
            CreateMap<Model.Mongo.Emissions, EmissionsResult>();


            CreateMap<CosmosTrajectory, SimpleQueryResult>()
                .ForMember(dest => dest.Weather, opt => opt.MapFrom(src => src.Weather))
                .ForMember(dest => dest.FuelPrice, opt => opt.MapFrom(src => src.FuelPrice))
                .ForMember(dest => dest.CountryPopulation, opt => opt.MapFrom(src => src.CountryPopulation))
                .ForMember(dest => dest.Economic, opt => opt.MapFrom(src => src.Economic))
                .ForMember(dest => dest.Emissions, opt => opt.MapFrom(src => src.Emissions));

            CreateMap<Model.Cosmos.Weather, WeatherResult>();
            CreateMap<Model.Cosmos.Fuelprice, FuelpriceResult>();
            CreateMap<Model.Cosmos.Countrypopulation, CountryPopulationResult>();
            CreateMap<Model.Cosmos.Economic, EconomicResult>();
            CreateMap<Model.Cosmos.Emissions, EmissionsResult>();
        }
    }
}
