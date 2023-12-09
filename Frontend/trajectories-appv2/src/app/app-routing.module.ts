import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MapComponent } from './Components/map/map.component';
import { TrajectoriesComponent } from './Components/trajectories/trajectories.component';
import { TrajectoryComponent } from './Components/trajectory/trajectory.component';
import { AggregationsComponent } from './Components/aggregations/aggregations.component';

const routes: Routes = [
  {
    path:'map',
    component: MapComponent
  },
  {
    path:'',
    component: TrajectoriesComponent
  },
  {
    path: 'trajectory/:id',
    component: TrajectoryComponent
  },
  {
    path: 'aggregations',
    component:AggregationsComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
