import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { SimpleQueryFilter } from '../Interfaces/SimpleQueryFilter';
import { AggregationFilter } from '../Interfaces/AggregationQueryFilter';

@Injectable({
  providedIn: 'root'
})
export class SharedDataService {

  private filterSource = new BehaviorSubject<SimpleQueryFilter | null | undefined>(null);
  currentFilter = this.filterSource.asObservable();

  private aggregateSource = new BehaviorSubject<AggregationFilter | null | undefined>(null);
  currentAggregationFilter = this.aggregateSource.asObservable();

  setFilter(filter: SimpleQueryFilter) {
    this.filterSource.next(filter)
  }

  setAggregationFilter(filter: AggregationFilter) {
    this.aggregateSource.next(filter);
  }
}
