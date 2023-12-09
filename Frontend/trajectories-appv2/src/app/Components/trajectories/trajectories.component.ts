import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ApiService } from '../../Services/api.service';
import { SimpleQueryFilter } from '../../Interfaces/SimpleQueryFilter';
import { TrajectoriesDataSource } from '../../Interfaces/TrajectoriesDataSource';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Router } from '@angular/router';
import { SharedDataService } from '../../Services/shared-data.service';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-trajectories',
  templateUrl: './trajectories.component.html',
  styleUrl: './trajectories.component.scss'
})
export class TrajectoriesComponent implements OnInit {
  filterForm: FormGroup = this.fb.group({
    cities: [null],
    averageSpeedFrom: [null],
    averageSpeedTo: [null],
    durationFrom: [null],
    durationTo: [null],
    lengthFrom: [null],
    lengthTo: [null],
    startDateFrom: [null],
    startDateTo: [null],
    endDateFrom: [null],
    endDateTo: [null],
    weatherFilter: this.fb.group({
      temperatureFrom: [null],
      temperatureTo: [null],
      airPressureFrom: [null],
      airPressureTo: [null],
      humidityFrom: [null],
      humidityTo: [null],
      windSpeedFrom: [null],
      windSpeedTo: [null],
      precipitationFrom: [null],
      precipitationTo: [null],
      precipitationDurationFrom: [null],
      precipitationDurationTo: [null],
      windDirections: [null],
    }),
    pointFilter: this.fb.group({
      areaFrom: [null],
      areaTo: [null],
      populationFrom: [null],
      populationTo: [null],
      densityFrom: [null],
      densityTo: [null],
      names: [null],
      parentNames: [null],
    })
  })

  displayedColumns: string[] = ['startDate', 'endDate', 'averageSpeed', 'duration', 'length', 'city', 'details'/* Add other columns as needed */];
  dataSource: TrajectoriesDataSource = new TrajectoriesDataSource;

  @ViewChild(MatPaginator)
  paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  pageSize = 100;
  pageIndex = 0;
  pageSizeOptions = [100];

  totalTrajectories: number = 0;
  fullTimeMongoDB: number = 0;
  fullTimeCosmosDB: number = 0;
  queryTimeMongoDB: number = 0;
  queryTimeCosmosDB: number = 0;

  showResults = true;

  constructor(private fb: FormBuilder, private _apiService: ApiService, private router: Router,
    private _sharedDataService: SharedDataService) {
    this.dataSource = new TrajectoriesDataSource();
  }

  ngOnInit(): void {

    this._sharedDataService.currentFilter.subscribe((filter:any) => {
      this.filterForm.controls['cities'].setValue(filter?.cities?.join(", "))
      this.filterForm.controls['averageSpeedFrom'].setValue(filter?.averageSpeedFrom)
      this.filterForm.controls['averageSpeedTo'].setValue(filter?.averageSpeedTo);
      this.filterForm.controls['durationFrom'].setValue(filter?.durationFrom);
      this.filterForm.controls['durationTo'].setValue(filter?.durationTo);
      this.filterForm.controls['lengthFrom'].setValue(filter?.lengthFrom);
      this.filterForm.controls['lengthTo'].setValue(filter?.lengthTo);
      this.filterForm.controls['startDateFrom'].setValue(filter?.startDateFrom);
      this.filterForm.controls['startDateTo'].setValue(filter?.startDateTo);
      this.filterForm.controls['endDateFrom'].setValue(filter?.endDateFrom);
      this.filterForm.controls['endDateTo'].setValue(filter?.endDateTo);

      // WeatherFilter controls
      const weatherFilter = filter?.weatherFilter;
      if (weatherFilter) {
        this.filterForm.get('weatherFilter.temperatureFrom')?.setValue(weatherFilter.temperatureFrom);
        this.filterForm.get('weatherFilter.temperatureTo')?.setValue(weatherFilter.temperatureTo);
        this.filterForm.get('weatherFilter.airPressureFrom')?.setValue(weatherFilter.airPressureFrom);
        this.filterForm.get('weatherFilter.airPressureTo')?.setValue(weatherFilter.airPressureTo);
        this.filterForm.get('weatherFilter.humidityFrom')?.setValue(weatherFilter.humidityFrom);
        this.filterForm.get('weatherFilter.humidityTo')?.setValue(weatherFilter.humidityTo);
        this.filterForm.get('weatherFilter.windSpeedFrom')?.setValue(weatherFilter.windSpeedFrom);
        this.filterForm.get('weatherFilter.windSpeedTo')?.setValue(weatherFilter.windSpeedTo);
        this.filterForm.get('weatherFilter.precipitationFrom')?.setValue(weatherFilter.precipitationFrom);
        this.filterForm.get('weatherFilter.precipitationTo')?.setValue(weatherFilter.precipitationTo);
        this.filterForm.get('weatherFilter.precipitationDurationFrom')?.setValue(weatherFilter.precipitationDurationFrom);
        this.filterForm.get('weatherFilter.precipitationDurationTo')?.setValue(weatherFilter.precipitationDurationTo);
        this.filterForm.get('weatherFilter.windDirections')?.setValue(weatherFilter.windDirections?.join(', '));
      }

      // PointFilter controls
      const pointFilter = filter?.pointFilter;
      if (pointFilter) {
        this.filterForm.get('pointFilter.areaFrom')?.setValue(pointFilter.areaFrom);
        this.filterForm.get('pointFilter.areaTo')?.setValue(pointFilter.areaTo);
        this.filterForm.get('pointFilter.populationFrom')?.setValue(pointFilter.populationFrom);
        this.filterForm.get('pointFilter.populationTo')?.setValue(pointFilter.populationTo);
        this.filterForm.get('pointFilter.densityFrom')?.setValue(pointFilter.densityFrom);
        this.filterForm.get('pointFilter.densityTo')?.setValue(pointFilter.densityTo);
        this.filterForm.get('pointFilter.names')?.setValue(pointFilter.names?.join(', '));
        this.filterForm.get('pointFilter.parentNames')?.setValue(pointFilter.parentNames?.join(', '));
      }

    })

    this.dataSource.paginator = this.paginator;
  }

  onSubmit() {
    this.showResults = false
    this.paginator.pageIndex = 0
    let filter = this.createFilter()
    if (this.paginator) {
      filter.page = this.paginator.pageIndex + 1; // Pages are 1-based, so add 1
    }
    console.log(filter.page)
    this._apiService.trajectories(filter).subscribe((trajectories: any)=> {
      console.log(trajectories.item1.count)
      this.totalTrajectories = trajectories.item1.count;
      this.fullTimeMongoDB = trajectories.item1.fullTime;  // Update with actual property names from your API response
      this.fullTimeCosmosDB = trajectories.item2.fullTime;  // Update with actual property names from your API response
      this.queryTimeMongoDB = trajectories.item1.queryTime;  // Update with actual property names from your API response
      this.queryTimeCosmosDB = trajectories.item2.queryTime;
      setTimeout(() => {
        this.paginator.length = trajectories.item1.count
        this.paginator.pageIndex = 0
      })
      this.dataSource.data = trajectories.item1.results;
      this.dataSource.paginator = this.paginator
      this.dataSource.sort = this.sort;

      this.showResults = true
    })
  }

  onPageChange = (event: PageEvent) => {
    this.showResults = false
    let filter = this.createFilter()
    filter.page = event.pageIndex + 1

    this._apiService.trajectories(filter).subscribe((trajectories: any)=> {
      console.log(trajectories.item1.count)
      setTimeout(() => {
        this.paginator.length = trajectories.item1.count
        this.pageSize = event.pageSize;
        this.pageIndex = event.pageIndex;
      })
      this.dataSource.data = trajectories.item1.results;
      this.showResults = true
      //this.dataSource.paginator = this.paginator
    })
  }

  openMap() {
    const filter = this.createFilter()
    this._sharedDataService.setFilter(filter);
    this.router.navigate(['/map']);
  }

  goToAggregations(){
    const filter = this.createFilter()
    this._sharedDataService.setFilter(filter);
    this.router.navigate(['/aggregations']);
  }

  openDetails(id: string) {
    // Navigate to /idinfile with the trajectory ID as a parameter
    const filter = this.createFilter()
    this._sharedDataService.setFilter(filter);
    this.router.navigate(['/trajectory', id]);
  }

  private createFilter() : SimpleQueryFilter {
    const cities = this.filterForm.value.cities ? this.filterForm.value.cities.split(',').map((city: string) => city.trim()) : null;
    const windDirections = this.filterForm.value.weatherFilter?.windDirections ? this.filterForm.value.weatherFilter.windDirections.split(',').map((direction: string) => direction.trim()) : null;
    const names = this.filterForm.value.pointFilter?.names ? this.filterForm.value.pointFilter.names.split(',').map((name: string) => name.trim()) : null;
    const parentNames = this.filterForm.value.pointFilter?.parentNames ? this.filterForm.value.pointFilter.parentNames.split(',').map((parentName: string) => parentName.trim()) : null;

    return {
      ...this.filterForm.value,
      cities,
      weatherFilter: {
        ...this.filterForm.value.weatherFilter,
        windDirections,
      },
      pointFilter: {
        ...this.filterForm.value.pointFilter,
        names,
        parentNames,
      },
      page : 1
    }
  }
}
