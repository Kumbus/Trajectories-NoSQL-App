export interface SimpleQueryResult {
  idInFile: string
  startDate: Date;
  endDate: Date;
  averageSpeed: number;
  duration: number;
  length: number;
  city: string;
  weather: WeatherResult;
  fuelPrice: FuelpriceResult;
  countryPopulation: CountryPopulationResult;
  economic: EconomicResult;
  emissions?: EmissionsResult;
}

export interface WeatherResult {
  temperature: number;
  airPressure: number;
  humidity: number;
  windSpeed: number;
  windDirection: string;
  precipitation: number;
  precipitationDuration: number;
}

export interface FuelpriceResult {
  gasolinePrice: number;
  dieselPrice: number;
}

export interface CountryPopulationResult {
  total: number;
  malePercentage: number;
  femalePercentage: number;
  density: number;
  urbanPercentage: number;
  ruralPercentage: number;
  age014Percentage: number;
  age1564Percentage: number;
  age65Percentage: number;
  mortalityInRoadTrafficPer100000: number;
}

export interface EconomicResult {
  gdpPerCapitaConstantLCU: number;
  gdpPerCapitaCurrentLCU: number;
  inflation: number;
  uneploymentNationalEstimate: number;
  uneploymentModeledILOEstimate: number;
  departuresInInternationalTourism: number;
  arrivalsInInternationalTourism: number;
  privateInvestmentsInTransport: number;
}

export interface EmissionsResult {
  co2EmissionsFromTransportPercentage: number;
  co2EmissionsFromGaseousFuelConsumptionPercentage: number;
}
