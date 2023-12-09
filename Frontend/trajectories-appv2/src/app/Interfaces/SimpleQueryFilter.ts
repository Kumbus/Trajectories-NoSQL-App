export interface SimpleQueryFilter {
  cities?: string[] | null;
  averageSpeedFrom?: number | null;
  averageSpeedTo?: number | null;
  durationFrom?: number | null;
  durationTo?: number | null;
  lengthFrom?: number | null;
  lengthTo?: number | null;
  startDateFrom?: Date | null;
  startDateTo?: Date | null;
  endDateFrom?: Date | null;
  endDateTo?: Date | null;
  weatherFilter?: WeatherFilter | null;
  pointFilter?: PointFilter | null;
  page: number;
}

export interface WeatherFilter {
  temperatureFrom?: number | null;
  temperatureTo?: number | null;
  airPressureFrom?: number | null;
  airPressureTo?: number | null;
  humidityFrom?: number | null;
  humidityTo?: number | null;
  windSpeedFrom?: number | null;
  windSpeedTo?: number | null;
  precipitationFrom?: number | null;
  precipitationTo?: number | null;
  precipitationDurationFrom?: number | null;
  precipitationDurationTo?: number | null;
  windDirections?: string[] | null;
}

export interface PointFilter {
  areaFrom?: number | null;
  areaTo?: number | null;
  populationFrom?: number | null;
  populationTo?: number | null;
  densityFrom?: number | null;
  densityTo?: number | null;
  names?: string[] | null;
  parentNames?: string[] | null;
}
