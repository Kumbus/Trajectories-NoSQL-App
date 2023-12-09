import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../Services/api.service';
import { ActivatedRoute, Route } from '@angular/router';
import L, { Polyline, icon, marker, point, polyline } from 'leaflet';

@Component({
  selector: 'app-trajectory',
  templateUrl: './trajectory.component.html',
  styleUrl: './trajectory.component.scss'
})
export class TrajectoryComponent implements OnInit{

  trajectory!: any;
  mapReady = false;
  trajectoryRoute: any
  startPoint: any
  endPoint: any

  constructor(private _apiService: ApiService,private route:ActivatedRoute){}
  ngOnInit(): void {
    this.route.params.subscribe( params =>
      {
        this._apiService.trajectory(params['id']).subscribe((trajectory:any) => {
          this.trajectory = trajectory
          this.trajectoryRoute = polyline(trajectory.points.map((p:any) => [p.latitude, p.longitude]))
          this.startPoint = marker([ trajectory.points[0]?.latitude, trajectory.points[0]?.longitude], {
            icon: icon({
              iconSize: [ 30, 50 ],
              iconAnchor: [ 13, 41 ],
              iconUrl: 'leaflet/marker-icon.png',
              shadowUrl: 'leaflet/marker-shadow.png'
            })
          });

          // Marker for the parking lot at the base of Mt. Ranier trails
          this.endPoint = marker([ trajectory.points[trajectory.points.length-1]?.latitude, trajectory.points[trajectory.points.length-1]?.longitude ], {
            icon: icon({
              iconSize: [ 25, 41 ],
              iconAnchor: [ 13, 41 ],
              iconUrl: 'leaflet/marker-icon.png',
              iconRetinaUrl: 'leaflet/marker-icon-2x.png',
              shadowUrl: 'leaflet/marker-shadow.png'
            })
          });

          this.options.layers.push(this.trajectoryRoute)
          this.options.layers.push(this.startPoint)
          this.options.layers.push(this.endPoint)
          this.options.layers.push(this.trajectoryRoute)
          this.mapReady = true
        })
      }
    )

  }

  options = {
    layers: [
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&amp;copy; OpenStreetMap contributors'
      })
    ],
    zoom: 10,
    center: L.latLng([52.3884157190797,9.71520775784172])
  };

  onMapReady(map: any) {
    map.fitBounds(this.trajectoryRoute.getBounds(), {
      padding: point(52.3884157190797,9.71520775784172),
      maxZoom: 12,
      animate: true
    })

  }

}
