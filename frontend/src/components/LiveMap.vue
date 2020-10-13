import Vue from 'vue'
// import openlayer css for style
import "ol/ol.css";
// This is library of openlayer for handle map
import Map from "ol/Map";
import View from "ol/View";
import { defaults as defaultControls, ScaleLine } from "ol/control";
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import { OSM, Vector as VectorSource } from 'ol/source';

export default {
    template: `<div id="map"></div>`,
    methods: {
            mounted: function () {
                console.log("Ja");
                this.initiateMap();
            },
            initiateMap: function () {

                // create vector layer
                var source = new VectorSource();
                var vector = new VectorLayer({
                    source: source
                });
                // create title layer
                var raster = new TileLayer({
                    source: new OSM(),
                });
                // create map with 2 layer
                new Map({
                    controls: defaultControls().extend([
                        new ScaleLine({
                            units: "degrees",
                        }),
                    ]),
                    target: "map",
                    layers: [raster, vector],
                    view: new View({
                        projection: "EPSG:4326",
                        center: [5.0499, 51.6499],
                        zoom: 16,
                    }),
                });

            }
        }