import { Component, OnInit } from '@angular/core';
import * as L from 'leaflet';
import 'leaflet.heat/dist/leaflet-heat.js'
import 'leaflet.heat';
import { ApiService } from '../../Services/api.service';
import { ActivatedRoute } from '@angular/router';
import { SharedDataService } from '../../Services/shared-data.service';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrl: './map.component.scss'
})
export class MapComponent implements OnInit
{
  coordinates=[]
  mapReady = false;
  leafletCenter = L.latLng([52.3884157190797,9.71520775784172])
  constructor(private _apiService: ApiService, private route: ActivatedRoute, private _sharedDataService: SharedDataService) {}
  ngOnInit(): void {
    this._sharedDataService.currentFilter.subscribe((filter:any) => {
      this._apiService.coordinates(filter).subscribe((points: any) => {
        this.coordinates = points
        this.mapReady = true
      })
    })
  }

    options = {
      layers: [
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '&amp;copy; OpenStreetMap contributors'
        }),
      ],
      zoom: 10,
      center: L.latLng([52.3884157190797,9.71520775784172])
    };

    onMapReady(map: any) {
          L.heatLayer(this.coordinates, heatLayerConfig2).addTo(map);

    }

    goToBeijing() {
      this.leafletCenter = L.latLng(39.9042, 116.4074)
    }

    goToHannover() {
      this.leafletCenter = L.latLng([52.3884157190797,9.71520775784172])
    }


}

  export const heatLayerConfig2 = {
    "radius": 8,
    "maxOpacity": 1,
    "scaleRadius": false,
    // property below is responsible for colorization of heat layer
    "useLocalExtrema": true,
    // here we need to assign property value which represent lat in our data
    latField: 'lat',
    // here we need to assign property value which represent lng in our data
    lngField: 'lng',
  };
