import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { TrajectoriesDataSource } from '../../Interfaces/TrajectoriesDataSource';
import { ApiService } from '../../Services/api.service';
import { SharedDataService } from '../../Services/shared-data.service';
import { SimpleQueryFilter } from '../../Interfaces/SimpleQueryFilter';
import { Ranges } from '../../Interfaces/AggregationQueryFilter';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-aggregations',
  templateUrl: './aggregations.component.html',
  styleUrl: './aggregations.component.scss'
})
export class AggregationsComponent implements OnInit{
  aggregationsForm: FormGroup = this.fb.group({
      groupByCity: [false],
      groupByStartDateYear: [false],
      groupByStartDateMonth: [false],
      groupByStartDateDayOfMonth: [false],
      groupByStartDateHour: [false],
      groupByStartDateDayOfWeek: [false],
      groupByEndDateYear: [false],
      groupByEndDateMonth: [false],
      groupByEndDateDayOfMonth: [false],
      groupByEndDateHour: [false],
      groupByEndDateDayOfWeek: [false],
      groupByWindDirection: [false],
      groupByRegion: [false],
      groupByParentRegion: [false],
      groupByAverageSpeed: [false],
      groupByLength: [false],
      groupByDuration: [false],
      groupByTemperature: [false],
      groupByHumidity: [false],
      groupByWindSpeed: [false],
      groupByPrecipitation: [false],
      groupByPrecipitationDuration: [false],
      groupByAirPressure: [false],
      groupByRegionArea: [false],
      groupByRegionPopulation: [false],
      groupByRegionDensity: [false],

      averageSpeedRanges: [null],
      lengthRanges: [null],
      durationRanges: [null],
      temperatureRanges: [null],
      humidityRanges: [null],
      windSpeedRanges: [null],
      precipitationRanges: [null],
      precipitationDurationRanges: [null],
      airPressureRanges: [null],
      regionAreaRanges: [null],
      regionPopulationRanges: [null],
      regionDensityRanges: [null],

  })

  dataSource: MatTableDataSource<any>;
  displayedColumns: string[] = [];

  @ViewChild(MatPaginator)
  paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  //pageSize = 20;
  //pageIndex = 0;
  pageSizeOptions = [20,50,100];

  totalAggregations: number = 0;
  fullTimeMongoDB: number = 0;
  fullTimeCosmosDB: number = 0;
  queryTimeMongoDB: number = 0;
  queryTimeCosmosDB: number = 0;

  showResults = true;

  simpleFilter: any

  constructor(private fb: FormBuilder, private _apiService: ApiService, private router: Router,
    private _sharedDataService: SharedDataService) {
    this.dataSource = new MatTableDataSource();
  }

  ngOnInit(): void {

    this._sharedDataService.currentFilter.subscribe((filter:any) => {
      this.simpleFilter = filter

      this._sharedDataService.currentAggregationFilter.subscribe((aggregationFilter:any) => {

        this.groupFields.forEach(field =>{
          this.aggregationsForm.controls[field]?.setValue(aggregationFilter?.groupFields?.[field]);
        })

        this.rangeFields.forEach(field =>{
          this.aggregationsForm.controls[field]?.setValue(aggregationFilter?.ranges?.[field].toString());
        })

      })

    })

    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }


  rangeFields = ['averageSpeedRanges', 'lengthRanges', 'durationRanges', 'temperatureRanges', 'humidityRanges', 'windSpeedRanges', 'precipitationRanges', 'precipitationDurationRanges', 'airPressureRanges', 'regionAreaRanges', 'regionPopulationRanges', 'regionDensityRanges'];
  groupFields = ['groupByCity', 'groupByStartDateYear', 'groupByStartDateMonth', 'groupByStartDateDayOfMonth', 'groupByStartDateHour', 'groupByStartDateDayOfWeek', 'groupByEndDateYear', 'groupByEndDateMonth', 'groupByEndDateDayOfMonth', 'groupByEndDateHour', 'groupByEndDateDayOfWeek', 'groupByWindDirection', 'groupByRegion', 'groupByParentRegion', 'groupByAverageSpeed', 'groupByLength', 'groupByDuration', 'groupByTemperature', 'groupByHumidity', 'groupByWindSpeed', 'groupByPrecipitation', 'groupByPrecipitationDuration', 'groupByAirPressure', 'groupByRegionArea', 'groupByRegionPopulation', 'groupByRegionDensity'];
  noRangeGroupFields = ['groupByCity', 'groupByStartDateYear', 'groupByStartDateMonth', 'groupByStartDateDayOfMonth', 'groupByStartDateHour', 'groupByStartDateDayOfWeek', 'groupByEndDateYear', 'groupByEndDateMonth', 'groupByEndDateDayOfMonth', 'groupByEndDateHour', 'groupByEndDateDayOfWeek', 'groupByWindDirection', 'groupByRegion', 'groupByParentRegion'];
  rangesGroupFields1 = ['groupByAverageSpeed', 'groupByLength', 'groupByDuration', 'groupByTemperature', 'groupByHumidity', 'groupByWindSpeed']
  rangesGroupFields2 = ['groupByPrecipitation', 'groupByPrecipitationDuration', 'groupByAirPressure', 'groupByRegionArea', 'groupByRegionPopulation', 'groupByRegionDensity'];

  getLabelForField(field: string): string {

    return field.replace(/([A-Z])/g, ' $1').trim();
  }


  onSubmit() {
    this.showResults = false
    //this.paginator.pageIndex = 0
    let filter = this.createFilter()
    if (this.paginator) {
      //filter.page = this.paginator.pageIndex + 1; // Pages are 1-based, so add 1
    }
    //console.log(filter.page)
    this._apiService.aggregations(filter).subscribe((trajectories: any)=> {
      console.log(trajectories.item1.count)
      this.totalAggregations = trajectories.item1.count;
      this.fullTimeMongoDB = trajectories.item1.fullTime;
      this.fullTimeCosmosDB = trajectories.item2.fullTime;
      this.queryTimeMongoDB = trajectories.item1.queryTime;
      this.queryTimeCosmosDB = trajectories.item2.queryTime;
      this.displayedColumns = []
      Object.entries(trajectories.item1.results[0]).forEach((element:any) => {
        this.displayedColumns.push(element[0])
      });
      this.dataSource.data = trajectories.item1.results;
      setTimeout(() => {
        this.dataSource.paginator = this.paginator
        this.dataSource.sort = this.sort;
      })

      this.showResults = true
    })
  }

  goHome()
  {
    const filter = this.createFilter()
    this._sharedDataService.setAggregationFilter(filter);
    this.router.navigate(['']);
  }



  private createFilter() : any
  {
    var groupFields = {
      groupByCity: this.aggregationsForm.value.groupByCity ? this.aggregationsForm.value.groupByCity : null,
      groupByStartDateYear: this.aggregationsForm.value.groupByStartDateYear ? this.aggregationsForm.value.groupByStartDateYear : null,
      groupByStartDateMonth: this.aggregationsForm.value.groupByStartDateMonth ? this.aggregationsForm.value.groupByStartDateMonth : null,
      groupByStartDateDayOfMonth: this.aggregationsForm.value.groupByStartDateDayOfMonth ? this.aggregationsForm.value.groupByStartDateDayOfMonth : null,
      groupByStartDateHour: this.aggregationsForm.value.groupByStartDateHour ? this.aggregationsForm.value.groupByStartDateHour : null,
      groupByStartDateDayOfWeek: this.aggregationsForm.value.groupByStartDateDayOfWeek ? this.aggregationsForm.value.groupByStartDateDayOfWeek : null,
      groupByEndDateYear: this.aggregationsForm.value.groupByEndDateYear ? this.aggregationsForm.value.groupByEndDateYear : null,
      groupByEndDateMonth: this.aggregationsForm.value.groupByEndDateMonth ? this.aggregationsForm.value.groupByEndDateMonth : null,
      groupByEndDateDayOfMonth: this.aggregationsForm.value.groupByEndDateDayOfMonth ? this.aggregationsForm.value.groupByEndDateDayOfMonth : null,
      groupByEndDateHour: this.aggregationsForm.value.groupByEndDateHour ? this.aggregationsForm.value.groupByEndDateHour : null,
      groupByEndDateDayOfWeek: this.aggregationsForm.value.groupByEndDateDayOfWeek ? this.aggregationsForm.value.groupByEndDateDayOfWeek : null,
      groupByWindDirection: this.aggregationsForm.value.groupByWindDirection ? this.aggregationsForm.value.groupByWindDirection : null,
      groupByRegion: this.aggregationsForm.value.groupByRegion ? this.aggregationsForm.value.groupByRegion : null,
      groupByParentRegion: this.aggregationsForm.value.groupByParentRegion ? this.aggregationsForm.value.groupByParentRegion : null,
      groupByAverageSpeed: this.aggregationsForm.value.groupByAverageSpeed ? this.aggregationsForm.value.groupByAverageSpeed : null,
      groupByLength: this.aggregationsForm.value.groupByLength ? this.aggregationsForm.value.groupByLength : null,
      groupByDuration: this.aggregationsForm.value.groupByDuration ? this.aggregationsForm.value.groupByDuration : null,
      groupByTemperature: this.aggregationsForm.value.groupByTemperature ? this.aggregationsForm.value.groupByTemperature : null,
      groupByHumidity: this.aggregationsForm.value.groupByHumidity ? this.aggregationsForm.value.groupByHumidity : null,
      groupByWindSpeed: this.aggregationsForm.value.groupByWindSpeed ? this.aggregationsForm.value.groupByWindSpeed : null,
      groupByPrecipitation: this.aggregationsForm.value.groupByPrecipitation ? this.aggregationsForm.value.groupByPrecipitation : null,
      groupByPrecipitationDuration: this.aggregationsForm.value.groupByPrecipitationDuration ? this.aggregationsForm.value.groupByPrecipitationDuration : null,
      groupByAirPressure: this.aggregationsForm.value.groupByAirPressure ? this.aggregationsForm.value.groupByAirPressure : null,
      groupByRegionArea: this.aggregationsForm.value.groupByRegionArea ? this.aggregationsForm.value.groupByRegionArea : null,
      groupByRegionPopulation: this.aggregationsForm.value.groupByRegionPopulation ? this.aggregationsForm.value.groupByRegionPopulation : null,
      groupByRegionDensity: this.aggregationsForm.value.groupByRegionDensity ? this.aggregationsForm.value.groupByRegionDensity : null,
    }
    var ranges:Ranges = {
      averageSpeedRanges: this.aggregationsForm.value.averageSpeedRanges ? this.aggregationsForm.value.averageSpeedRanges.split(',').map((city: string) => Number(city.trim())) : null,
      lengthRanges: this.aggregationsForm.value.lengthRanges ? this.aggregationsForm.value.lengthRanges.split(',').map((city: string) => city.trim()) : null,
      durationRanges: this.aggregationsForm.value.durationRanges ? this.aggregationsForm.value.durationRanges.split(',').map((city: string) => city.trim()) : null,
      temperatureRanges: this.aggregationsForm.value.temperatureRanges ? this.aggregationsForm.value.temperatureRanges.split(',').map((city: string) => city.trim()) : null,
      humidityRanges: this.aggregationsForm.value.humidityRanges ? this.aggregationsForm.value.humidityRanges.split(',').map((city: string) => city.trim()) : null,
      windSpeedRanges: this.aggregationsForm.value.windSpeedRanges ? this.aggregationsForm.value.windSpeedRanges.split(',').map((city: string) => city.trim()) : null,
      precipitationRanges: this.aggregationsForm.value.precipitationRanges ? this.aggregationsForm.value.precipitationRanges.split(',').map((city: string) => city.trim()) : null,
      precipitationDurationRanges:this.aggregationsForm.value.precipitationDurationRanges ? this.aggregationsForm.value.precipitationDurationRanges.split(',').map((city: string) => city.trim()) : null,
      airPressureRanges: this.aggregationsForm.value.airPressureRanges ? this.aggregationsForm.value.airPressureRanges.split(',').map((city: string) => city.trim()) : null,
      regionAreaRanges: this.aggregationsForm.value.regionAreaRanges ? this.aggregationsForm.value.regionAreaRanges.split(',').map((city: string) => city.trim()) : null,
      regionPopulationRanges: this.aggregationsForm.value.regionPopulationRanges ? this.aggregationsForm.value.regionPopulationRanges.split(',').map((city: string) => city.trim()) : null,
      regionDensityRanges: this.aggregationsForm.value.regionDensityRanges ? this.aggregationsForm.value.regionDensityRanges.split(',').map((city: string) => city.trim()) : null,
    }

    var filter = {
      groupFields: groupFields,
      ranges: ranges,
      simpleQueryFilter: this.simpleFilter
    }

    return filter
  }

}
