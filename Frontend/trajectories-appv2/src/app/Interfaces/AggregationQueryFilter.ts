// aggregation-filter.interface.ts
export interface AggregationFilter {
groupByFields? : GroupFields,
ranges?: Ranges
  }

  export interface GroupFields {
    groupByCity?: boolean;
    groupByStartDateYear?: boolean;
    groupByStartDateMonth?: boolean;
    groupByStartDateDayOfMonth?: boolean;
    groupByStartDateHour?: boolean;
    groupByStartDateDayOfWeek?: boolean;
    groupByEndDateYear?: boolean;
    groupByEndDateMonth?: boolean;
    groupByEndDateDayOfMonth?: boolean;
    groupByEndDateHour?: boolean;
    groupByEndDateDayOfWeek?: boolean;
    groupByWindDirection?: boolean;
    groupByRegion?: boolean;
    groupByParentRegion?: boolean;
    groupByAverageSpeed?: boolean;
    groupByLength?: boolean;
    groupByDuration?: boolean;
    groupByTemperature?: boolean;
    groupByHumidity?: boolean;
    groupByWindSpeed?: boolean;
    groupByPrecipitation?: boolean;
    groupByPrecipitationDuration?: boolean;
    groupByAirPressure?: boolean;
    groupByRegionArea?: boolean;
    groupByRegionPopulation?: boolean;
    groupByRegionDensity?: boolean;
  };
  export interface Ranges {
    averageSpeedRanges?: number[];
    lengthRanges?: number[];
    durationRanges?: number[];
    temperatureRanges?: number[];
    humidityRanges?: number[];
    windSpeedRanges?: number[];
    precipitationRanges?: number[];
    precipitationDurationRanges?: number[];
    airPressureRanges?: number[];
    regionAreaRanges?: number[];
    regionPopulationRanges?: number[];
    regionDensityRanges?: number[];
  };

